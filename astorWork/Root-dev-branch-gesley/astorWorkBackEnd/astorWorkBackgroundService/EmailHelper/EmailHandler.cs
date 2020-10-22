using astorWorkBackgroundService.Models;
using astorWorkDAO;
using astorWorkShared.GlobalModels;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using DinkToPdf.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkBackgroundService.EmailHelper
{
    class EmailHandler
    {
        protected IAstorWorkEmail _emailService;
        protected astorWorkDbContext _astorWorkDbContext;
        protected TenantInfo _tenantInfo;
        protected string _hostname;
        protected IConverter _converter;

        public EmailHandler(IAstorWorkEmail emailService, astorWorkDbContext astorWorkDbContext, TenantInfo tenantInfo)
        {
            _emailService = emailService;
            _astorWorkDbContext = astorWorkDbContext;
            UpdateNotificationAuditService updateNotificationAuditService = new UpdateNotificationAuditService(astorWorkDbContext);
            updateNotificationAuditService._context = astorWorkDbContext;
            _tenantInfo = tenantInfo;
            _hostname = tenantInfo.RowKey == "localhost" ? "localhost:4200" : tenantInfo.RowKey + "." + AppConfiguration.GetHostName();
        }

        public async Task SendNewMRFEmail(NotificationAudit notificationAudit, IConverter converter)
        {
            this._converter = converter;

            try
            {
                var recipients = notificationAudit.Recipients;
                string[] recipientAddress = recipients.Select(p => p.Email).ToArray();
                string[] recipientNames = recipients.Select(p => p.PersonName).ToArray();
                int MRFID = Convert.ToInt32(notificationAudit.Reference);
                MRFMaster mrf = _astorWorkDbContext.MRFMaster.Include(m => m.Materials).Where(m => m.ID == MRFID).FirstOrDefault();

                string MRFNo = mrf.MRFNo;
                int noOfMaterials = mrf.Materials.Count;
                PageMaster pageMaster = _astorWorkDbContext.PageMaster.Where(p => p.UrlPath == "materials").FirstOrDefault();
                string fullurl = _astorWorkDbContext.GetPageFullUrl(pageMaster);
                //string strHost = _tenantInfo.Hostname == null ? "localhost:4200" : _tenantInfo.Hostname;
                string link = $"http://{ _hostname + fullurl};mrfNo={MRFNo}".Trim();
                //@"http://"+ _hostname + fullurl + ";mrfNo=" + MRFNo;
                string[] notificationParams = new string[] { MRFNo, noOfMaterials.ToString(), link, mrf.CreatedDate.ToString() };

                string subject = string.Format("New MRF ({0}) is Created", MRFNo);
                List<string> attachmentPaths = new List<string>();
                MRFMaster mrfMaster = await _astorWorkDbContext.MRFMaster
                                                               .Include(m => m.Materials)
                                                               .ThenInclude(m => m.MaterialType)
                                                               .Where(m => m.ID == Convert.ToInt32(notificationAudit.Reference))
                                                               .FirstOrDefaultAsync();

                attachmentPaths.Add(CreateMRFAttachment(mrfMaster, converter));
                await _emailService.SendBulk(recipientAddress, recipientNames, subject, 0, notificationParams, attachmentPaths);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public async Task SendNewBIMSyncEmail(NotificationAudit notificationAudit)
        {
            try
            {
                var recipientAddress = notificationAudit.Recipients.Select(p => p.Email).ToArray();
                var recipientNames = notificationAudit.Recipients.Select(p => p.PersonName).ToArray();
                int syncId = Convert.ToInt32(notificationAudit.Reference);
                var syncSession = await _astorWorkDbContext.BIMSyncAudit.FindAsync(syncId);

                var syncDateTime = syncSession.SyncTime.ToString("MM/dd/yyyy HH:mm:ss");
                var countSynced = syncSession.SyncedMaterialIds.Split(',').Length.ToString();
                var countNotSynced = syncSession.UnsyncedMaterialIds.Split(',').Length.ToString();

                PageMaster pageMaster = _astorWorkDbContext.PageMaster.Where(p => p.UrlPath == "bim-syncs").FirstOrDefault();
                string fullurl = _astorWorkDbContext.GetPageFullUrl(pageMaster);
                //string strHost = _tenantInfo.Hostname == null ? "localhost:4200" : _tenantInfo.Hostname;
                string link = $"http://{ _hostname + fullurl}/{syncId}".Trim();
                //@"http://" + _hostname + fullurl + "/" + syncId;

                string[] notificationParams = new string[] { syncDateTime, countSynced, countNotSynced, link };

                string subject = string.Format("New sync happened for model {0}", syncSession.BIMModelId);

                await _emailService.SendBulk(recipientAddress, recipientNames, subject, (int)Enums.NotificationCode.BIMSync, notificationParams);

            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public async Task SendDeliveryMaterialsEmail(NotificationAudit notificationAudit, string tenant, IConverter converter)
        {
            this._converter = converter;

            try
            {
                string projectOrVendorName = GetProjectOrVendorName(notificationAudit);
                IEnumerable<DeliveryMaterial> deliveryMaterials = await GetDeliveryMaterials(notificationAudit);

                if (deliveryMaterials.Count() > 0)
                {
                    List<string> attachmentPaths = new List<string>();
                    attachmentPaths.Add(CreateDeliveryAttachment(tenant, projectOrVendorName, deliveryMaterials));

                    await SendEmails(notificationAudit, projectOrVendorName, attachmentPaths);
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public string CreateDeliveryAttachment(string tenant, string projectOrVendorName, IEnumerable<DeliveryMaterial> deliveryMaterials)
        {
            string filePath = string.Format("{0}-{1}.pdf", tenant, projectOrVendorName);
            string header = "Delayed Delivery Materials (" + DateTime.Now.ToString("dd/MM/yyyy") + ")";
            string subHeader = tenant + " - " + projectOrVendorName;

            return _emailService.CreateDocument(filePath, header, subHeader, AttachmentHandler.CreateDeliveryTblContent(deliveryMaterials), _converter, _tenantInfo.LogoImageURL);
        }

        public string CreateMRFAttachment(MRFMaster mrfMaster, IConverter converter)
        {
            this._converter = converter;

            string filePath = string.Format("MRF {0} Created.pdf", mrfMaster.MRFNo);
            //string header = string.Format("{0} Created", mrfMaster.MRFNo);
            //string subHeader = DateTime.Now.ToString("dd/MM/yyyy");

            return _emailService.CreateDocument(filePath, "", "", AttachmentHandler.CreateMRFTblContent(mrfMaster), _converter, _tenantInfo.LogoImageURL);
        }

        public async Task SendEmails(NotificationAudit notificationAudit, string projectOrVendorName, List<string> attachmentPaths)
        {
            IEnumerable<UserMaster> receipients = notificationAudit.Recipients;
            string[] recipientAddress = receipients.Select(r => r.Email).ToArray();
            string[] recipientNames = receipients.Select(r => r.PersonName).ToArray();

            //PageMaster pageMaster = _astorWorkDbContext.PageMaster.Where(p => p.UrlPath == "materials").FirstOrDefault();
            //string fullurl = _astorWorkDbContext.GetPageFullUrl(pageMaster);
            //string strHost = _tenantInfo.Hostname == "localhost" ? "localhost:4200" : _tenantInfo.Hostname;
            //string link = @"http://" + strHost + fullurl + ";mrfNo=" + delayedDeliveryMaterials.FirstOrDefault().MRF.MRFNo;

            string subject = string.Empty;
            string templateText = string.Empty;

            if (notificationAudit.Code == Convert.ToInt32(Enums.NotificationCode.DelayInDelivery))
            {
                subject = string.Format("Undelivered Materials for {0}", projectOrVendorName);
                templateText = "past their expected delivery date";
            }
            else
            {
                subject = string.Format("Materials delivery expected today for {0}", projectOrVendorName);
                templateText = "expected to deliver today";
            }

            string[] notificationParams = new string[] { templateText, projectOrVendorName };

            await _emailService.SendBulk(recipientAddress, recipientNames, subject, notificationAudit.Code, notificationParams, attachmentPaths);
        }



        public async Task<IEnumerable<DeliveryMaterial>> GetDeliveryMaterials(NotificationAudit notificationAudit)
        {
            UpdateNotificationAuditService updateNotificationAuditService = new UpdateNotificationAuditService(_astorWorkDbContext);

            bool isProjectID = notificationAudit.NotificationTimer.Project != null;

            if (notificationAudit.Code == Convert.ToInt32(Enums.NotificationCode.DelayInDelivery))
                return updateNotificationAuditService.GetDelayedDeliveryMaterials(Convert.ToInt32(notificationAudit.Reference), isProjectID);
            else
                return updateNotificationAuditService.GetExpectedDeliveryMaterials(Convert.ToInt32(notificationAudit.Reference), isProjectID);
        }

        public string GetProjectOrVendorName(NotificationAudit notificationAudit)
        {

            if (notificationAudit.NotificationTimer.Project != null)
            {
                ProjectMaster project = _astorWorkDbContext.ProjectMaster.Where(p => p.ID == Convert.ToInt32(notificationAudit.Reference)).FirstOrDefault();
                return project.Name;
            }
            else
            {
                OrganisationMaster vendor = _astorWorkDbContext.OrganisationMaster.Where(p => p.ID == Convert.ToInt32(notificationAudit.Reference)).FirstOrDefault();
                return vendor.Name;
            }
        }

        public string GetQCFailLink(int qcids, string caseName)
        {
            PageMaster pageMaster = _astorWorkDbContext.PageMaster.Where(p => p.UrlPath == "qc-defects").FirstOrDefault();
            string fullurl = _astorWorkDbContext.GetPageFullUrl(pageMaster);
            //string strHost = _tenantInfo.Hostname == null ? "localhost:4200" : _tenantInfo.Hostname;
            string link = $"http://{ _hostname + fullurl};qcids={qcids};caseName={caseName}".Trim();
            return link.Replace(":id", "0");
        }

        public async Task SendQCFailEmail(NotificationAudit notificationAudit)
        {
            try
            {
                MaterialQCDefect qcDefect = _astorWorkDbContext.MaterialQCDefect
                                            .Include(d => d.QCCase)
                                            .ThenInclude(c => c.MaterialMaster)
                                            .Where(d => d.ID == Convert.ToInt32(notificationAudit.Reference)).FirstOrDefault();

                if (qcDefect == null)
                    return;

                var UserMaster = notificationAudit.Recipients;
                string[] recipientAddress = UserMaster.Select(p => p.Email).ToArray();
                string[] recipientNames = UserMaster.Select(p => p.PersonName).ToArray();
                string subject = string.Empty;
                string msg = string.Empty;
                int defectCount = 0;

                string markingNo = $"{qcDefect.QCCase.MaterialMaster.Block}-{qcDefect.QCCase.MaterialMaster.Level}-{qcDefect.QCCase.MaterialMaster.Zone}-{ qcDefect.QCCase.MaterialMaster.MarkingNo}"
                 ;
                string caseName = qcDefect.QCCase.CaseName;

                string link = GetQCFailLink(qcDefect.QCCase.ID, caseName);

                if (notificationAudit.Code == Convert.ToInt32(Enums.NotificationCode.QCFailed))
                {
                    defectCount = _astorWorkDbContext.MaterialQCDefect.Where(d => d.QCCaseID == qcDefect.QCCaseID && d.Status < Enums.QCStatus.QC_passed_by_Maincon).Count();
                    if (defectCount == 1)
                    {
                        string createdBy = qcDefect.QCCase.CreatedBy.PersonName;
                        string createdDate = qcDefect.QCCase.CreatedDate.ToString("dd/MM/yyyy");
                        msg = string.Format("A new QC Case, {0} was created by {1} on {2}, with a new Defect, for the Material {3}", caseName, createdBy, createdDate, markingNo);
                        subject = string.Format("New QCCase {0} Created, for material {1}", caseName, markingNo);
                    }
                    else
                    {
                        string createdBy = qcDefect.CreatedBy.PersonName;
                        string createdDate = qcDefect.CreatedDate.ToString("dd/MM/yyyy");
                        string remarks = qcDefect.Remarks;
                        StringBuilder sb = new StringBuilder();
                        
                      
                        sb.Append(
                        @"<table border = '1' cellspacing = '0' cellpadding = '0' style = 'border-collapse:collapse; border: none; font-family:arial'><tr>");
                      
                        string[] colNames = new string[] { "User Created", "Case Name", "Material", "Remarks" };

                        for (int i = 0; i < colNames.Length; i++)
                            sb.AppendFormat(string.Format(@"<td width = '250' valign = 'top' style = 'width:100.1pt;border:solid windowtext 1.0pt;padding:0cm 5.4pt 0cm 5.4pt; font-size:10.0pt' >{0}</td>", colNames[i]));

                        sb.AppendFormat("</tr>");

                        string[] colData = new string[] { createdBy, caseName, markingNo, remarks };

                        sb.AppendFormat(@"<tr>");

                        for (int i = 0; i < colData.Length; i++)
                             sb.AppendFormat(string.Format(@"<td width = '200' valign = 'top' style = 'width:100.1pt;border:solid windowtext 1.0pt;padding:0cm 5.4pt 0cm 5.4pt; font-size:10.0pt' >{0}</td>", colData[i]));

                        sb.AppendFormat(@"</tr>");
                        sb.Append(@"</table>");
                        msg = sb.ToString();
                        subject = string.Format("A new defect under case ({0}) is created ", caseName, markingNo);
                    }
                }
                else
                {
                    defectCount = _astorWorkDbContext.MaterialQCDefect.Where(d => d.QCCaseID == qcDefect.QCCaseID && d.Status < Enums.QCStatus.QC_passed_by_Maincon).Count();
                    if (defectCount == 0)
                    {
                        msg = "Case " + caseName + " closed as all the defects under the case are closed for the material " + markingNo + ".";
                        subject = string.Format("Case {0}, for material {1} is closed", caseName, markingNo);
                    }
                    else if (_astorWorkDbContext.MaterialQCDefect.Where(d => d.QCCaseID == qcDefect.QCCaseID && d.Status < Enums.QCStatus.QC_rectified_by_Subcon).Count() > 0)
                    {
                        msg = "Defect has been rectified for material "+ markingNo +" under " + caseName + ".";
                        subject = string.Format("Defect under case {0} Rectified", caseName, markingNo);
                    }
                    else
                    {
                        msg = "Defect has been closed for material "+ markingNo +" with QC Name or under  " + caseName + ".";
                        subject = string.Format("Defect ({0}) closed", caseName, markingNo);
                    }
                }
                string[] notificationParams = new string[] { msg, link };
                await _emailService.SendBulk(recipientAddress, recipientNames, subject, notificationAudit.Code, notificationParams);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public async Task SendCloseMRFEmail(NotificationAudit notificationAudit)
        {
            try
            {
                var UserMaster = notificationAudit.Recipients;
                string[] recipientAddress = UserMaster.Select(p => p.Email).ToArray();
                string[] recipientNames = UserMaster.Select(p => p.PersonName).ToArray();
                int MRFID = Convert.ToInt32(notificationAudit.Reference);
                MRFMaster mrf = _astorWorkDbContext.MRFMaster.Include(m => m.Materials).Where(m => m.ID == MRFID).FirstOrDefault();

                string MRFNo = mrf.MRFNo;
                string duration = (mrf.CreatedDate.Subtract(mrf.UpdatedDate).ToString(@"dd\:hh").Replace(":", " days, ") + " hours").Replace("00", "0");
                int noOfMaterials = mrf.Materials.Count;
                PageMaster pageMaster = _astorWorkDbContext.PageMaster.Where(p => p.UrlPath == "materials").FirstOrDefault();
                string fullurl = _astorWorkDbContext.GetPageFullUrl(pageMaster);
                // string strHost = _tenantInfo.Hostname == null ? "localhost:4200" : _tenantInfo.Hostname;
                string link = $"http://{ _hostname + fullurl};mrfNo={MRFNo}".Trim();
                //@"http://" + _hostname + fullurl + ";mrfNo=" + MRFNo;
                string[] notificationParams = new string[] { MRFNo, noOfMaterials.ToString(), link, duration, mrf.OrderDate.ToString("dd-mm-yyyy"), mrf.CreatedDate.ToString("dd-mm-yyyy")  };

                string subject = string.Format("MRF ({0}) Completed.", MRFNo);

                await _emailService.SendBulk(recipientAddress, recipientNames, subject, (int)Enums.NotificationCode.CloseMRF, notificationParams);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public async Task SendJobAssignedEmail(NotificationAudit notificationAudit)
        {
            try
            {
                List<UserMaster> UserMaster = notificationAudit.Recipients.ToList();
                string[] recipientAddress = UserMaster.Select(p => p.Email).ToArray();
                string[] recipientNames = UserMaster.Select(p => p.PersonName).ToArray();

                JobSchedule jobSchedule = await _astorWorkDbContext.JobSchedule
                                                                   .Include(js => js.Trade)
                                                                   .Include(js => js.Material)
                                                                   .ThenInclude(mm =>mm.Project)
                                                                   .Where(js => js.ID == Convert.ToInt32(notificationAudit.Reference))
                                                                   .FirstOrDefaultAsync();

                //Job, Subcon, Material, PlannedEndDate, PlannedStartDate
                DateTimeOffset PlannedStartDate = (DateTimeOffset)jobSchedule.PlannedStartDate;
                DateTimeOffset PlannedEndDate = (DateTimeOffset)jobSchedule.PlannedEndDate;
                string[] notificationParams = new string[] { jobSchedule.Trade.Name, $"{jobSchedule.Material.Block}-{jobSchedule.Material.Level}-{jobSchedule.Material.Zone}-{jobSchedule.Material.MarkingNo}",
                  PlannedStartDate.DateTime.ToString(),PlannedEndDate.DateTime.ToString(), "+" + TimeSpan.FromMinutes(+(jobSchedule.Material.Project.TimeZoneOffset)).ToString() }; 

                string subject = string.Format("New Job assigned ({0})", notificationParams);

                await _emailService.SendBulk(recipientAddress, recipientNames, subject, (int)Enums.NotificationCode.JobAssigned, notificationParams);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public async Task SendJobCompletedEmail(NotificationAudit notificationAudit)
        {
            try
            {
                List<UserMaster> UserMaster = notificationAudit.Recipients.ToList();
                string[] recipientAddress = UserMaster.Select(p => p.Email).ToArray();
                string[] recipientNames = UserMaster.Select(p => p.PersonName).ToArray();

                JobSchedule jobSchedule = await _astorWorkDbContext.JobSchedule
                                                                   .Include(js => js.Trade)
                                                                   .Include(js => js.Subcon)
                                                                   .Include(js => js.Material)
                                                                   .ThenInclude(mm=>mm.Project)
                                                                   .Where(js => js.ID == Convert.ToInt32(notificationAudit.Reference))
                                                                   .FirstOrDefaultAsync();

                DateTimeOffset PlannedStartDate = (DateTimeOffset)jobSchedule.PlannedStartDate;
                DateTimeOffset PlannedEndDate = (DateTimeOffset)jobSchedule.PlannedEndDate;
                DateTimeOffset ActualStartDate = (DateTimeOffset)jobSchedule.ActualStartDate;
                DateTimeOffset ActualEndDate = (DateTimeOffset)jobSchedule.ActualEndDate;

                //Job, Subcon, Material, PlannedEndDate, PlannedStartDate
                string[] notificationParams = new string[] { jobSchedule.Trade.Name, jobSchedule.Subcon.Name,$"{jobSchedule.Material.Block}-{jobSchedule.Material.Level}-{jobSchedule.Material.Zone}-{jobSchedule.Material.MarkingNo}" ,
                                                             PlannedStartDate.DateTime.ToString(), PlannedEndDate.DateTime.ToString(),
                                                             ActualStartDate.DateTime.ToString(), ActualEndDate.DateTime.ToString(), "+" + TimeSpan.FromMinutes(+(jobSchedule.Material.Project.TimeZoneOffset)).ToString()
                                                           };

                string subject = string.Format("Job completed {0}", notificationParams);

                await _emailService.SendBulk(recipientAddress, recipientNames, subject, (int)Enums.NotificationCode.JobCompleted, notificationParams);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public async Task SendJobQCFailedEmail(NotificationAudit notificationAudit)
        {
            try
            {
                List<UserMaster> UserMaster = notificationAudit.Recipients.ToList();
                string[] recipientAddress = UserMaster.Select(p => p.Email).ToArray();
                string[] recipientNames = UserMaster.Select(p => p.PersonName).ToArray();

                JobSchedule jobSchedule = await _astorWorkDbContext.JobSchedule
                                                                   .Include(js => js.Trade)
                                                                   .Include(js => js.Material)
                                                                   .Where(js => js.ID == Convert.ToInt32(notificationAudit.Reference))
                                                                   .FirstOrDefaultAsync();

                //Job, Subcon, Material, PlannedEndDate, PlannedStartDate
                string[] notificationParams = new string[] { jobSchedule.Trade.Name,$"{jobSchedule.Material.Block}-{jobSchedule.Material.Level}-{jobSchedule.Material.Zone}-{jobSchedule.Material.MarkingNo}"
                                                           };

                string subject = string.Format("QC failed for {0} job", notificationParams);

                await _emailService.SendBulk(recipientAddress, recipientNames, subject, (int)Enums.NotificationCode.JobQCFailed, notificationParams);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public async Task SendJobQCPassedEmail(NotificationAudit notificationAudit)
        {
            try
            {
                List<UserMaster> UserMaster = notificationAudit.Recipients.ToList();
                string[] recipientAddress = UserMaster.Select(p => p.Email).ToArray();
                string[] recipientNames = UserMaster.Select(p => p.PersonName).ToArray();

                JobSchedule jobSchedule = await _astorWorkDbContext.JobSchedule
                                                                   .Include(js => js.Trade)
                                                                   .Include(js => js.Material)
                                                                   .Where(js => js.ID == Convert.ToInt32(notificationAudit.Reference))
                                                                   .FirstOrDefaultAsync();

                //Job, Subcon, Material, PlannedEndDate, PlannedStartDate
                string[] notificationParams = new string[] { jobSchedule.Trade.Name,$"{jobSchedule.Material.Block}-{jobSchedule.Material.Level}-{jobSchedule.Material.Zone}-{jobSchedule.Material.MarkingNo}"
                                                           };

                string subject = string.Format("QC Passed QC {0} job", notificationParams);

                await _emailService.SendBulk(recipientAddress, recipientNames, subject, (int)Enums.NotificationCode.JobQCPassed, notificationParams);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public async Task SendJobQCAcceptedEmail(NotificationAudit notificationAudit)
        {
            try
            {
                List<UserMaster> UserMaster = notificationAudit.Recipients.ToList();
                string[] recipientAddress = UserMaster.Select(p => p.Email).ToArray();
                string[] recipientNames = UserMaster.Select(p => p.PersonName).ToArray();

                JobSchedule jobSchedule = await _astorWorkDbContext.JobSchedule
                                                                   .Include(js => js.Trade)
                                                                   .Include(js => js.Material)
                                                                   .Where(js => js.ID == Convert.ToInt32(notificationAudit.Reference))
                                                                   .FirstOrDefaultAsync();

                //Job, Subcon, Material, PlannedEndDate, PlannedStartDate
                string[] notificationParams = new string[] { jobSchedule.Trade.Name,$"{jobSchedule.Material.Block}-{jobSchedule.Material.Level}-{jobSchedule.Material.Zone}-{jobSchedule.Material.MarkingNo}"
                                                           };

                string subject = string.Format("QC Accepted for {0} job", notificationParams);

                await _emailService.SendBulk(recipientAddress, recipientNames, subject, (int)Enums.NotificationCode.JobQCAccepted, notificationParams);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public async Task SendJobQCRejectedEmail(NotificationAudit notificationAudit)
        {
            try
            {
                List<UserMaster> UserMaster = notificationAudit.Recipients.ToList();
                string[] recipientAddress = UserMaster.Select(p => p.Email).ToArray();
                string[] recipientNames = UserMaster.Select(p => p.PersonName).ToArray();

                JobSchedule jobSchedule = await _astorWorkDbContext.JobSchedule
                                                                   .Include(js => js.Trade)
                                                                   .Include(js => js.Material)
                                                                   .Where(js => js.ID == Convert.ToInt32(notificationAudit.Reference))
                                                                   .FirstOrDefaultAsync();

                //Job, Subcon, Material, PlannedEndDate, PlannedStartDate
                string[] notificationParams = new string[] { jobSchedule.Trade.Name,$"{jobSchedule.Material.Block}-{jobSchedule.Material.Level}-{jobSchedule.Material.Zone}-{jobSchedule.Material.MarkingNo}"
                                                           };

                string subject = string.Format("QC Rejected for {0} job", notificationParams);

                await _emailService.SendBulk(recipientAddress, recipientNames, subject, (int)Enums.NotificationCode.JobQCRejected, notificationParams);
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }
        public async Task SendMaterialQCPassedEmail(NotificationAudit notificationAudit)
        {
            try
            {
                List<UserMaster> UserMaster = notificationAudit.Recipients.ToList();
                string[] recipientAddress = UserMaster.Select(p => p.Email).ToArray();
                string[] recipientNames = UserMaster.Select(p => p.PersonName).ToArray();

                MaterialStageAudit materialStageAudit = await _astorWorkDbContext.MaterialStageAudit
                                                    .Include(ma => ma.Material)
                    .ThenInclude(mm => mm.MaterialType)
                    .Where(ma => ma.ID == Convert.ToInt32(notificationAudit.Reference)).FirstOrDefaultAsync();
                string[] notificationParams = new string[] { $"{materialStageAudit.Material.Block}-{materialStageAudit.Material.Level}-{materialStageAudit.Material.Zone}-{materialStageAudit.Material.MaterialType.Name}" };

                string subject = string.Format("QC Passed ({0})", notificationParams);

                await _emailService.SendBulk(recipientAddress, recipientNames, subject, (int)Enums.NotificationCode.MaterialQCPassed, notificationParams);
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public async Task SendMaterialQCFailedEmail(NotificationAudit notificationAudit)
        {
            try
            {
                List<UserMaster> UserMaster = notificationAudit.Recipients.ToList();
                string[] recipientAddress = UserMaster.Select(p => p.Email).ToArray();
                string[] recipientNames = UserMaster.Select(p => p.PersonName).ToArray();

                MaterialStageAudit materialStageAudit = await _astorWorkDbContext.MaterialStageAudit
                    .Include(ma => ma.Material)
                    .ThenInclude(mm => mm.MaterialType)
                    .Where(ma => ma.ID == Convert.ToInt32(notificationAudit.Reference)).FirstOrDefaultAsync();
                string[] notificationParams = new string[] { $"{materialStageAudit.Material.Block}-{materialStageAudit.Material.Level}-{materialStageAudit.Material.Zone}-{materialStageAudit.Material.MaterialType.Name}" };

                string subject = string.Format("QC failed ({0})", notificationParams);

                await _emailService.SendBulk(recipientAddress, recipientNames, subject, (int)Enums.NotificationCode.MaterialQCFailed, notificationParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task SendModuleReceivedEmail(NotificationAudit notificationAudit)
        {
            try
            {
                List<UserMaster> UserMaster = notificationAudit.Recipients.ToList();
                string[] recipientAddress = UserMaster.Select(p => p.Email).ToArray();
                string[] recipientNames = UserMaster.Select(p => p.PersonName).ToArray();

                MaterialMaster materials = await _astorWorkDbContext.MaterialMaster
                    .Include(mm => mm.MaterialType)
                    .Where(ma => ma.MRF.ID == Convert.ToInt32(notificationAudit.Reference)).FirstOrDefaultAsync();
                string[] notificationParams = new string[] { $"{materials.Block}-{materials.Level}-{materials.Zone}-{materials.MaterialType.Name}" };

                string subject = string.Format("Module Received ({0})", notificationParams);

                await _emailService.SendBulk(recipientAddress, recipientNames, subject, (int)Enums.NotificationCode.ModuleReceived, notificationParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

using astorWorkBackgroundService.Models;
using astorWorkDAO;
using astorWorkShared.MultiTenancy;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using DinkToPdf.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public async Task SendNewMRFEmail(NotificationAudit notificationAudit)
        {
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
                string[] notificationParams = new string[] { MRFNo, noOfMaterials.ToString(), link };

                string subject = string.Format("New MRF {0} Created", MRFNo);

                await _emailService.SendBulk(recipientAddress, recipientNames, subject, 0, notificationParams);
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

        public async Task SendDeliveryMaterialsEmail(NotificationAudit notificationAudit, string tenant, IConverter _converter)
        {
            this._converter = _converter;

            try
            {
                string projectOrVendorName = GetProjectOrVendorName(notificationAudit);
                IEnumerable<DeliveryMaterial> deliveryMaterials = await GetDeliveryMaterials(notificationAudit);

                if (deliveryMaterials.Count() > 0)
                {
                    List<string> attachmentPaths = new List<string>();
                    attachmentPaths.Add(CreateAttachment(tenant, projectOrVendorName, deliveryMaterials));

                    await SendEmails(notificationAudit, projectOrVendorName, attachmentPaths);
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public string CreateAttachment(string tenant, string projectOrVendorName, IEnumerable<DeliveryMaterial> deliveryMaterials) {
            string filePath = string.Format("{0}{1}-{2}.pdf", Path.GetTempPath(), tenant, projectOrVendorName);
            string header = "Delayed Delivery Materials (" + DateTime.Now.ToString("dd/MM/yyyy") + ")";
            string subHeader = tenant + " - " + projectOrVendorName;

            return _emailService.CreateDocument(filePath, header, subHeader, AttachmentHandler.CreateTblContent(deliveryMaterials), _converter);
        }

        public async Task SendEmails(NotificationAudit notificationAudit, string projectOrVendorName, List<string> attachmentPaths) {
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

        public async Task<IEnumerable<DeliveryMaterial>> GetDeliveryMaterials(NotificationAudit notificationAudit) {
            UpdateNotificationAuditService updateNotificationAuditService = new UpdateNotificationAuditService(_astorWorkDbContext);
            
            if (notificationAudit.Code == Convert.ToInt32(Enums.NotificationCode.DelayInDelivery))
                return updateNotificationAuditService.GetDelayedDeliveryMaterials(Convert.ToInt32(notificationAudit.Reference), notificationAudit.NotificationTimer.Project != null);
            else
                return updateNotificationAuditService.GetExpectedDeliveryMaterials(Convert.ToInt32(notificationAudit.Reference), notificationAudit.NotificationTimer.Project != null);
        }

        public string GetProjectOrVendorName(NotificationAudit notificationAudit) {
            
            if (notificationAudit.NotificationTimer.Project != null)
            {
                ProjectMaster project = _astorWorkDbContext.ProjectMaster.Where(p => p.ID == Convert.ToInt32(notificationAudit.Reference)).FirstOrDefault();
                return project.Name;
            }
            else
            {
                VendorMaster vendor = _astorWorkDbContext.VendorMaster.Where(p => p.ID == Convert.ToInt32(notificationAudit.Reference)).FirstOrDefault();
                return vendor.Name;
            }
        }

        public async Task SendQCFailEmail(NotificationAudit notificationAudit)
        {
            try
            {
                var UserMaster = notificationAudit.Recipients;
                string[] recipientAddress = UserMaster.Select(p => p.Email).ToArray();
                string[] recipientNames = UserMaster.Select(p => p.PersonName).ToArray();
                int QCDefectID = Convert.ToInt32(notificationAudit.Reference);
                string subject = string.Empty;
                string msg = string.Empty;
                int defectCount = 0;
                MaterialQCDefect qcDefect = _astorWorkDbContext.MaterialQCDefect.Include(c => c.QCCase).ThenInclude(s => s.StageAudit).ThenInclude(m => m.MaterialMaster).Where(d => d.ID == QCDefectID).FirstOrDefault();



                string markingNo = qcDefect.QCCase.StageAudit.MaterialMaster.MarkingNo;
                string caseName = qcDefect.QCCase.CaseName;

                PageMaster pageMaster = _astorWorkDbContext.PageMaster.Where(p => p.UrlPath == "qc-defects/:id").FirstOrDefault();
                string fullurl = _astorWorkDbContext.GetPageFullUrl(pageMaster);
                //string strHost = _tenantInfo.Hostname == null ? "localhost:4200" : _tenantInfo.Hostname;
                string link = $"http://{ _hostname + fullurl};caseName={caseName}".Trim();
                link = link.Replace(":id", "0");
                if (notificationAudit.Code == Convert.ToInt32(Enums.NotificationCode.QCFailed))
                {
                    defectCount = _astorWorkDbContext.MaterialQCDefect.Where(d => d.QCCaseId == qcDefect.QCCaseId && d.IsOpen).Count();
                    if (defectCount == 1)
                    {
                        string createdBy = qcDefect.QCCase.CreatedBy.PersonName;
                        string createdDate = qcDefect.QCCase.CreatedDate.ToString("dd/MM/yyyy");
                        msg = string.Format("A new QC Case, {0} was created by {1} on {2}, with a new Defect, for the Material {3}", caseName, createdBy, createdDate, markingNo);
                        subject = string.Format("New QCCase {0} Created, for {1} material ", caseName, markingNo);
                    }
                    else
                    {
                        string createdBy = qcDefect.CreatedBy.PersonName;
                        string createdDate = qcDefect.CreatedDate.ToString("dd/MM/yyyy");
                        string remarks = qcDefect.Remarks;
                        msg = string.Format("A new defect, {0} was created by {1} on {2}, for the Material {3}, with the following remarks: {4}", caseName, createdBy, createdDate, markingNo, remarks);
                        subject = string.Format("New Defect Created under case {0}, for {1} material ", caseName, markingNo);
                    }
                }
                else
                {
                    defectCount = _astorWorkDbContext.MaterialQCDefect.Where(d => d.QCCaseId == qcDefect.QCCaseId && d.IsOpen).Count();
                    if (defectCount == 0)
                    {
                        msg = "Case " + caseName + " closed, as all the defect under the case is closed, for the materail " + markingNo;
                        subject = string.Format("Case {0}, for {1} material is closed ", caseName, markingNo);
                    }
                    else
                    {
                        msg = "Defect under the Case " + caseName + ", for the materail " + markingNo + " is closed";
                        subject = string.Format("Defect under case {0} for {1} material is closed ", caseName, markingNo);
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
                string[] notificationParams = new string[] { MRFNo, noOfMaterials.ToString(), link, duration };

                string subject = string.Format("MRF {0} Closed", MRFNo);

                await _emailService.SendBulk(recipientAddress, recipientNames, subject, (int)Enums.NotificationCode.CloseMRF, notificationParams);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
    }
}

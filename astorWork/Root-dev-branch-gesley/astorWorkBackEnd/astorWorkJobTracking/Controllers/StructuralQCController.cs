using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static astorWorkShared.Utilities.Enums;
using astorWorkDAO;
using astorWorkShared.Services;
using astorWorkJobTracking.Models;
using astorWorkShared.Utilities;
using Microsoft.AspNetCore.Http;
using astorWorkJobTracking.Common;
namespace astorWorkJobTracking.Controllers
{
    [Produces("application/json")]
    [Route("structural-checklist")]
    public class StructuralQCController : CommonController
    {
        protected astorWorkDbContext _context;
        protected IAstorWorkBlobStorage _blobStorage;

        public StructuralQCController(astorWorkDbContext context, IAstorWorkBlobStorage blobStorage):base(context)
        {
            _context = context;
            _blobStorage = blobStorage;
        }
        [HttpPost("submit")]
        public async Task<int> SubmitChecklist([FromQuery]int checklist_id, [FromQuery]int material_id, [FromQuery]int route_to_id, [FromBody] SignedChecklist signedChecklist)
        {
            ChecklistAudit checklistAudit = new ChecklistAudit();
            try
            {
                var materialstageaudit = _context.MaterialStageAudit
                    .Include(msa => msa.Stage)
                    .Where(msa => msa.MaterialID == material_id).OrderByDescending(msa => msa.Stage.Order).FirstOrDefault();
                checklistAudit = await _context.ChecklistAudit.Include(c => c.MaterialStageAudit)
                                                                           .ThenInclude(ma => ma.Material)
                                                                           .Where(ca => ca.ChecklistID == checklist_id
                                                                                     && ca.MaterialStageAudit == materialstageaudit)
                                                                           .OrderByDescending(ca => ca.CreatedDate)
                                                                           .FirstOrDefaultAsync();

                QCStatus status = await GetChecklistQCStatus(checklistAudit, signedChecklist.ChecklistItems, material_id, route_to_id);
                UserMaster routeTo = await GetRouteTo(route_to_id);    // Route to subcon if QC failed, if not, route to selected RTO

                checklistAudit = await AddChecklistAudit(checklist_id, material_id, status, signedChecklist.Signature, routeTo);
                await UpdateChecklistItemAudit(checklistAudit, signedChecklist.ChecklistItems);
                await UpdateMaterialStatus(checklistAudit);
                await _context.SaveChangesAsync();

                MaterialMaster materialMaster = checklistAudit.MaterialStageAudit.Material;
                List<UserMaster> receipients = new List<UserMaster>();
                int notificationCode = 0;

                if (checklistAudit.Status == QCStatus.QC_routed_to_RTO )
                {
                    receipients = await GetRTO(checklistAudit);
                    notificationCode = (int)NotificationCode.MaterialQCPassed;
                }
                else if(checklistAudit.Status== QCStatus.QC_failed_by_Maincon)
                {
                    receipients = await GetVendorProductionOfficer(materialMaster);
                    receipients.Add(getVendorProjectManager(checklistAudit));
                    notificationCode = (int)NotificationCode.MaterialQCFailed;
                }
                else if (checklistAudit.Status == QCStatus.QC_passed_by_Maincon)
                {
                    receipients = await GetVendorProductionOfficer(materialMaster);
                    receipients.Add(getVendorProjectManager(checklistAudit));
                    notificationCode = (int)NotificationCode.MaterialQCPassed;
                }
                if (notificationCode > 0)
                    await UpdateNotificationAudit(receipients, notificationCode, 0, checklistAudit.MaterialStageAuditID.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Convert.ToInt32(checklistAudit.Status);
        }
       
        [HttpPost("accept-reject")]
        public async Task<int> AcceptRejectChecklist([FromQuery]int checklist_id, [FromQuery]int material_id, [FromQuery] int status_id, [FromBody] SignedChecklist signedChecklist)
        {
            ChecklistAudit checklistAudit = new ChecklistAudit();

            try
            {
                QCStatus status = (QCStatus)status_id;
                //UserMaster routeTo = await GetRouteTo(job_schedule_id, status, 0);  // Route to subcon
                //for error solving 
                string signature = signedChecklist.Signature;
                checklistAudit = await AddChecklistAudit(checklist_id, material_id, status, signature);

                await UpdateChecklistItemAudit(checklistAudit, signedChecklist.ChecklistItems);
                await UpdateMaterialStatus(checklistAudit);
                await _context.SaveChangesAsync();

                int notificationCode = 0;
                List<UserMaster> receipients = new List<UserMaster>();
                // receipients = await GetMainContractor(checklistAudit.JobSchedule); 

                receipients = await GetMainContractorStrcuturalQC(checklistAudit);
                if (checklistAudit.Status == QCStatus.QC_accepted_by_RTO)
                    notificationCode = (int)NotificationCode.JobQCAccepted;
                else if (checklistAudit.Status == QCStatus.QC_rejected_by_RTO)
                    notificationCode = (int)NotificationCode.JobQCRejected;

                if (notificationCode > 0)
                    await UpdateNotificationAudit(receipients, notificationCode, 0, checklistAudit.MaterialStageAuditID.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Convert.ToInt32(checklistAudit.Status);
        }

        private  UserMaster getVendorProjectManager(ChecklistAudit checklistAudit)
        {
            return  _context.UserMaster
                .Include(u => u.Organisation)
                .Include(u => u.Role)
                .Where(u => u.Organisation != null && u.Organisation.ID == checklistAudit.MaterialStageAudit.Material.OrganisationID
                && u.RoleID == (int)RoleType.VendorProjectManager).FirstOrDefault();
        }

        protected async Task<List<UserMaster>> GetMainContractorStrcuturalQC(ChecklistAudit checklistAudit)
        {
            return await _context.UserMaster
                .Include(u => u.Organisation)
                .Include(u => u.Role)
                .Where(u => u.ProjectID==checklistAudit.MaterialStageAudit.Material.ProjectID
                && u.RoleID == (int)RoleType.MainConQC).Distinct().ToListAsync();
        }
        protected async Task<List<UserMaster>> GetVendorProductionOfficer(MaterialMaster materialMaster)
        {
            return await _context.UserMaster
                .Include(u => u.Organisation)
                .Include(u => u.Role)
                .Where(u => u.Organisation != null && u.Organisation.ID == materialMaster.OrganisationID
                && u.RoleID == (int)RoleType.VendorProductionOfficer).ToListAsync();
        }

        private async Task UpdateMaterialStatus(ChecklistAudit checklistAudit)
        {
            MaterialStageAudit materialStageAudit = checklistAudit.MaterialStageAudit;
            JobStatus qcstatus;
            switch(checklistAudit.Status)
            {
                case QCStatus.QC_failed_by_Maincon:
                     materialStageAudit.QCStatus = JobStatus.QC_failed_by_Maincon;
                     break;

                case QCStatus.QC_passed_by_Maincon:
                    qcstatus = await GetMaterialStatus(materialStageAudit, checklistAudit.Status, checklistAudit.ChecklistID);
                    materialStageAudit.QCStatus = qcstatus;

                    break;
                case QCStatus.QC_rectified_by_Subcon:
                    materialStageAudit.QCStatus = JobStatus.QC_rectified_by_Subcon;
                    break;
                case QCStatus.QC_rejected_by_RTO:
                    materialStageAudit.QCStatus = JobStatus.QC_rejected_by_RTO;
                    break;
                case QCStatus.QC_accepted_by_RTO:
                    qcstatus = await GetMaterialStatus(materialStageAudit, checklistAudit.Status, checklistAudit.ChecklistID);
                    materialStageAudit.QCStatus = qcstatus;

                    break;
                case QCStatus.QC_routed_to_RTO:
                    materialStageAudit.QCStatus = JobStatus.QC_routed_to_RTO;
                    break;
                default:
                    break;
            }
            _context.Update(materialStageAudit);

        }

        private async Task<JobStatus> GetMaterialStatus(MaterialStageAudit materialStageAudit, QCStatus status, int checklistID)
        {
            var lastchecklist = (await _context.ChecklistMaster.Where(chk => chk.MaterialStageID == materialStageAudit.StageID && chk.IsActive==true).OrderByDescending(chk => chk.Sequence).FirstOrDefaultAsync())?.ID;

            if(lastchecklist!=null&&lastchecklist==checklistID)
                return JobStatus.All_QC_passed;  // Status to be completed if it is last checklist in the Trade to be accepted or passed
            else
            {
                if (status == QCStatus.QC_passed_by_Maincon)
                    return JobStatus.QC_passed_by_Maincon; // QC passed but not routed to RTO, QC proccess will proceed to next checklist in trade
                else
                    return JobStatus.QC_accepted_by_RTO;   // QC accepted and QC proccess will proceed to next checklist in trade
            }
        }

        private async Task<ChecklistAudit> AddChecklistAudit(int checklistID, int material_id, QCStatus status, string signature, UserMaster routeTo = null)
        {
            ChecklistAudit checklistAudit = new ChecklistAudit();
            try
            {

                MaterialStageAudit materialStageAudit = await _context.MaterialStageAudit
                                                                      .Include(ma=>ma.Material)
                                                                      .Where(msa => msa.MaterialID == material_id)
                                                                      .OrderByDescending(msa => msa.Stage.Order)
                                                                      .FirstOrDefaultAsync();

                checklistAudit = new ChecklistAudit
                {
                    JobScheduleID = null,
                    MaterialStageAuditID = materialStageAudit?.ID,
                    MaterialStageAudit = (materialStageAudit != null) ? materialStageAudit : null,
                    ChecklistID = checklistID,
                    Checklist = await _context.ChecklistMaster.Where(clm => clm.ID == checklistID).FirstOrDefaultAsync(),
                    CreatedDate = DateTimeOffset.Now,
                    CreatedBy = await _context.GetUserFromHttpContext(HttpContext),
                    Status = status,
                    RouteTo = routeTo,
                    SignatureURL = !string.IsNullOrEmpty(signature) ? await _blobStorage.UploadSignature(signature) : null
                };

                _context.Add(checklistAudit);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return checklistAudit;
        }
        private async Task<UserMaster> GetRouteTo(int route_to_id)
        {
            try
            {
                if (route_to_id != 0)
                    return await _context.UserMaster.Where(u => u.ID == route_to_id).FirstOrDefaultAsync();
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        private async Task<QCStatus> GetChecklistQCStatus(ChecklistAudit checklistAudit, List<ChecklistItem> checklistItems, int material_id,int route_to_id)
        {
            QCStatus status = (checklistAudit != null) ? checklistAudit.Status : QCStatus.Pending_QC;

            if (checklistItems != null)
            {
                int qcPassOrNA = 0;

                foreach (ChecklistItem checklistItem in checklistItems)
                {
                    // If any item on the checklist is failed, checklist fails QC
                    if (status != QCStatus.QC_failed_by_Maincon && checklistItem.StatusCode == (int)ChecklistItemStatus.Fail)
                        return QCStatus.QC_failed_by_Maincon;

                    // Check that all items on the checklist is Pass or NA
                    if (checklistItem.StatusCode == (int)ChecklistItemStatus.Pass || checklistItem.StatusCode == (int)ChecklistItemStatus.NA)
                        qcPassOrNA++;
                }

                // If Trade is configured for RTO routing, route to RTO, if not, pass QC
               if (checklistItems.Count == qcPassOrNA)
                   status=route_to_id!=0? QCStatus.QC_routed_to_RTO: QCStatus.QC_passed_by_Maincon;

                //    return await IsRoutedToRTO(material_id) ? QCStatus.QC_routed_to_RTO : QCStatus.QC_passed_by_Maincon;

            }

            return status;
        }

        private async Task<bool> IsRoutedToRTO(int material_id)
        {
            MaterialTypeMaster materialType = (await _context.MaterialMaster
                                               .Include(t => t.MaterialType)
                                               .Where(p => p.ID == material_id)
                                               .FirstOrDefaultAsync()).MaterialType;


            return !string.IsNullOrEmpty(materialType.RouteTo);
        }

        private async Task UpdateChecklistItemAudit(ChecklistAudit checklistAudit, List<ChecklistItem> checklistItems)
        {
            try
            {
                foreach (ChecklistItem checklistItem in checklistItems)
                {
                    ChecklistItemMaster checklistItemMaster = await _context.ChecklistItemMaster.Where(cli => cli.ID == checklistItem.ID).FirstOrDefaultAsync();
                    ChecklistItemAudit checklistItemAudit = await _context.ChecklistItemAudit.Where(clia => clia.ChecklistItemID == checklistItem.ID
                                                                                                         && clia.ChecklistAudit == checklistAudit)
                                                                                             .FirstOrDefaultAsync();
                    if (checklistItemAudit == null)
                    {
                        checklistItemAudit = new ChecklistItemAudit
                        {
                            ChecklistAuditID = checklistAudit.ID,
                            ChecklistAudit = checklistAudit,
                            ChecklistItemID = checklistItem.ID,
                            ChecklistItem = checklistItemMaster,
                            CreatedDate = DateTimeOffset.Now,
                            CreatedBy = checklistAudit.CreatedBy,
                            Status = (ChecklistItemStatus)checklistItem.StatusCode
                        };

                        await _context.AddAsync(checklistItemAudit);
                    }
                    else
                    {
                        checklistItemAudit.ChecklistAuditID = checklistAudit.ID;    // Update the checklist audit to the latest one
                        checklistItemAudit.ChecklistAudit = checklistAudit;
                        checklistItemAudit.Status = (ChecklistItemStatus)checklistItem.StatusCode;
                        checklistItemAudit.CreatedDate = DateTimeOffset.Now;
                        checklistItemAudit.CreatedBy = checklistAudit.CreatedBy;

                        _context.Update(checklistItemAudit);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       

    }
}
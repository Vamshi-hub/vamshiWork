using astorWorkDAO;
using astorWorkJobTracking.Common;
using astorWorkJobTracking.Models;
using astorWorkShared.GlobalExceptions;
using astorWorkShared.GlobalResponse;
using astorWorkShared.Models;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static astorWorkShared.Utilities.Enums;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace astorWorkJobTracking.Controllers
{
    [Produces("application/json")]
    [Route("checklist-audit")]
    public class ChecklistAuditController : CommonController
    {
        protected astorWorkDbContext _context;
        protected IAstorWorkBlobStorage _blobStorage;

        public ChecklistAuditController(astorWorkDbContext context, IAstorWorkBlobStorage blobStorage):base(context)
        {
            _context = context;
            _blobStorage = blobStorage;
        }

        // POST checklist-audit/submit?checklist_id=1&job_schedule_id=12&route_to=2
        [HttpPost("submit")]
        public async Task<int> SubmitChecklist([FromQuery]int checklist_id, [FromQuery]int job_schedule_id, [FromQuery]int route_to_id, [FromBody] SignedChecklist signedChecklist)
        {
            ChecklistAudit checklistAudit = new ChecklistAudit();
            try
            {
                checklistAudit = await _context.ChecklistAudit.Include(c => c.JobSchedule)
                                                                           .ThenInclude(js => js.Trade)
                                                                           .Where(ca => ca.ChecklistID == checklist_id
                                                                                     && ca.JobScheduleID == job_schedule_id)
                                                                           .OrderByDescending(ca => ca.CreatedDate)
                                                                           .FirstOrDefaultAsync();

                QCStatus status = await GetChecklistQCStatus(checklistAudit, signedChecklist.ChecklistItems, job_schedule_id);
                UserMaster routeTo = await GetRouteTo(job_schedule_id, status, route_to_id);    // Route to subcon if QC failed, if not, route to selected RTO

                checklistAudit = await AddChecklistAudit(checklist_id, job_schedule_id, status, signedChecklist.Signature, routeTo);
                await UpdateChecklistItemAudit(checklistAudit, signedChecklist.ChecklistItems);
                await UpdateJobStatus(checklistAudit);
                await _context.SaveChangesAsync();

                JobSchedule jobSchedule = checklistAudit.JobSchedule;
                List<UserMaster> receipients = new List<UserMaster>();
                int notificationCode = 0;

                if (checklistAudit.Status == QCStatus.QC_routed_to_RTO || checklistAudit.Status == QCStatus.QC_passed_by_Maincon)
                {
                    receipients = await GetRTO(checklistAudit);
                    notificationCode = (int)NotificationCode.JobQCPassed;
                }
                else if (checklistAudit.Status == QCStatus.QC_failed_by_Maincon)
                {
                    receipients = await GetSubContractor(jobSchedule);
                    notificationCode = (int)NotificationCode.JobQCFailed;
                }

                // receipients.Add(await _context.GetUserFromHttpContext(HttpContext));
                if (notificationCode > 0)
                    await UpdateNotificationAudit(receipients, notificationCode, 0, jobSchedule.ID.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Convert.ToInt32(checklistAudit.Status);
        }

        // POST checklist-audit/route-to-subcon?checklist_id=1&job_schedule_id=12
        [HttpPost("route-to-subcon")]
        public async Task<int> RouteToSubcon([FromQuery]int checklist_id, [FromQuery]int job_schedule_id)
        {
            // Only QC Rejected jobs will trigger this API

            ChecklistAudit checklistAudit = new ChecklistAudit();
            try
            {
                QCStatus status = QCStatus.QC_failed_by_Maincon;  // Will change to QC failed when Route to Subcon is activated
                UserMaster routeTo = await GetRouteTo(job_schedule_id, status, 0);  // Route to subcon
                string signature = null;
                checklistAudit = await AddChecklistAudit(checklist_id, job_schedule_id, QCStatus.QC_failed_by_Maincon, signature);
                await UpdateJobStatus(checklistAudit);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Convert.ToInt32(checklistAudit.Status);
        }

        // POST checklist-audit/accept-reject?checklist_id=1&job_schedule_id=12&status_id
        [HttpPost("accept-reject")]
        public async Task<int> AcceptRejectChecklist([FromQuery]int checklist_id, [FromQuery]int job_schedule_id, [FromQuery] int status_id, [FromBody] SignedChecklist signedChecklist)
        {
            ChecklistAudit checklistAudit = new ChecklistAudit();

            try
            {
                QCStatus status = (QCStatus)status_id;
                //UserMaster routeTo = await GetRouteTo(job_schedule_id, status, 0);  // Route to subcon
                //for error solving 
                string signature = signedChecklist.Signature;
                checklistAudit = await AddChecklistAudit(checklist_id, job_schedule_id, status, signature);

                await UpdateChecklistItemAudit(checklistAudit, signedChecklist.ChecklistItems);
                await UpdateJobStatus(checklistAudit);
                await _context.SaveChangesAsync();
                int notificationCode = 0;
                List<UserMaster> receipients = new List<UserMaster>();
               
                receipients = await GetMainContractorQC(checklistAudit);
                if (checklistAudit.Status == QCStatus.QC_accepted_by_RTO)
                    notificationCode = (int)NotificationCode.JobQCAccepted;
                else if (checklistAudit.Status == QCStatus.QC_rejected_by_RTO)
                    notificationCode = (int)NotificationCode.JobQCRejected;

                // receipients = await GetMainContractor(checklistAudit.JobSchedule); 
                //else if (checklistAudit.Status == QCStatus.QC_rectified_by_Subcon)
                //    notificationCode = (int)NotificationCode.QCRectified;              
                // receipients.Add(await _context.GetUserFromHttpContext(HttpContext));

                if (notificationCode > 0)
                    await UpdateNotificationAudit(receipients, notificationCode, 0, checklistAudit.JobSchedule.ID.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Convert.ToInt32(checklistAudit.Status);
        }

        private async Task<UserMaster> GetRouteTo(int jobScheduleID, QCStatus status, int route_to_id)
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

        private async Task<QCStatus> GetChecklistQCStatus(ChecklistAudit checklistAudit, List<ChecklistItem> checklistItems, int jobScheduleID)
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
                    return await IsRTO(jobScheduleID) ? QCStatus.QC_routed_to_RTO : QCStatus.QC_passed_by_Maincon;
            }

            return status;
        }

        private async Task<bool> IsRTO(int jobScheduleID)
        {
            TradeMaster trade = (await _context.JobSchedule
                                               .Include(t => t.Trade)
                                               .Where(p => p.ID == jobScheduleID)
                                               .FirstOrDefaultAsync())
                                .Trade;

            return !string.IsNullOrEmpty(trade.RouteTo);
        }

        private async Task<ChecklistAudit> AddChecklistAudit(int checklistID, int job_schedule_id, QCStatus status, string signature, UserMaster routeTo = null)
        {
            ChecklistAudit checklistAudit = new ChecklistAudit();
            try
            {
                JobSchedule jobSchedule = await _context.JobSchedule.Where(js => js.ID == job_schedule_id).FirstOrDefaultAsync();
                MaterialStageAudit materialStageAudit = await _context.MaterialStageAudit
                                                                      .Include(m=>m.Material)
                                                                      .Where(msa => msa.MaterialID == jobSchedule.MaterialID)
                                                                                         .OrderByDescending(msa => msa.Stage.Order)
                                                                                         .FirstOrDefaultAsync();

                checklistAudit = new ChecklistAudit
                {
                    JobScheduleID = jobSchedule.ID,
                    JobSchedule = jobSchedule,
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

        private async Task UpdateJobStatus(ChecklistAudit checklistAudit)
        {
            JobAudit jobAudit = new JobAudit();
            JobSchedule jobSchedule = checklistAudit.JobSchedule;
            JobStatus jobStatus;

            switch (checklistAudit.Status)
            {
                case QCStatus.QC_failed_by_Maincon:
                    jobSchedule.Status = JobStatus.QC_failed_by_Maincon;
                    jobAudit.Status = JobStatus.QC_failed_by_Maincon;
                    break;
                case QCStatus.QC_passed_by_Maincon:
                    jobStatus = await GetJobStatus(jobSchedule, checklistAudit.Status, checklistAudit.ChecklistID);
                    jobSchedule.Status = jobStatus;
                    jobAudit.Status = jobStatus;
                    break;
                case QCStatus.QC_rectified_by_Subcon:
                    jobSchedule.Status = JobStatus.QC_rectified_by_Subcon;
                    jobAudit.Status = JobStatus.QC_rectified_by_Subcon;
                    break;
                case QCStatus.QC_rejected_by_RTO:
                    jobSchedule.Status = JobStatus.QC_rejected_by_RTO;
                    jobAudit.Status = JobStatus.QC_rejected_by_RTO;
                    break;
                case QCStatus.QC_accepted_by_RTO:
                    jobStatus = await GetJobStatus(jobSchedule, checklistAudit.Status, checklistAudit.ChecklistID);
                    jobSchedule.Status = jobStatus;
                    jobAudit.Status = jobStatus;
                    break;
                case QCStatus.QC_routed_to_RTO:
                    jobSchedule.Status = JobStatus.QC_routed_to_RTO;
                    jobAudit.Status = JobStatus.QC_routed_to_RTO;
                    break;
                default:
                    break;
            }

            jobAudit.JobSchedule = jobSchedule;
            jobAudit.CreatedBy = checklistAudit.CreatedBy;
            jobAudit.CreatedDate = DateTime.UtcNow;

            await _context.AddAsync(jobAudit);
            _context.Update(jobSchedule);
        }

        private async Task<JobStatus> GetJobStatus(JobSchedule jobSchedule, QCStatus qcStatus, int checklistID)
        {
            var lastChecklist = (await _context.ChecklistMaster.Where(chk => chk.TradeID == jobSchedule.TradeID &&chk.IsActive==true).OrderByDescending(chk => chk.Sequence)?.FirstOrDefaultAsync())?.ID;
            if (lastChecklist != null && lastChecklist == checklistID)
                return JobStatus.All_QC_passed;  // Status to be completed if it is last checklist in the Trade to be accepted or passed
            else
            {
                if (qcStatus == QCStatus.QC_passed_by_Maincon)
                    return JobStatus.All_QC_passed; // QC passed but not routed to RTO, QC proccess will proceed to next checklist in trade
                else
                    return JobStatus.QC_accepted_by_RTO;   // QC accepted and QC proccess will proceed to next checklist in trade
            }
        }
    }
}

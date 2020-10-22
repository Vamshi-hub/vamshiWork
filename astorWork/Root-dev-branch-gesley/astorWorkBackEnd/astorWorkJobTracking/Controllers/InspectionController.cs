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

namespace astorWorkJobTracking.Controllers
{
    [Produces("application/json")]
    [Route("inspection")]
    public class InspectionController : CommonController
    {
        protected astorWorkDbContext _context;
        protected IAstorWorkBlobStorage _blobStorage;

        public InspectionController(astorWorkDbContext context, IAstorWorkBlobStorage blobStorage) : base(context)
        {
            _context = context;
            _blobStorage = blobStorage;
        }
        [HttpGet("{job_id}")]
        public async Task<List<RTOInspectionForm>> GetInspectionsByJob([FromRoute]int job_id)
        {
            List<RTOInspectionForm> lstRtoInspection = null;
            TradeMaster trade = null;
            ChecklistMaster checklist = null;
            List<ChecklistItemAssociation> ChecklistItemAssociation = null;
            try
            {
                lstRtoInspection = _context.ChecklistAudit
                    .Include(ca => ca.Checklist)
                    .Include(ca => ca.JobSchedule)
                    .ThenInclude(js => js.Material)
                    .Include(ca => ca.JobSchedule.Trade)
                    .Include(ca => ca.InspectionInfo).ThenInclude(ins => ins.Signatures)
                    .Where(ca => ca.JobScheduleID == job_id
                    && ca.Checklist.Type == Enums.ChecklistType.Architectural_RTO)
                    .Select(ca => new RTOInspectionForm()
                    {
                        Discipline = ca.JobSchedule.Trade.Type == 0 ? "Architectural" : "M&E",
                        TradeOrActivity = ca.Checklist.Name,
                        Date = ca.InspectionInfo.InspectionDate,
                        Time = ca.InspectionInfo.InspectionTime,
                        DrawingNo = ca.InspectionInfo.ReferenceNo,
                        ReferenceNo = ca.ID.ToString(),
                        ModuleName = ca.InspectionInfo.RoomType,
                        JobScheduleID = ca.JobScheduleID != null ? Convert.ToInt32(ca.JobScheduleID) : 0,
                        Signatures = ca.InspectionInfo.Signatures.Select(sgn => new Signatures()
                        {
                            URL = GetSignatureURL(sgn.Signature),
                            UserID = Convert.ToInt32(sgn.UserID),
                        }).ToList()
                    }).ToList();
                var rtoinspection = new RTOInspectionForm();
                if (lstRtoInspection == null || lstRtoInspection.Count == 0)
                {
                    trade = _context.JobSchedule.Include(js => js.Trade).Where(js => js.ID == job_id).FirstOrDefault().Trade;
                    checklist = _context.ChecklistMaster.Where(chk => chk.TradeID == trade.ID && chk.Type == Enums.ChecklistType.Architectural_RTO &&chk.IsActive==true).FirstOrDefault();
                    rtoinspection.Discipline = trade.Type == 1 ? "Architectural" : "M&E";
                    rtoinspection.TradeOrActivity = checklist.Name;
                    ChecklistItemAssociation = _context.ChecklistItemAssociation.Include(ch => ch.ChecklistItem).Include(ch => ch.Checklist).Where(ch => ch.ChecklistID == checklist.ID && ch.IsActive == true).ToList();
                    rtoinspection.ChecklistItemName = ChecklistItemAssociation.Select(cha => cha.ChecklistItem.Name).ToList();
                }
                else
                {
                    rtoinspection.Discipline = lstRtoInspection.FirstOrDefault().Discipline;
                    rtoinspection.TradeOrActivity = lstRtoInspection.FirstOrDefault().TradeOrActivity;
                }

                rtoinspection.Date = null;
                rtoinspection.Time = null;
                rtoinspection.DrawingNo = string.Empty;
                rtoinspection.ReferenceNo = string.Empty;
                rtoinspection.ModuleName = string.Empty;
                rtoinspection.JobScheduleID = job_id;
                rtoinspection.Signatures = null;

                lstRtoInspection.Add(rtoinspection);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstRtoInspection;
        }

        /// <summary>
        /// Used to Post inspection details from Mobile RTO InspectionForm Page
        /// </summary>
        /// <param name="InspectionFormData"></param>
        /// <param name="jobschedule_id"></param>
        /// <param name="checklist_id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task SubmitIncepctionForm([FromBody] InspectionFormDetails InspectionFormDetails, [FromQuery] int jobschedule_id, [FromQuery] int checklist_id)
        {
            InspectionAudit inspectionAudit = new InspectionAudit();
            try
            {
                inspectionAudit.ReferenceNo = InspectionFormDetails.DrawingNo;
                inspectionAudit.RoomType = InspectionFormDetails.ModuleName;
                inspectionAudit.InspectionTime = InspectionFormDetails.Time;
                inspectionAudit.InspectionDate = DateTime.UtcNow;
                inspectionAudit.Remarks = InspectionFormDetails.Remarks;
                await _context.InspectionAudit.AddAsync(inspectionAudit);
                await _context.SaveChangesAsync();
                List<InspectionSignatureAssociation> lstInspectionSignature = new List<InspectionSignatureAssociation>();
                foreach (var signatureAssociation in InspectionFormDetails.Signatures)
                {
                    InspectionSignatureAssociation inspectionSignature = new InspectionSignatureAssociation();
                    inspectionSignature.InspectionAuditID = inspectionAudit.ID;
                    inspectionSignature.UserID = signatureAssociation.UserID;
                    inspectionSignature.Signature = !string.IsNullOrEmpty(signatureAssociation.ImageBase64) ? await _blobStorage.UploadSignature(signatureAssociation.ImageBase64) : null;
                    lstInspectionSignature.Add(inspectionSignature);
                }
                await _context.InspectionSignatureAssociation.AddRangeAsync(lstInspectionSignature);
                await _context.SaveChangesAsync();
                ChecklistAudit checklistAudit = new ChecklistAudit();
                checklistAudit.InspectionInfoID = inspectionAudit.ID;
                checklistAudit.JobScheduleID = jobschedule_id;
                checklistAudit.ChecklistID = checklist_id;
                if (InspectionFormDetails.StatusCode == 0)
                    checklistAudit.Status = Enums.QCStatus.QC_accepted_by_RTO;
                else if (InspectionFormDetails.StatusCode == 1)
                    checklistAudit.Status = Enums.QCStatus.QC_rejected_by_RTO;
                checklistAudit.CreatedDate = DateTimeOffset.UtcNow;
                checklistAudit.CreatedBy = await _context.GetUserFromHttpContext(HttpContext);
                await _context.ChecklistAudit.AddAsync(checklistAudit);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private string GetSignatureURL(string signature)
        {
            string token = _blobStorage.GetContainerAccessToken();
            return signature + token;
        }
    }
}

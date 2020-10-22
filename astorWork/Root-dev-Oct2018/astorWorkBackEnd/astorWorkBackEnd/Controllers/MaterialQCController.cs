using astorWorkBackEnd.Common;
using astorWorkBackEnd.Models;
using astorWorkDAO;
using astorWorkShared.GlobalResponse;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace astorWorkBackEnd.Controllers
{
    [Produces("application/json")]
    [Route("qc")]
    public class MaterialQCController : CommonMaterialStageAuditsController
    {
        private IAstorWorkBlobStorage _blobStorage;
        public MaterialQCController(astorWorkDbContext context, IAstorWorkBlobStorage blobStorage) : base(context)
        {
            _blobStorage = blobStorage;
        }

        [HttpPost("defect")]
        public async Task<APIResponse> PostQCDefect([FromQuery] int case_id, [FromQuery] int stage_audit_id, [FromBody] object Remarks)
        {
            if (!ModelState.IsValid)
            {
                return new APIBadRequest();
            }
            try
            {
                JObject json = JObject.Parse(Remarks.ToString());
                string strRemarks = json["remarks"].ToString();
                MaterialQCDefect qcDefect = null;

                var user = _context.GetUserFromHttpContext(HttpContext);
                if (user == null)
                {
                    return new DbRecordNotFound("User not found");
                }

                if (case_id == 0)
                {
                    var countQCCase = await _context.MaterialQCCase.CountAsync();
                    string runningNum = (countQCCase + 1).ToString("D4");
                    string strCaseName = $"QC-{DateTime.Now.Year}-{DateTime.Now.ToString("MMM")}-{runningNum}";

                    qcDefect = new MaterialQCDefect
                    {
                        Remarks = strRemarks,
                        IsOpen = true,
                        CreatedById = user.ID,
                        CreatedDate = DateTime.UtcNow
                    };

                    qcDefect.QCCase = new MaterialQCCase()
                    {
                        CaseName = strCaseName,
                        CreatedBy = user,
                        CreatedDate = DateTime.UtcNow,
                        StageAuditId = stage_audit_id
                    };
                }
                else
                {
                    qcDefect = new MaterialQCDefect
                    {
                        QCCaseId = case_id,
                        Remarks = strRemarks,
                        IsOpen = true,
                        CreatedBy = user,
                        CreatedDate = DateTime.UtcNow
                    };
                    qcDefect.QCCase = _context.MaterialQCCase.Where(c => c.ID == case_id).FirstOrDefault();
                    if (qcDefect.QCCase != null)
                    {
                        qcDefect.QCCase.UpdatedBy = user;
                        qcDefect.QCCase.UpdatedDate = DateTime.UtcNow;
                    }
                }
                await _context.MaterialQCDefect.AddAsync(qcDefect);

                #region Notification code

                var stageAudit = await _context.MaterialStageAudit.Include(msa => msa.MaterialMaster)
                    .ThenInclude(mm => mm.MRF)
                    .Where(msa => msa.ID == stage_audit_id).FirstAsync();

                var userIds = _context.UserMRFAssociation
                    .Where(uma => uma.MRFID == stageAudit.MaterialMaster.MRF.ID)
                    .Select(u => u.UserID).ToArray();

                var lstUsers = _context.UserMaster.Where(u => userIds.Contains(u.ID)).ToList();
                lstUsers.Add(user);
                await UpdateNotificationAudit(lstUsers, Convert.ToInt32(Enums.NotificationCode.QCFailed), Convert.ToInt32(Enums.NotificationType.Email), qcDefect.ID.ToString());

                #endregion

                await _context.SaveChangesAsync();

                var Defects_id = new { id = qcDefect.ID.ToString() };

                var countCases = await _context.MaterialQCCase
                    .Where(qc => qc.StageAuditId == stage_audit_id).CountAsync();
                Console.WriteLine($"Stage audit <{stage_audit_id}> has {countCases} QC cases");
                return new APIResponse(0, Defects_id);
            }
            catch (Exception ex)
            {                
                Console.WriteLine(ex.Message);
                Console.Write(ex.StackTrace);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                    Console.Write(ex.InnerException.StackTrace);
                }
                return new APIResponse(504, null, ex.Message);
            }
        }


        [HttpPost("photo")]
        public async Task<APIResponse> UploadQCPhoto([FromQuery] int defect_id, [FromBody] QCPhoto qcPhoto)
        {
            if (!ModelState.IsValid)
            {
                return new APIBadRequest();
            }

            var userClaim = HttpContext.User.Claims.FirstOrDefault(cl => cl.Type.Equals(ClaimTypes.NameIdentifier));
            if (userClaim == null)
            {
                return new DbRecordNotFound("User Claim not found");
            }

            var user = _context.UserMaster.Find(int.Parse(userClaim.Value));
            if (user == null)
            {
                return new DbRecordNotFound("User not found");
            }

            var MaterialQCDefect = _context.MaterialQCDefect.Where(p => p.ID == defect_id).Include(p => p.Photos).FirstOrDefault();

            if (MaterialQCDefect == null)
            {
                return new DbRecordNotFound("MaterialQCDefect not found");
            }

            try
            {
                byte[] data = System.Convert.FromBase64String(qcPhoto.ImageContent);
                MemoryStream ms = new MemoryStream(data);
                var fileName = Guid.NewGuid() + ".jpg";
                var success = await _blobStorage.UploadFile(AppConfiguration.GetQCContainerName(), fileName, ms);
                string Fileurl = _blobStorage.GetContainerHost() + fileName.ToString();
                MaterialQCDefect.Photos.Add(new MaterialQCPhotos() { URL = Fileurl, Remarks = qcPhoto.Remarks, CreatedBy = user, CreatedDate = DateTime.UtcNow, IsOpen = !qcPhoto.Closed });
                await _context.SaveChangesAsync();
                var photoID = new { id = MaterialQCDefect.Photos.LastOrDefault().ID };
                return new APIResponse(0, photoID);
            }
            catch (Exception ex)
            {
                return new ExternalServiceFail(ex.Message);
            }

        }


        [HttpPut("defect")]
        public async Task<APIResponse> PutQCDefect([FromQuery] int defect_id, [FromBody] QCDefect qcDefect)
        {
            if (!ModelState.IsValid)
            {
                return new APIBadRequest();
            }

            var userClaim = HttpContext.User.Claims.FirstOrDefault(cl => cl.Type.Equals(ClaimTypes.NameIdentifier));
            if (userClaim == null)
            {
                return new DbRecordNotFound("User Claim not found");
            }

            var user = _context.UserMaster.Find(int.Parse(userClaim.Value));
            if (user == null)
            {
                return new DbRecordNotFound("User not found");
            }

            var MaterialQCDefect = _context.MaterialQCDefect.Where(p => p.ID == defect_id).Include(p => p.Photos).FirstOrDefault();

            if (MaterialQCDefect == null)
            {
                return new DbRecordNotFound("MaterialQCDefect not found");
            }

            try
            {
                var defect = _context.MaterialQCDefect.Include(d => d.QCCase)
                    .FirstOrDefault(d => d.ID == defect_id);
                defect.IsOpen = !qcDefect.Closed;
                defect.Remarks = qcDefect.Remarks;
                defect.UpdatedDate = DateTime.UtcNow;
                defect.UpdatedBy = user;
                defect.QCCase.UpdatedBy = user;
                defect.QCCase.UpdatedDate = DateTime.UtcNow;
                _context.Entry(defect).State = EntityState.Modified;

                #region Notification code

                if (!defect.IsOpen)
                {
                    var mrfID = _context.MaterialQCCase.Include(c => c.StageAudit).ThenInclude(s => s.MaterialMaster).ThenInclude(m => m.MRF).Where(c => c.ID == defect.QCCaseId).FirstOrDefault().StageAudit.MaterialMaster.MRF.ID;
                    var userIds = _context.UserMRFAssociation.Where(uma => uma.MRFID == mrfID).Select(u => u.UserID).ToArray();
                    var lstUsers = _context.UserMaster.Where(u => userIds.Contains(u.ID)).ToList();
                    lstUsers.Add(user);
                    await UpdateNotificationAudit(lstUsers, Convert.ToInt32(Enums.NotificationCode.QCRectified), Convert.ToInt32(Enums.NotificationType.Email), defect.ID.ToString());
                }

                #endregion

                await _context.SaveChangesAsync();


                var openDefects = _context.MaterialQCDefect.Where(D => D.QCCaseId == defect.QCCaseId && D.IsOpen == true);
                if (openDefects == null || openDefects.Count() < 1)
                {
                    int stageAuditId = defect.QCCase.StageAuditId;

                    return new APIResponse(0, stageAuditId, "Defect and Case Closed");
                }
                return new APIResponse(0, defect.ID);
            }
            catch (Exception ex)
            {
                return new ExternalServiceFail(ex.Message);
            }
        }


        [HttpGet("photo")]
        public APIResponse GetQCPhotos([FromQuery] int defect_id)
        {
            if (!ModelState.IsValid)
            {
                return new APIBadRequest();
            }

            List<MaterialQCPhotos> LstQCPhotos = _context.MaterialQCDefect?.Where(m => m.ID == defect_id)?.Include(p => p.Photos).Include(q => q.CreatedBy).FirstOrDefault()?.Photos;

            if (LstQCPhotos == null)
            {
                return new DbRecordNotFound("No Photo(s) found for this defect ");
            }
            else
            {
                string tocken = _blobStorage.GetContainerAcessTocken();
                var qcPhotos = LstQCPhotos.Select(q => new { q.ID, q.Remarks, URL = q.URL + tocken, q.CreatedBy?.UserName, q.CreatedDate, q.IsOpen }).ToList();
                return new APIResponse(0, qcPhotos);
            }

        }

        [HttpGet("case")]
        public APIResponse GetQCCases([FromQuery] int project_id, [FromQuery] int? stage_audit_id)
        {
            if (!ModelState.IsValid)
            {
                return new APIBadRequest();
            }

            var listQCCases = _context.MaterialQCCase
                .Include(mqc => mqc.CreatedBy)
                .Include(mqc => mqc.UpdatedBy)
                .Include(mqc => mqc.Defects)
                .ThenInclude(d => d.CreatedBy)
                .Include(mqc => mqc.Defects)
                .ThenInclude(d => d.UpdatedBy)
                .Include(mqc => mqc.StageAudit)
                .ThenInclude(sa => sa.Stage)
                .Include(mqc => mqc.StageAudit)
                .ThenInclude(sa => sa.MaterialMaster)
                .Where(mqc => stage_audit_id.HasValue ? mqc.StageAuditId == stage_audit_id : mqc.StageAudit.MaterialMaster.ProjectId == project_id)
                .OrderByDescending(q => q.CreatedDate)
                .ToList();

            if (listQCCases == null)
            {
                return new DbRecordNotFound("MaterialStage No Case(s) found");
            }
            else
            {
                // Sort defects by updated date
                /*
                listQCCases.ForEach(q => q.Defects = q.Defects.OrderByDescending(d => d.UpdatedDate).ThenByDescending(d => d.CreatedDate).ToList());
                */

                var result = new List<dynamic>();
                foreach (var qcCase in listQCCases)
                {

                    var isOpen = qcCase.Defects.Any(d => d.IsOpen);
                    var durationSpan = isOpen ? DateTimeOffset.Now.Subtract(qcCase.CreatedDate) : qcCase.UpdatedDate?.Subtract(qcCase.CreatedDate);

                    var duration = durationSpan.HasValue ? durationSpan.Value.TotalHours >= 1 ? (durationSpan.Value.ToString(@"dd\:hh").Replace(":", " days, ") + " hours").Replace("00", "0") : (Math.Round(durationSpan.Value.TotalMinutes) + " minutes") : "N.A.";

                    var countOpenDefects = qcCase.Defects.Where(d => d.IsOpen).Count();
                    var countClosedDefects = qcCase.Defects.Where(d => !d.IsOpen).Count();

                    var progress = qcCase.Defects.Count == 0 ? 0 : countClosedDefects * 100 / qcCase.Defects.Count;

                    result.Add(new
                    {
                        qcCase.ID,
                        qcCase.CaseName,
                        CreatedBy = qcCase.CreatedBy.PersonName,
                        qcCase.CreatedDate,
                        qcCase.UpdatedDate,
                        UpdatedBy = qcCase.UpdatedBy == null ? null : qcCase.UpdatedBy.PersonName,
                        isOpen,
                        countOpenDefects,
                        countClosedDefects,
                        duration,
                        progress,
                        stageName = qcCase.StageAudit.Stage.Name,
                        markingNo = qcCase.StageAudit.MaterialMaster.MarkingNo,
                        qcCase.StageAuditId
                    });
                }
                /*
                var qcCases = listQCCases.Select(q => new
                {
                    q.ID,
                    q.CaseName,
                    CreatedBy = q.CreatedBy.UserName,
                    q.CreatedDate,
                    updatedDate = q.Defects != null ? q.Defects.FirstOrDefault().UpdatedDate != null ? q.Defects.FirstOrDefault().UpdatedDate : q.Defects?.FirstOrDefault()?.CreatedDate : null,
                    updatedBy = q.Defects != null ? q.Defects.FirstOrDefault().UpdatedBy != null ? q.Defects.FirstOrDefault().UpdatedBy.UserName : q.Defects?.FirstOrDefault()?.CreatedBy.UserName : null,
                    isOpen = q.Defects?.Any(d => d.IsOpen),
                    countOpenDefects = q.Defects?.Where(d => d.IsOpen).Count(),
                    countClosedDefects = q.Defects?.Where(d => !d.IsOpen).Count(),

                    duration = (q.CreatedDate.Subtract(q.Defects?.Any(d => d.IsOpen) == true ? DateTimeOffset.UtcNow : q.Defects?.LastOrDefault()?.UpdatedDate != null ?
                                q.Defects.LastOrDefault().UpdatedDate.GetValueOrDefault() : q.Defects.LastOrDefault().CreatedDate).ToString(@"dd\:hh").Replace(":", " days, ") + " hours").Replace("00", "0"),

                    progress = (q.Defects?.Where(d => d.IsOpen).Count() + q.Defects?.Where(d => !d.IsOpen).Count()) > 0 ? (q.Defects?.Where(d => !d.IsOpen).Count() / (q.Defects?.Where(d => d.IsOpen).Count() + q.Defects?.Where(d => !d.IsOpen).Count())) * 100 : 0,
                    stageName = q.StageAudit.Stage.Name,
                    markingNo = q.StageAudit.MaterialMaster.MarkingNo,
                    q.StageAuditId

                }).ToList();
                */

                if (stage_audit_id.HasValue) {
                    var countCases = _context.MaterialQCCase
                        .Where(qc => qc.StageAuditId == stage_audit_id.Value).Count();
                    Console.WriteLine($"Stage audit <{stage_audit_id.Value}> has {countCases} QC cases");
                }
                return new APIResponse(0, result);
            }
        }

        //returns List of defects for single case
        [HttpGet("defect")]
        public APIResponse GetDefects([FromQuery] int case_id)
        {
            if (!ModelState.IsValid)
            {
                return new APIBadRequest();
            }

            List<MaterialQCDefect> lstMaterialQCDefect = _context.MaterialQCDefect
                .Include(p => p.Photos)
                .ThenInclude(photo => photo.CreatedBy)
                .Include(p => p.CreatedBy)
                .Include(p => p.UpdatedBy)
                .Where(m => m.QCCaseId == case_id)
                .ToList();

            if (lstMaterialQCDefect == null)
            {
                return new DbRecordNotFound("MaterialStage No Defect(s) found");
            }
            else
            {
                var qcCases = lstMaterialQCDefect
                    .Select(q => new
                    {
                        q.ID,
                        q.Remarks,
                        CreatedBy = q.CreatedBy.PersonName,
                        q.CreatedDate,
                        /*
                        updatedDate = q.Photos.Any() ? q.Photos.OrderBy(p => p.CreatedDate).LastOrDefault().CreatedDate : q.CreatedDate,
                        updatedBy = q.Photos.Any() ? q.Photos.OrderBy(p => p.CreatedDate).LastOrDefault().CreatedBy.UserName : q.CreatedBy.UserName,
                        */
                        q.UpdatedDate,
                        UpdatedBy = q.UpdatedBy != null?q.UpdatedBy?.PersonName:null,
                        isOpen = q.IsOpen,
                        countPhotos = q.Photos.Count()
                    }).ToList();
                return new APIResponse(0, qcCases);
            }
        }

        //returns List of defects for single case
        [HttpGet("defects-By-StageAuditID")]
        public APIResponse GetListofDefectsByStageAuditID([FromQuery] int stage_audit_id)
        {
            if (!ModelState.IsValid)
            {
                return new APIBadRequest();
            }

            List<MaterialQCDefect> lstMaterialQCDefect = _context.MaterialQCDefect.Include(d => d.QCCase)
                .Include(p => p.Photos)
                .ThenInclude(photo => photo.CreatedBy)
                .Include(p => p.CreatedBy)
                .Include(p => p.UpdatedBy)
                .Where(m => m.QCCase.StageAuditId == stage_audit_id)
                .ToList();

            if (lstMaterialQCDefect == null)
            {
                return new DbRecordNotFound("MaterialStage No Defect(s) found");
            }
            else
            {
                var qcCases = lstMaterialQCDefect.Select(q => new
                {
                    q.ID,
                    q.Remarks,
                    CreatedBy = q.CreatedBy.PersonName,
                    q.CreatedDate,
                    updatedDate = q.UpdatedDate,
                    updatedBy = q.UpdatedBy == null ? null : q.UpdatedBy.PersonName,
                    isOpen = q.IsOpen,
                    countPhotos = q.Photos.Count(),
                    caseID = q.QCCaseId
                }).ToList();
                return new APIResponse(0, qcCases);
            }
        }
    }
}
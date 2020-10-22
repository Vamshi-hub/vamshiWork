using astorWorkMaterialTracking.Common;
using astorWorkMaterialTracking.Models;
using astorWorkDAO;
using astorWorkShared.GlobalExceptions;
using astorWorkShared.GlobalResponse;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using astorWorkShared.Models;

namespace astorWorkMaterialTracking.Controllers
{
    [Produces("application/json")]
    [Route("qc")]
    public class MaterialQCController : CommonQCController
    {
        public MaterialQCController(astorWorkDbContext context, IAstorWorkBlobStorage blobStorage) : base(context)
        {
            _blobStorage = blobStorage;
        }

        [HttpGet("case")]
        public async Task<List<QCCase>> ListQCCases([FromQuery] int? project_id, [FromQuery] int? material_id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            List<MaterialQCCase> qcCases = await _context.MaterialQCCase
                                                         .Include(q => q.MaterialMaster)
                                                         .ThenInclude(m => m.StageAudits)
                                                         .ThenInclude(sa => sa.Stage)
                                                         .Include(q=>q.MaterialMaster.Organisation)
                                                         .Include(q => q.Defects)
                                                         .Include(q => q.CreatedBy)
                                                         .ToListAsync();  

            if (material_id.HasValue)
                qcCases = qcCases.Where(qc => qc.MaterialMasterId == material_id.Value).ToList();
            else if(project_id.HasValue)
                qcCases = qcCases.Where(qc => qc.MaterialMaster?.ProjectID == project_id.Value).ToList();

            if (qcCases.Count() == 0)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "No QC Case(s) found");
            
            List<QCCase> QCCases = new List<QCCase>();
            foreach (MaterialQCCase qcCase in qcCases)
                QCCases.Add(CreateQCCase(qcCase));

            return QCCases;
        }

        [HttpPost("defect")]
        public async Task<int> CreateQCDefect([FromQuery] int case_id, [FromQuery] int material_id, [FromQuery] int organisation_id, [FromBody] dynamic Remarks)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            try
            {
                MaterialMaster material = await GetMaterialMaster(material_id);
                if (material == null && material_id > 0)
                    throw new GenericException(ErrorMessages.DbRecordNotFound, "No material(s) found");

                UserMaster user = await _context.GetUserFromHttpContext(HttpContext);
                if (user == null)
                    throw new GenericException(ErrorMessages.DbRecordNotFound, "User not found");

                string remarks = Remarks["remarks"];
                MaterialQCDefect qcDefect = await CreateNewQCDefect(case_id, remarks, material, organisation_id, user);
                if (qcDefect == null)
                    throw new GenericException(ErrorMessages.DbRecordNotFound, "No QC defect(s) found");

                await _context.MaterialQCDefect.AddAsync(qcDefect);

                #region Notification code

                if (material == null)
                    material = await GetMaterialMaster(qcDefect.QCCase.MaterialMasterId);

              
                await _context.SaveChangesAsync();
                List<UserMaster> receipients = GetReceipients(material, user);

                if (qcDefect.ID > 0)
                    await UpdateNotificationAudit(receipients, Convert.ToInt32(Enums.NotificationCode.QCFailed), Convert.ToInt32(Enums.NotificationType.Email), qcDefect.ID.ToString());

                #endregion

               
                return qcDefect.ID;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ExceptionUtility.GetExceptionDetails(ex));
                throw new GenericException(504, ex.Message);
            }
        }

        

        [HttpPost("photo")]
        public async Task<int> UploadQCPhoto([FromQuery] int defect_id, [FromBody] QCPhoto qcPhoto)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            UserMaster user = await _context.GetUserFromHttpContext(HttpContext);

            if (user == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "User not found");

            MaterialQCDefect defect = await _context.MaterialQCDefect
                .Where(p => p.ID == defect_id)
                .Include(p => p.Photos)
                .FirstOrDefaultAsync();

            if (defect == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "No QC defect(s) found");

            try
            {
                return await UploadQCPhoto(qcPhoto, user, defect);
            }
            catch (Exception ex)
            {
                throw new GenericException(ErrorMessages.ExternalServiceFail, ex.Message);
            }
        }

        [HttpPut("defect")]
        public async Task<int> PutQCDefect([FromQuery] int defect_id, [FromQuery] int organisation_id,[FromQuery] int status_code, [FromBody] QCDefect qcDefect)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            MaterialQCDefect defect = await GetMaterialQCDefect(defect_id);

            if (defect == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "QC defect not found");

            try
            {
                UserMaster user = await _context.GetUserFromHttpContext(HttpContext);
                if (user == null)
                    throw new GenericException(ErrorMessages.DbRecordNotFound, "User not found");

                UpdateQCDefect(defect, qcDefect, organisation_id, user, status_code);

                #region Notification code

                if (defect.Status == Enums.QCStatus.QC_passed_by_Maincon || defect.Status==Enums.QCStatus.QC_rectified_by_Subcon)
                {
                    List<UserMaster> receipients = defect.QCCase.MaterialMaster.MRF.UserMRFAssociations
                        .Select(uma => uma.User).ToList();
                    //receipients.Add(user); only send to respective person no need of current user

                    await UpdateNotificationAudit(receipients, Convert.ToInt32(Enums.NotificationCode.QCRectified), Convert.ToInt32(Enums.NotificationType.Email), defect.ID.ToString());
                }

                #endregion

                await _context.SaveChangesAsync();

                
                List<MaterialQCDefect> openDefects = await GetOpenQCDefects(defect.QCCaseID);

                // 0 means all defects are closed and case is closed
                if (openDefects.Count() == 0)
                    return 0;
                else
                    return defect.ID;
            }
            catch (Exception ex)
            {
                throw new GenericException(ErrorMessages.ExternalServiceFail, ex.Message);
            }
        }

        [HttpGet("photo")]
        public IEnumerable<QCPhoto> GetQCPhotos([FromQuery] int defect_id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            List<MaterialQCPhotos> materialQCPhotos = GetMaterialQCPhotos(defect_id);

            if (materialQCPhotos == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "No Photo(s) found for this defect!");
           
            string token = _blobStorage.GetContainerAccessToken();
            return CreateQCPhotos(materialQCPhotos, token);
        }

        //returns List of defects for single case
        [HttpGet("defect")]
        public async Task<List<QCDefect>> GetDefects([FromQuery] int case_id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            List<MaterialQCDefect> materialQCDefects = await GetMaterialQCDefects(case_id);

            if (materialQCDefects == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "No Defect(s) found");
            
            return CreateQCDefect(materialQCDefects);
        }
    }
}
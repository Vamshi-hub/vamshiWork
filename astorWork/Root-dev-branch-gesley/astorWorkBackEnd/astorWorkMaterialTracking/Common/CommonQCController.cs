using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using astorWorkMaterialTracking.Models;
using astorWorkDAO;
using Microsoft.EntityFrameworkCore;
using System.Data;
using astorWorkShared.Services;
using astorWorkShared.GlobalModels;
using System.Text;
using System.IO;
using DinkToPdf.Contracts;
using astorWorkShared.Utilities;
using astorWorkShared.Models;
using static astorWorkShared.Utilities.Enums;

namespace astorWorkMaterialTracking.Common
{
    public class CommonQCController : CommonController
    {
        protected IAstorWorkEmail _emailService;
        protected TenantInfo _tenant;
        protected IConverter _converter;
        protected IAstorWorkBlobStorage _blobStorage;

        public CommonQCController(astorWorkDbContext context) : base(context)
        {
        }

        protected IEnumerable<MaterialQCCase> GetMaterialQCCases(int projectId)
        {
            return _context.MaterialQCCase
                           .Include(q => q.MaterialMaster)
                           .ThenInclude(m => m.StageAudits)
                           .ThenInclude(sa => sa.Stage)
                           .Include(q => q.Defects)
                           .Include(q => q.CreatedBy)
                           .Where(q => q.MaterialMaster.ProjectID == projectId);
        }

        protected QCCase CreateQCCase(MaterialQCCase qcCase)
        {
            bool isOpen = qcCase.Defects.Any(d => d.Status < Enums.QCStatus.QC_passed_by_Maincon);
            string duration = CalculateDuration(isOpen, qcCase);
            int countOpenDefects = qcCase.Defects.Where(d => d.Status < Enums.QCStatus.QC_passed_by_Maincon).Count();
            int countClosedDefects = qcCase.Defects.Where(d => d.Status == Enums.QCStatus.QC_passed_by_Maincon).Count();
            double progress = qcCase.Defects.Count == 0 ? 0 : countClosedDefects * 100 / qcCase.Defects.Count;

            return new QCCase
            {
                ID = qcCase.ID,
                CaseName = qcCase.CaseName,
                MarkingNo = qcCase.MaterialMaster.MarkingNo,
                StageName = (qcCase.MaterialMaster.StageAudits.Count == 0) ? "Unp" : qcCase.MaterialMaster.StageAudits.OrderByDescending(sa => sa.Stage.Order).FirstOrDefault().Stage.Name,
                CreatedBy = qcCase.CreatedBy.PersonName,
                CreatedDate = qcCase.CreatedDate,
                UpdatedBy = qcCase.UpdatedBy == null ? null : qcCase.UpdatedBy.PersonName,
                UpdatedDate = qcCase.UpdatedDate,
                IsOpen = isOpen,
                CountOpenDefects = countOpenDefects,
                CountClosedDefects = countClosedDefects,
                Progress = progress,
                Duration = duration
                //StageAuditId = qcCase.MaterialMaster.StageAudits.OrderByDescending(sa => sa.Stage.Order).FirstOrDefault().ID
            };
        }

        protected string CalculateDuration(bool isOpen, MaterialQCCase qcCase)
        {
            TimeSpan? durationSpan = isOpen ? DateTimeOffset.Now.Subtract(qcCase.CreatedDate) : qcCase.UpdatedDate?.Subtract(qcCase.CreatedDate);
            return durationSpan.HasValue ? durationSpan.Value.TotalHours >= 1 ? (durationSpan.Value.ToString(@"dd\:hh").Replace(":", " days, ") + " hours").Replace("00", "0") : (Math.Round(durationSpan.Value.TotalMinutes) + " minutes") : "N.A.";
        }

        protected async Task<MaterialQCCase> UpdateQCCaseForNewDefect(int caseId, UserMaster user)
        {
            MaterialQCCase qcCase = await _context.MaterialQCCase.FindAsync(caseId);

            if (qcCase == null)
                return null;

            qcCase.UpdatedBy = user;
            qcCase.UpdatedDate = DateTimeOffset.UtcNow;

            return qcCase;
        }

        protected async Task<MaterialQCDefect> CreateNewQCDefect(int caseId, string remarks, MaterialMaster material, int organisationID, UserMaster user)
        {
            MaterialQCDefect qcDefect = null;

            if (caseId == 0)
            {
                qcDefect = CreateQCDefect(remarks, organisationID, user);
                qcDefect.QCCase = CreateQCCase(material, user);
            }
            else
            {
                MaterialQCCase qcCase = await UpdateQCCaseForNewDefect(caseId, user);
                if (qcCase == null)
                    return null;

                qcDefect = CreateQCDefect(remarks, organisationID, user, caseId);
                qcDefect.QCCase = qcCase;
            }

            return qcDefect;
        }

        protected MaterialQCDefect CreateQCDefect(string remarks, int organisationID, UserMaster user, int caseID = 0)
        {
            MaterialQCDefect qcDefect = new MaterialQCDefect();
            OrganisationMaster organisation = _context.OrganisationMaster.Where(o => o.ID == organisationID).FirstOrDefault();

            if (caseID > 0)
                qcDefect.QCCaseID = caseID;
            qcDefect.Organisation = organisation;
            qcDefect.OrganisationID = organisation.ID;
            qcDefect.Remarks = remarks;
            qcDefect.Status = Enums.QCStatus.QC_failed_by_Maincon;
            qcDefect.CreatedBy = user;
            qcDefect.CreatedDate = DateTimeOffset.UtcNow;

            return qcDefect;
        }

        protected MaterialQCCase CreateQCCase(MaterialMaster material, UserMaster createdBy)
        {
            int countQCCase = material.QCCases == null ? 0 : material.QCCases.Count;
            string runningNum = (countQCCase + 1).ToString("D3");
            string caseName = $"QC-{DateTime.Now.Year}-{DateTime.Now.ToString("MMM")}-{material.ID}-{runningNum}";
            return new MaterialQCCase()
            {
                MaterialMasterId = material.ID,
                CaseName = caseName,
                CreatedBy = createdBy,
                CreatedDate = DateTimeOffset.UtcNow
            };
        }

        protected List<UserMaster> GetReceipients(MaterialMaster material, UserMaster user)
        {
            List<UserMaster> receipients = material.MRF.UserMRFAssociations.Select(uma => uma.User).Distinct().ToList();
            receipients.Add(user);//only send to respective person no need of current user
            return receipients;
        }

        protected async Task<MaterialMaster> GetMaterialMaster(int id)
        {
            if (id == 0)
                return null;

            return await _context.MaterialMaster
                    .Include(mm => mm.QCCases)
                    .ThenInclude(c => c.Defects)
                    .Include(mm => mm.MRF)
                    .ThenInclude(mrf => mrf.UserMRFAssociations)
                    .ThenInclude(uma => uma.User)
                    .Where(mm => mm.ID == id)
                    .FirstOrDefaultAsync();
        }

        protected async Task<int> UploadQCPhoto(QCPhoto qcPhoto, UserMaster user, MaterialQCDefect defect)
        {
            byte[] data = Convert.FromBase64String(qcPhoto.ImageContent);
            MemoryStream ms = new MemoryStream(data);
            var fileName = Guid.NewGuid() + ".jpg";
            bool success = await _blobStorage.UploadFile(AppConfiguration.GetQCContainerName(), fileName, ms);
            string Fileurl = _blobStorage.GetContainerHost() + fileName.ToString();
            var photo = new MaterialQCPhotos()
            {
                URL = Fileurl,
                Remarks = qcPhoto.Remarks,
                CreatedBy = user,
                CreatedDate = DateTime.UtcNow,
                Status = defect.Status,
            };
            defect.Photos.Add(photo);
            await _context.SaveChangesAsync();
            return photo.ID;
        }

        protected async Task<MaterialQCDefect> GetMaterialQCDefect(int defectID)
        {
            return await _context.MaterialQCDefect
                .Where(d => d.ID == defectID)
                .Include(d => d.QCCase)
                .ThenInclude(c => c.MaterialMaster)
                .ThenInclude(m => m.MRF)
                .ThenInclude(mrf => mrf.UserMRFAssociations)
                .ThenInclude(uma => uma.User)
                .Include(d => d.Photos)
                .FirstOrDefaultAsync();
        }

        protected async Task<List<MaterialQCDefect>> GetOpenQCDefects(int qcCaseID)
        {
            return await _context.MaterialQCDefect
                           .Where(d => d.QCCaseID == qcCaseID && d.Status < Enums.QCStatus.QC_passed_by_Maincon)
                           .ToListAsync();
        }

        protected void UpdateQCDefect(MaterialQCDefect defect, QCDefect qcDefect, int organisationID, UserMaster user, int statusCode)
        {
            OrganisationMaster organisation = _context.OrganisationMaster.Where(o => o.ID == organisationID).FirstOrDefault();
            //defect.Status = !qcDefect.Closed?Enums.QCStatus.QC_failed_by_Maincon:Enums.QCStatus.QC_passed_by_Maincon;
            //defect.Status = (QCStatus)qcDefect.StatusCode;  //if  status through QCDefect . 

            defect.Status = (QCStatus)statusCode;  //if status through fromquery
            defect.Remarks = qcDefect.Remarks;
            defect.Organisation = organisation;
            defect.OrganisationID = organisation.ID;
            defect.UpdatedDate = DateTime.UtcNow;
            defect.UpdatedBy = user;
            defect.QCCase.UpdatedBy = user;
            defect.QCCase.UpdatedDate = DateTime.UtcNow;
            _context.Entry(defect).State = EntityState.Modified;
        }

        protected List<MaterialQCPhotos> GetMaterialQCPhotos(int defectID)
        {
            return _context.MaterialQCDefect?
                           .Where(m => m.ID == defectID)?
                           .Include(p => p.Photos)
                           .Include(q => q.CreatedBy)
                           .Include(q=>q.UpdatedBy)
                           .FirstOrDefault()?
                           .Photos
                           .ToList();
        }

        protected List<QCPhoto> CreateQCPhotos(List<MaterialQCPhotos> materialQCPhotos, string token)
        {
            return materialQCPhotos.Select(q => new QCPhoto
            {
                ID = q.ID,
                Remarks = q.Remarks,
                URL = q.URL + token,
                CreatedBy = q.CreatedBy?.UserName,
                CreatedDate = q.CreatedDate,
                IsOpen = (int)q.Status <(int)QCStatus.QC_rectified_by_Subcon
            }).ToList();
        }

        protected async Task<List<MaterialQCDefect>> GetMaterialQCDefects(int caseID)
        {
            return await _context.MaterialQCDefect
                           .Include(d => d.Photos)
                           .ThenInclude(photo => photo.CreatedBy)
                           .Include(d => d.CreatedBy)
                           .Include(d => d.UpdatedBy)
                           .Include(d => d.Organisation)
                           .Where(m => m.QCCaseID == caseID)
                           .ToListAsync();
        }

        protected List<QCDefect> CreateQCDefect(List<MaterialQCDefect> materialQCDefects)
        {
            return materialQCDefects.Select(d => new QCDefect
            {
                ID = d.ID,
                Remarks = d.Remarks,
                CreatedBy = d.CreatedBy?.PersonName,
                CreatedDate = d.CreatedDate,
                UpdatedDate = d.UpdatedDate,
                UpdatedBy = d.UpdatedBy?.PersonName,
                StatusCode = (int)d.Status,
                IsOpen = d.Status < Enums.QCStatus.QC_passed_by_Maincon && d.Status != Enums.QCStatus.QC_rectified_by_Subcon,
                IsClosed = d.Status.Equals(Enums.QCStatus.QC_accepted_by_RTO),
                IsRectified = d.Status.Equals(Enums.QCStatus.QC_rectified_by_Subcon),
                CountPhotos = d.Photos.Count(),
                SubconName = d.Organisation?.Name,
                SelectedSubconID = d.OrganisationID
            }).ToList();
        }
    }
}
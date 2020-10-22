using astorWorkBackEnd.Models;
using astorWorkDAO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace astorWorkBackEnd.Common
{
    [Produces("application/json")]
    [Route("api/CommonMaterialMasters")]
    public class CommonDashboardController : CommonController
    {
        protected CommonDashboardController(astorWorkDbContext context) : base(context)
        {
        }
        #region Common Methods

        protected IEnumerable<MaterialOverallProgress> CreateOverallProgress(int projectID, IEnumerable<MaterialStageAudit> materialStageAudits)
        {
            // Get Total Material Count by Level
            int TotalMaterialsCount = _context.MaterialMaster
                                .Where(m => m.Project.ID == projectID).Count();


            var progressByDate = materialStageAudits
                .Where(m => m.Stage.ID == installedStageId && m.MaterialMaster.Project.ID == projectID)
                .GroupBy(m => m.CreatedDate.Date)
                .OrderBy(m => m.Key.Date)
                .Select(m => new MaterialOverallProgress
                {
                    Date = m.Key.Date,
                    Progress = Math.Round((((double)m.Count()) / ((double)TotalMaterialsCount) * 100), 2)
                });

            List<MaterialOverallProgress> materialOverallProgresses = new List<MaterialOverallProgress>();
            MaterialOverallProgress materialOverallProgress = new MaterialOverallProgress();
            materialOverallProgress.Date = _context.ProjectMaster.Where(p => p.ID == projectID).FirstOrDefault().EstimatedStartDate;
            materialOverallProgress.Progress = 0;
            materialOverallProgresses.Add(materialOverallProgress);

            double progress = 0;
            foreach (MaterialOverallProgress mop in progressByDate)
            {
                progress += mop.Progress;
                mop.Progress = progress;
                materialOverallProgresses.Add(mop);
            }

            return materialOverallProgresses;
        }

        protected IEnumerable<MaterialInProgress> CreateInProgress(int projectID, string block, IEnumerable<MaterialStageAudit> materialStageAudits)
        {
            // List<MaterialInProgress> materialOverallProgressList = new List<MaterialInProgress>();

            // Installed stage will always be the last stage
            var installedStage = _context.MaterialStageMaster.OrderBy(msm => msm.Order).LastOrDefault();

            // Get Installed Material Count by Level
            var installedMaterials = materialStageAudits
                                    .Where(m => m.Stage == installedStage && m.MaterialMaster.Project.ID == projectID && m.MaterialMaster.Block == block)
                                    .GroupBy(m => m.MaterialMaster.Level)
                                    .OrderBy(m => m.Key.PadLeft(3))
                                .Select(m => new { Level = m.Key, Count = m.Count() })
                                .ToList();

            // Get Total Material Count by Level
            var materialList = _context.MaterialMaster
                                .Where(m => m.Project.ID == projectID && m.Block == block)
                                .GroupBy(m => m.Level)
                                .OrderBy(m => m.Key.PadLeft(3))
                                .ToList();

            var materialOverallProgressList = materialList.Select(m => new MaterialInProgress
            {
                Block = block,
                Level = m.Key,
                InstalledMaterialCount = installedMaterials
                                    .Where(im => im.Level == m.Key).Select(im => im.Count).FirstOrDefault(),
                TotalMaterialCount = m.Count()
            }).Take(3).ToList();

            materialOverallProgressList.ForEach(tm =>
            tm.Progress = Math.Round(((double)tm.InstalledMaterialCount) / ((double)tm.TotalMaterialCount) * 100, 2));

            return materialOverallProgressList.Where(IP => IP.InstalledMaterialCount > 0);
        }

        protected MaterialStats CreateStats(int projectID)
        {
            /*
            MaterialStats materialStats = new MaterialStats();

            materialStats = CreateMaterialCount(materialStats, projectID);
            materialStats = CreateMRFCount(materialStats, projectID);
            materialStats = CreateQCFailedCount(materialStats, projectID);
            */

            var lstMaterialMasters = _context.MaterialMaster
                .Include(m => m.MRF)
                .Include(m => m.StageAudits)
                .ThenInclude(sa => sa.Stage)
                .Include(m => m.StageAudits)
                .ThenInclude(sa => sa.QCCases)
                .ThenInclude(c => c.Defects)
                .Where(m => m.Project.ID == projectID)
                .ToList();

            // Create material count
            double InstalledMaterialsCount = lstMaterialMasters
                .Where(mm => mm.StageAudits.OrderByDescending(s => s.Stage.Order).FirstOrDefault()?.StageID == installedStageId).Distinct().Count();

            double TotalMaterialsCount = lstMaterialMasters.Count();
            double InstalledMaterialsProgress = 0;
            if (TotalMaterialsCount > 0)
                InstalledMaterialsProgress = Math.Round((InstalledMaterialsCount / TotalMaterialsCount) * 100, 2);

            double DeliveredMaterialsCount = lstMaterialMasters
                .Where(mm => mm.StageAudits.OrderByDescending(s => s.Stage.Order).FirstOrDefault()?.StageID == deliveredStageId).Distinct().Count();
            double RequestedMaterialsCount = lstMaterialMasters.Where(m => m.MRF != null).Count();
            double DeliveredMaterialsProgress = 0;
            if (RequestedMaterialsCount > 0)
                DeliveredMaterialsProgress = Math.Round((DeliveredMaterialsCount / RequestedMaterialsCount) * 100, 2);

            // Create MRF count

            int CompletedMRFCount = 0;
            double CompletedMRFProgress = 0;
            int TotalMRFCount = 0;
            var lstMrfMasters = lstMaterialMasters.Where(mm => mm.MRF != null).Select(mm => mm.MRF).Distinct().ToList();

            foreach (var mrf in lstMrfMasters)
                if (mrf.MRFCompletion == 100)
                    CompletedMRFCount++;

            TotalMRFCount = lstMrfMasters.Count();

            if (TotalMRFCount > 0)
                CompletedMRFProgress = Math.Round(((double)CompletedMRFCount / (double)TotalMRFCount) * 100, 2);

            // Create QC failed count
            var qcCases = _context.MaterialQCCase.Include(qc => qc.Defects)
                .Include(qc => qc.StageAudit).ThenInclude(sa => sa.MaterialMaster)
                .Where(qc => qc.StageAudit.MaterialMaster.ProjectId == projectID)
                .ToList();

            int QCTotalCount = qcCases.Count;
            int QCFailedCount = qcCases.Where(qc => qc.Defects.Any(d => d.IsOpen)).Count();

            var materialStats = new MaterialStats
            {
                InstalledMaterialsCount = Convert.ToInt32(InstalledMaterialsCount),
                TotalMaterialsCount = Convert.ToInt32(TotalMaterialsCount),
                InstalledMaterialsProgress = InstalledMaterialsProgress,
                DeliveredMaterialsCount = Convert.ToInt32(DeliveredMaterialsCount),
                RequestedMaterialsCount = Convert.ToInt32(RequestedMaterialsCount),
                DeliveredMaterialsProgress = DeliveredMaterialsProgress,
                CompletedMRFCount = CompletedMRFCount,
                TotalMRFCount = TotalMRFCount,
                CompletedMRFProgress = CompletedMRFProgress,
                QCTotalCount = QCTotalCount,
                QCFailedCount = QCFailedCount,
                QCFailedProgress = QCTotalCount > 0 ? Math.Round(((double)QCFailedCount / (double)QCTotalCount) * 100, 2) : 0
            };

            return materialStats;
        }

        /*
        protected MaterialStats CreateMaterialCount(MaterialStats materialStats, int projectID)
        {
            var lstMaterialMasters = _context.MaterialMaster.Include(m => m.MRF).Where(m => m.Project.ID == projectID).Select(M => new { M.ID, M.MRF }).ToList();
            IQueryable<MaterialStageAudit> materialStageAudits = _context.MaterialStageAudit.Where(sa => sa.MaterialMaster.Project.ID == projectID);

            double InstalledMaterialsCount = materialStageAudits.Where(sa => sa.Stage == getMaxStage()).Count();
            double TotalMaterialsCount = lstMaterialMasters.Count();
            double InstalledMaterialsProgress = 0;
            if (TotalMaterialsCount > 0)
                InstalledMaterialsProgress = Math.Round((InstalledMaterialsCount / TotalMaterialsCount) * 100, 2);

            double DeliveredMaterialsCount = materialStageAudits.Include(d => d.MaterialMaster).ThenInclude(m => m.StageAudits).ThenInclude(sa => sa.Stage)
                .Where(d => d.MaterialMaster.StageAudits.OrderByDescending(s => s.Stage.Order).FirstOrDefault().Stage.Name == DeliveredStageName).Count();
            double RequestedMaterialsCount = lstMaterialMasters.Where(m => m.MRF != null).Count();
            double DeliveredMaterialsProgress = 0;
            if (RequestedMaterialsCount > 0)
                DeliveredMaterialsProgress = Math.Round((DeliveredMaterialsCount / RequestedMaterialsCount) * 100, 2);

            materialStats.InstalledMaterialsCount = Convert.ToInt32(InstalledMaterialsCount);
            materialStats.TotalMaterialsCount = Convert.ToInt32(TotalMaterialsCount);
            materialStats.InstalledMaterialsProgress = InstalledMaterialsProgress;
            materialStats.DeliveredMaterialsCount = Convert.ToInt32(DeliveredMaterialsCount);
            materialStats.RequestedMaterialsCount = Convert.ToInt32(RequestedMaterialsCount);
            materialStats.DeliveredMaterialsProgress = DeliveredMaterialsProgress;

            return materialStats;
        }

        protected MaterialStats CreateMRFCount(MaterialStats materialStats, int projectID)
        {
            int CompletedMRFCount = 0;
            double CompletedMRFProgress = 0;
            int TotalMRFCount = 0;
            var lstMrfMasters = _context.MRFMaster.Include(m => m.Materials).Where(mrf => mrf.Materials.FirstOrDefault().Project.ID == projectID).Select(m => new { m.ID, m.Materials }).ToList();

            foreach (var mrf in lstMrfMasters)
                if (GetNumberOfInstalledMaterials(mrf.ID) == mrf.Materials.Count)
                    CompletedMRFCount++;
            TotalMRFCount = lstMrfMasters.Count();
            if (TotalMRFCount > 0)
                CompletedMRFProgress = Math.Round(((double)CompletedMRFCount / (double)TotalMRFCount) * 100, 2);

            materialStats.CompletedMRFCount = CompletedMRFCount;
            materialStats.TotalMRFCount = TotalMRFCount;
            materialStats.CompletedMRFProgress = CompletedMRFProgress;

            return materialStats;
        }

        protected MaterialStats CreateQCFailedCount(MaterialStats materialStats, int projectID)
        {
            IQueryable<MaterialMaster> QCMaterials = _context.MaterialMaster.Where(m => m.StageAudits != null)
                                                            .Where(m => m.Project.ID == projectID)
                                                            .Include(m => m.StageAudits)
                                                            .ThenInclude(sa => sa.Stage)
                                                            .Include(m => m.StageAudits)
                                                            .ThenInclude(sa => sa.QCCases)
                                                            .ThenInclude(c => c.Defects); ;

            int QCTotalCount = _context.MaterialQCCase.Count();

            int QCFailedCount = 0;
            foreach (MaterialMaster materialMaster in QCMaterials)
            {
                List<int> processedCaseIds = new List<int>();
                MaterialStageAudit currStageAudit = materialMaster.StageAudits.OrderByDescending(sa => sa.CreatedDate).FirstOrDefault();
                if (currStageAudit != null)
                {
                    if (currStageAudit.Stage.IsQCStage)
                    {
                        if (!currStageAudit.StagePassed)
                        {
                            foreach (MaterialQCCase qcCase in currStageAudit.QCCases)
                            {
                                if (qcCase.Defects != null)
                                {
                                    foreach (MaterialQCDefect qcDefect in qcCase.Defects)
                                        if (qcDefect.IsOpen && !processedCaseIds.Contains(qcCase.ID))
                                        {
                                            QCFailedCount++;
                                            processedCaseIds.Add(qcCase.ID);
                                        }
                                }
                            }
                        }
                    }
                }
            }

            materialStats.QCTotalCount = QCTotalCount;
            materialStats.QCFailedCount = QCFailedCount;
            materialStats.QCFailedProgress = Math.Round(((double)QCFailedCount / (double)QCTotalCount) * 100, 2);

            return materialStats;
        }
        */

        protected IEnumerable<QCOpenMaterial> CreateQCOpenMaterialsList(int projectID)
        {
            var QCOpenMaterialsList = _context.MaterialQCCase.Include(qc => qc.Defects)
                .Include(qc => qc.StageAudit).ThenInclude(sa => sa.MaterialMaster)
                .Where(qc => qc.Defects.Any(d => d.IsOpen) && qc.StageAudit.MaterialMaster.ProjectId == projectID)
                .Select(qc => new QCOpenMaterial
                {
                    Block = qc.StageAudit.MaterialMaster.Block,
                    Level = qc.StageAudit.MaterialMaster.Level,
                    Zone = qc.StageAudit.MaterialMaster.Zone,
                    CaseName = qc.CaseName,
                    CaseID = qc.ID,
                    MarkingNo = qc.StageAudit.MaterialMaster.MarkingNo,
                    MaterialDescription = $"{qc.StageAudit.MaterialMaster.Block}, Level {qc.StageAudit.MaterialMaster.Level}, Zone {qc.StageAudit.MaterialMaster.Zone}",
                    Remarks = qc.StageAudit.Remarks,
                    StageID = qc.StageAudit.StageID,
                    CreatedOn = qc.CreatedDate
                })
                .Distinct()
                .OrderByDescending(qc => qc.CreatedOn)
                .ToList();
            /*
            List<QCOpenMaterial> QCOpenMaterialsList = new List<QCOpenMaterial>();
            IEnumerable<MaterialMaster> materialMasters = _context.MaterialMaster
                                                            .Include(m => m.Project)
                                                            .Include(m => m.StageAudits)
                                                            .ThenInclude(sa => sa.Stage)
                                                            .Include(m => m.StageAudits)
                                                            .ThenInclude(sa => sa.QCCases)
                                                            .ThenInclude(c => c.Defects)
                                                            .Where(m => m.StageAudits.Count > 0
                                                            && m.Project.ID == projectID);


            int resultCount = 0;

            if (materialMasters.Count() > 0)
            {
                IEnumerable<MaterialMaster> QCOpenMaterials = materialMasters
                    .Where(m => m.StageAudits.OrderByDescending(sa => sa.CreatedDate).FirstOrDefault().Stage.IsQCStage
                        && m.StageAudits.OrderByDescending(sa => sa.CreatedDate).FirstOrDefault().StagePassed == false);

                foreach (MaterialMaster qcOpenMaterial in QCOpenMaterials)
                {
                    MaterialQCCase qcOpenCase = GetQCOpenCase(qcOpenMaterial);

                    if (resultCount >= 10)
                        break;

                    if (qcOpenCase != null)
                    {
                        QCOpenMaterial newQCOpenMaterial = new QCOpenMaterial();
                        newQCOpenMaterial.CaseID = qcOpenCase.ID;
                        newQCOpenMaterial.CaseName = qcOpenCase.CaseName;
                        newQCOpenMaterial.ID = qcOpenMaterial.ID;
                        newQCOpenMaterial.StageID = qcOpenMaterial.StageAudits.OrderByDescending(sa => sa.CreatedDate).FirstOrDefault().ID;
                        newQCOpenMaterial.MarkingNo = qcOpenMaterial.MarkingNo;
                        newQCOpenMaterial.MaterialDescription = qcOpenMaterial.Project.Name + "-" + qcOpenMaterial.Block + "-" + qcOpenMaterial.Level + "-" + qcOpenMaterial.Zone;
                        newQCOpenMaterial.CreatedOn = qcOpenMaterial.StageAudits.OrderByDescending(sa => sa.CreatedDate).FirstOrDefault().CreatedDate;
                        newQCOpenMaterial.Remarks = qcOpenMaterial.StageAudits.OrderByDescending(sa => sa.CreatedDate).FirstOrDefault().Remarks;
                        newQCOpenMaterial.Block = qcOpenMaterial.Block;
                        newQCOpenMaterial.Level = qcOpenMaterial.Level;
                        newQCOpenMaterial.Zone = qcOpenMaterial.Zone;
                        QCOpenMaterialsList.Add(newQCOpenMaterial);
                        resultCount++;
                    }
                }
            }
            */
            return QCOpenMaterialsList.Take(10);
        }

        protected IEnumerable<MaterialListItem> CreateDeliveredInstalledMaterialsList(List<MaterialMaster> listMaterials, int stageID)
        {
            List<MaterialListItem> materialList = new List<MaterialListItem>();

            var materials = listMaterials.Where(m => m.StageAudits.OrderByDescending(sa => sa.Stage.Order).FirstOrDefault()?.StageID == stageID)
                .OrderByDescending(m => m.StageAudits.OrderByDescending(sa => sa.Stage.Order).FirstOrDefault()?.CreatedDate)
                .ToList();
            /*
            IEnumerable<MaterialStageAudit> materialStageAudits = _context.MaterialStageAudit
                                                                    .Include(m => m.MaterialMaster)
                                                                    .ThenInclude(m => m.MRF)
                                                                    .Include(m => m.MaterialMaster)
                                                                    .ThenInclude(m => m.Project)
                                                                    .Include(m => m.Stage)
                                                                    .Include(m => m.MaterialMaster)
                                                                    .ThenInclude(m => m.MaterialInfoAudits)
                                                                    .Where(m => m.MaterialMaster.Project.ID == projectID &&
                                                                                m.MaterialMaster.StageAudits.OrderByDescending(sa => sa.Stage.Order).FirstOrDefault().Stage.Name == status);
            */

            foreach (MaterialMaster materialMaster in materials.Take(10))
            {
                MaterialListItem materialListItem = new MaterialListItem();
                materialListItem.MarkingNo = materialMaster.MarkingNo;
                materialListItem.Block = materialMaster.Block;
                materialListItem.Level = materialMaster.Level;
                materialListItem.Zone = materialMaster.Zone;
                materialListItem.Type = materialMaster.MaterialType;
                materialListItem.ID = materialMaster.ID;

                if (materialMaster.MRF != null)
                {
                    materialListItem.MRFNo = materialMaster.MRF.MRFNo;
                    materialListItem.ExpectedDelivery = (materialMaster.MaterialInfoAudits == null || materialMaster.MaterialInfoAudits.Count == 0) ? materialMaster.MRF.ExpectedDeliveryDate :
                                                        materialMaster.MaterialInfoAudits.OrderByDescending(a => a.CreatedDate).FirstOrDefault().ExpectedDeliveryDate;
                }
                materialList.Add(materialListItem);
            }

            return materialList;
        }

        protected List<MRF> CreateCompletedMRFList(int project_id)
        {
            var mrfMasters = _context.MRFMaster
                .Include(m => m.Materials)
                .ThenInclude(m => m.Project)
                .Where(m => m.MRFCompletion == 100 && m.Materials.FirstOrDefault().ProjectId == project_id)
                .OrderByDescending(m => m.ExpectedDeliveryDate).ToList();

            List<MRF> mrfs = new List<MRF>();

            foreach (MRFMaster mrfMaster in mrfMasters.Take(10))
            {
                MRF mrf = new MRF();

                mrf.MrfNo = mrfMaster.MRFNo;
                mrf.Block = mrfMaster.Materials.FirstOrDefault().Block;
                mrf.Level = mrfMaster.Materials.FirstOrDefault().Level;
                mrf.Zone = mrfMaster.Materials.FirstOrDefault().Zone;

                List<string> materialTypes = new List<string>();
                foreach (MaterialMaster materialMaster in mrfMaster.Materials)
                    materialTypes.Add(materialMaster.MaterialType);

                mrf.MaterialTypes = materialTypes.Distinct().ToList();

                mrfs.Add(mrf);
            }

            return mrfs;
        }

        protected MaterialQCCase GetQCOpenCase(MaterialMaster qcOpenMaterial)
        {
            List<MaterialQCCase> qcCases = qcOpenMaterial.StageAudits.OrderByDescending(sa => sa.CreatedDate).FirstOrDefault().QCCases;

            if (qcCases == null)
                return null;

            foreach (MaterialQCCase qcCase in qcCases)
            {
                if (qcCase.Defects != null)
                {
                    foreach (MaterialQCDefect qcDefect in qcCase.Defects)
                        if (qcDefect.IsOpen)
                            return qcCase;
                }
            }
            return null;
        }

        protected DailyMaterialStatusCount CreateDailyMaterialStatusCounts(int projectID)
        {
            var DailyStatus = _context.MaterialStageAudit
                                .Include(a => a.MaterialMaster)
                                .Where(sa => sa.CreatedDate.Date == DateTime.UtcNow.Date && sa.MaterialMaster.Project.ID == projectID);

            DailyMaterialStatusCount dailyMaterialStatusCount = new DailyMaterialStatusCount();

            dailyMaterialStatusCount.StartDeliveryCount = DailyStatus.Where(d => d.Stage.ID == startDeliveryStageId).Count();
            dailyMaterialStatusCount.DeliveredCount = DailyStatus.Where(d => d.Stage.ID == deliveredStageId).Count();
            dailyMaterialStatusCount.InstalledCount = DailyStatus.Where(d => d.Stage.ID == installedStageId).Count();

            return dailyMaterialStatusCount;
        }

        #endregion
    }
}

using astorWorkMaterialTracking.Models;
using astorWorkDAO;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static astorWorkShared.Utilities.Enums;

namespace astorWorkMaterialTracking.Common
{
    public class CommonDashboardController : CommonController
    {
        protected IAstorWorkBlobStorage _blobStorage;

        protected CommonDashboardController(astorWorkDbContext context) : base(context)
        {
        }
        #region Common Methods

        protected async Task<List<MaterialOverallProgress>> CreateOverallProgress(int projectID, List<MaterialStageAudit> materialStageAudits)
        {
            await SetMaterialStagesID();

            // Get Total Material Count by Level
            int TotalMaterialsCount = _context.MaterialMaster
                                .Where(m => m.Project.ID == projectID).Count();

            List<MaterialOverallProgress> progressByDate = materialStageAudits.Where(m => m.Stage.ID == installedStageID 
                                                                                       && m.MaterialMaster.Project.ID == projectID)
                                                                              .GroupBy(m => m.CreatedDate.Date)
                                                                              .OrderBy(m => m.Key.Date)   
                                                                              .Select(m => new MaterialOverallProgress
                                                                              {
                                                                                  Date = m.Key.Date,
                                                                                  Progress = GetProgress(m.Count(), TotalMaterialsCount)
                                                                              })
                                                                              .ToList(); 

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

        protected async Task<List<MaterialInProgress>> CreateInProgress(int projectID, string block, IEnumerable<MaterialStageAudit> materialStageAudits)
        {
            // List<MaterialInProgress> materialOverallProgressList = new List<MaterialInProgress>();

            // Installed stage will always be the last stage
            MaterialStageMaster installedStage = await _context.MaterialStageMaster.OrderBy(msm => msm.Order).LastOrDefaultAsync();

            // Get Installed Material Count by Level
            var installedMaterials = materialStageAudits
                                    .Where(m => m.Stage == installedStage 
                                             && m.MaterialMaster.Project.ID == projectID 
                                             && m.MaterialMaster.Block == block)
                                    .GroupBy(m => m.MaterialMaster.Level)
                                    .OrderBy(m => m.Key.PadLeft(3))
                                    .Select(m => new { Level = m.Key, Count = m.Count() })
                                    .ToList();

            // Get Total Material Count by Level
            var materials = await _context.MaterialMaster.Where(m => m.Project.ID == projectID 
                                                               && m.Block == block)
                                                              .GroupBy(m => m.Level)
                                                              .OrderBy(m => m.Key.PadLeft(3))
                                                              .ToListAsync();

            var materialOverallProgressList = materials.Select(m => new MaterialInProgress
                                                                    {
                                                                        Block = block,
                                                                        Level = m.Key,
                                                                        InstalledMaterialCount = installedMaterials.Where(im => im.Level == m.Key)
                                                                                                                   .Select(im => im.Count)
                                                                                                                   .FirstOrDefault(),
                                                                        TotalMaterialCount = m.Count()
                                                                    })
                                                                    .Take(3)
                                                                    .ToList();

            materialOverallProgressList.ForEach(tm => tm.Progress = GetProgress(tm.InstalledMaterialCount, tm.TotalMaterialCount));
            return materialOverallProgressList.Where(IP => IP.InstalledMaterialCount > 0).ToList();
        }

        protected async Task<MaterialStats> CreateStats(int projectID)
        {
            await SetMaterialStagesID();
            List<UserMaster> Usermaster = await _context.UserMaster.Include(um => um.Project).Where(um => um.ProjectID == projectID && um.RoleID==4).ToListAsync();
            List<ProjectMaster> Projectdetails = await _context.ProjectMaster.Where(um => um.ID == projectID).ToListAsync();
            List<MaterialMaster> materials = await _context.MaterialMaster
                                                           .Include(m => m.MRF)
                                                           .Include(m => m.StageAudits)
                                                           .ThenInclude(sa => sa.Stage)
                                                           .Include(m => m.QCCases)
                                                           .ThenInclude(c => c.Defects)
                                                           .Include(c=>c.Project)
                                                           .Where(m => m.Project.ID == projectID)
                                                           .ToListAsync();
         
            // Create material count
            int installedMaterialsCount = GetCount(materials, installedStageID);
            int totalMaterialsCount = materials.Count();
            double installedMaterialsProgress = GetProgress(installedMaterialsCount, totalMaterialsCount);
            
            int deliveredMaterialsCount = GetCount(materials, deliveredStageID);
            int requestedMaterialsCount = materials.Where(m => m.MRF != null).Count();
            double deliveredMaterialsProgress = GetProgress(deliveredMaterialsCount, requestedMaterialsCount);

            //Ready To Deliverd Materials Count
            int readyToDeliveredMaterialsCount = materials.Where((m => m.StageAudits.FirstOrDefault()?.StageID == producedStageID)).ToList().Count();
            // commented for alec Material Dashboard
             // && m.StageAudits[0].QCStatus == Enums.JobStatus.All_QC_passed
             //|| (m.StageAudits.FirstOrDefault()?.StageID == startDeliveryStageID
             //&& m.StageAudits[0].QCStatus == Enums.JobStatus.All_QC_passed))).ToList().Count();

            double readyToDeliveredMaterialsProgress = GetProgress(readyToDeliveredMaterialsCount, requestedMaterialsCount);

            // Create MRF count
            List<MRFMaster> mrfs = materials.Where(mm => mm.MRF != null)
                                            .Select(mm => mm.MRF)
                                            .Distinct()
                                            .ToList();

            int completedMRFCount = mrfs.Where(mrf => mrf.MRFCompletion == 1).Count();
            int totalMRFCount = (mrfs != null)?mrfs.Count():0;
            double completedMRFProgress = GetProgress(completedMRFCount, totalMRFCount);

            // Create QC failed count
            List<MaterialQCCase> qcCases = await _context.MaterialQCCase.Include(qc => qc.Defects)
                                                                        .Include(qc => qc.MaterialMaster)
                                                                        .Where(qc => qc.MaterialMaster.ProjectID == projectID)
                                                                        .ToListAsync();
            var qcStrcturalChecklist = await _context.ChecklistAudit.Include(qc => qc.MaterialStageAudit)
                                                            .ThenInclude(m => m.MaterialMaster)
                                                            .Where(qc => qc.MaterialStageAudit.MaterialMaster.ProjectID == projectID && qc.JobScheduleID==null)
                                                            .GroupBy(qc => new { qc.ChecklistID, qc.MaterialStageAuditID })
                                                            .ToListAsync();
            var archiqcchecklist = await _context.ChecklistAudit
                                                            .Include(m => m.JobSchedule)
                                                            .ThenInclude(m=>m.Material)
                                                            .Where(qc => qc.JobSchedule.Material.ProjectID == projectID && qc.JobScheduleID !=null)
                                                            .GroupBy(qc => new { qc.JobScheduleID, qc.ChecklistID })
                                                            .ToListAsync();
            //var qcchecklist = materials.Where(mm => mm.StageAudits.Where(sa => sa.QCStatus > 0).Count() > 0).ToList();
            int qcTotalCount = qcCases.Count + (qcStrcturalChecklist != null && qcStrcturalChecklist.Count >0 ? qcStrcturalChecklist.Count:0) + (archiqcchecklist != null && archiqcchecklist.Count > 0 ? archiqcchecklist.Count : 0);
            int qcFailedCount = qcCases.Where(qc => qc.Defects.Any(d => d.Status == Enums.QCStatus.QC_failed_by_Maincon)).Count();
            double qcFailedProgress = GetProgress(qcFailedCount, qcTotalCount);
            string projectmanagerName = Usermaster.Where(j => j.Project.ID == projectID && j.RoleID == 4).Select(s => s.PersonName).FirstOrDefault();
            string projectstartdate = Projectdetails.Where(j => j.ID == projectID).Select(s => s.EstimatedStartDate).FirstOrDefault().ToString();
            string projectEnddate = Projectdetails.Where(j => j.ID == projectID).Select(s => s.EstimatedEndDate).FirstOrDefault().ToString();

            MaterialStats materialStats = new MaterialStats
            {
                InstalledMaterialsCount = installedMaterialsCount,
                TotalMaterialsCount = totalMaterialsCount,
                InstalledMaterialsProgress = installedMaterialsProgress,
                DeliveredMaterialsCount = deliveredMaterialsCount,
                RequestedMaterialsCount = requestedMaterialsCount,
                DeliveredMaterialsProgress = deliveredMaterialsProgress,
                CompletedMRFCount = completedMRFCount,
                TotalMRFCount = totalMRFCount,
                CompletedMRFProgress = completedMRFProgress,
                QCTotalCount = qcTotalCount,
                QCFailedCount = qcFailedCount,
                QCFailedProgress = qcFailedProgress,
                ReadyToDeliveredMaterialsProgress = readyToDeliveredMaterialsProgress,
                ReadyToDeliveredMaterialsCount = readyToDeliveredMaterialsCount,
                ProjectManager = projectmanagerName,
                ProjectStartDate = DateTimeOffset.Parse(projectstartdate),
                ProjectEndDate = DateTimeOffset.Parse(projectEnddate),
            };

            return materialStats;
        }
        protected List<Materialstagedetails> CreateMaterialStageStats(int projectID)
        {

            List<Materialstagedetails> materials = new List<Materialstagedetails>();
            materials = _context.MaterialStageAudit
                           .Include(c => c.MaterialMaster)
                            .ThenInclude(c => c.MRF)
                           .Include(c => c.Stage)
                           .Where(m => m.MaterialMaster.ProjectID == projectID)
                           .GroupBy(b=> new {
                               b.MaterialMaster.ID
                              ,b.MaterialMaster.Block
                               ,b.MaterialMaster.Level
                               ,b.MaterialMaster.Zone
                               ,type = b.MaterialMaster.MaterialType.Name
                               ,b.MaterialMaster.MarkingNo
                               ,b.MaterialMaster.MRF.MRFNo
                               ,stageID = b.Stage.ID
                               ,stageName = b.Stage.Name
                               ,stageOrder = b.Stage.Order
                               ,qcstatus=b.QCStatus,
                               createdDate=b.CreatedDate
                              
                           })
                      .Select(list => new Materialstagedetails
                      {
                          Block =  list.Key.Block,
                          Level = list.Key.Level,
                          Zone = list.Key.Zone,
                          type = list.Key.type,
                          MarkingNo = list.Key.MarkingNo,
                          Module = $"{list.Key.Block}-{list.Key.Level}-{list.Key.Zone}-{list.Key.MarkingNo}",
                          ID = list.Key.ID,
                          StageID = list.Key.stageID,
                          StageName = list.Key.stageName,
                          mrfNo = list.Key.MRFNo,
                          StageOrder=list.Key.stageOrder,
                          qCStatus = list.Key.qcstatus.ToString(),
                          createdDate = list.Key.createdDate.Date,
                          UtcDate=DateTime.UtcNow.Date
                      }).Distinct().ToList();

            return materials;
        }
        protected  async Task<List<MaterialDetail>>  CreateReadytoprojectStats(int projectID)
        {
            MaterialStats materialStats = null;
            List<JobAudit> materials = new List<JobAudit>();
            List<JobAudit> result = new List<JobAudit>();
            List<MaterialDetail> sss = new List<MaterialDetail>();
            try
            {

                  materials = await _context.JobAudit
                                                           .Include(m => m.JobSchedule)
                                                           .ThenInclude(js => js.Material)
                                                            .ThenInclude(j => j.MaterialType)
                                                           .Include(j => j.JobSchedule.Trade)
                                                          .Where(m => m.JobSchedule.Material.Project.ID == projectID)
                                                          .ToListAsync();
                List<JobAudit> Completedjobs = materials.Where(j => j.Status == JobStatus.Job_completed).ToList();
                List<JobAudit> QCCompletedjobs = materials.Where(j => j.Status == JobStatus.All_QC_passed).ToList();
                result = Completedjobs.Where(p => QCCompletedjobs.Any(p2 => p2.JobSchedule.ID == p.JobSchedule.ID)).Distinct().ToList();
                int ReadytoProject = result.Count();
                sss = result.GroupBy(s => new { s.JobSchedule.Material.Block, s.JobSchedule.Material.Level, s.JobSchedule.Material.Zone,s.JobSchedule.ID,s.JobSchedule.Trade.Name,s.JobSchedule.Material.MaterialType})
                    .Select(list => new MaterialDetail
                    {
                        ID = list.Key.ID,
                        Jobname =list.Key.Name,
                        Block=list.Key.Block,
                        Level=list.Key.Level,
                        Zone=list.Key.Zone,
                        MaterialType = list.Key.MaterialType.Name
                    }).Distinct().Take(10).ToList();
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return sss;
        }



        protected int GetCount(List<MaterialMaster> materials, int stageID) {
            return materials.Where(mm => mm.StageAudits.OrderByDescending(s => s.Stage.Order)
                                                             .FirstOrDefault()?
                                                             .StageID == stageID)
                            .Distinct()
                            .Count();
        }

        protected double GetProgress(double stageCount, double total) {
            if (total > 0)
            {
                double percent = stageCount / total * 100;
                return Math.Round(percent, 2);
            }
            return 0;
        }

        protected DelayedMaterial CreateDelayedMaterial(MaterialMaster material, int index, Random rndMode, string[] MATERIAL_TYPES) {
            char gridLineX = (char)(65 + index % 26);
            char gridLineY = (char)(48 + index % 10);
            string gridLine = new string(new char[] { gridLineX, gridLineY });
            
            Random rndDays = new Random(67890);
            string materialType = MATERIAL_TYPES[rndMode.Next(4)];

            return new DelayedMaterial
            {
                Block = material.Block.Replace("BLK", "SMU").Replace(" ", ""),
                Level = material.Level,
                Zone = material.Zone,
                MarkingNo = $"{materialType.ToUpper()}-L{material.Level}-Z{material.Zone}-{gridLine}-00{rndMode.Next(1, 8)}",
                GridLine = gridLine,
                Type = materialType,
                MRFNo = material.MRF.MRFNo.Replace("PP09", "MRF"),
                PlannedProductionDate = DateTime.Today.AddDays(-rndDays.Next(50, 70)),
                ActualProductionDate = DateTime.Today.AddDays(-rndDays.Next(40, 50)),
                PlannedDeliveryDate = DateTime.Today.AddDays(-rndDays.Next(30, 50)),
                ActualDeliveryDate = DateTime.Today.AddDays(-rndDays.Next(20, 30)),
                PlannedInstallationDate = DateTime.Today.AddDays(-rndDays.Next(10, 30)),
                ActualInstallationDate = DateTime.Today.AddDays(-rndDays.Next(1, 10)),
            };
        }

        

        protected async Task<List<QCOpenMaterial>> CreateQCOpenMaterialsList(int projectID)
        {
            return await _context.MaterialQCCase.Include(qc => qc.Defects)
                .Include(qc => qc.MaterialMaster)
                .Where(qc => qc.Defects.Any(d => d.Status == Enums.QCStatus.QC_failed_by_Maincon) && qc.MaterialMaster.ProjectID == projectID)
                .Select(qc => new QCOpenMaterial
                {
                    Block = qc.MaterialMaster.Block,
                    Level = qc.MaterialMaster.Level,
                    Zone = qc.MaterialMaster.Zone,
                    CaseName = qc.CaseName,
                    CaseID = qc.ID,
                    MarkingNo = qc.MaterialMaster.MarkingNo,
                    MaterialDescription = $"{qc.MaterialMaster.Block}-L{qc.MaterialMaster.Level}-{qc.MaterialMaster.Zone}",
                    CreatedOn = qc.CreatedDate
                })
                .Distinct()
                .OrderByDescending(qc => qc.CreatedOn)
                .Take(10)
                .ToListAsync();
        }

        protected List<MaterialDetail> ReadyToDeliverdedMaterialsList(List<MaterialMaster> materials, int stageID,int startDeliveryStageID)
        {
            materials = materials.Where((m => m.StageAudits.FirstOrDefault()?.StageID == stageID)).OrderByDescending(m => m.StageAudits.OrderByDescending(sa => sa.Stage.Order).FirstOrDefault()?.CreatedDate).ToList();
            //comented for alec materialdashboard
           //&& m.StageAudits[0].QCStatus == Enums.JobStatus.All_QC_passed
           //|| (m.StageAudits.FirstOrDefault()?.StageID == startDeliveryStageID
           //&& m.StageAudits[0].QCStatus == Enums.JobStatus.All_QC_passed)))
           // .OrderByDescending(m => m.StageAudits.OrderByDescending(sa => sa.Stage.Order).FirstOrDefault()?.CreatedDate).ToList();

            List<MaterialDetail> readyToDeliveredMaterials = new List<MaterialDetail>();

            foreach (MaterialMaster materialMaster in materials.Take(10))
            {
                MaterialDetail material = CreateMaterialDetail(materialMaster);
                readyToDeliveredMaterials.Add(material);
            }

            return readyToDeliveredMaterials;
        }
        protected List<MaterialDetail> CreateDeliveredInstalledMaterialsList(List<MaterialMaster> materials, int stageID)
        {
            materials = materials.Where(m => m.StageAudits.OrderByDescending(sa => sa.Stage.Order).FirstOrDefault()?.StageID == stageID)
                .OrderByDescending(m => m.StageAudits.OrderByDescending(sa => sa.Stage.Order).FirstOrDefault()?.CreatedDate).ToList();

            List<MaterialDetail> deliveredInstalledMaterials = new List<MaterialDetail>();

            foreach (MaterialMaster materialMaster in materials.Take(10))
            {
                MaterialDetail material = CreateMaterialDetail(materialMaster);
                deliveredInstalledMaterials.Add(material);
            }

            return deliveredInstalledMaterials;
        }
     
        protected MaterialDetail CreateMaterialDetail(MaterialMaster materialMaster) {
            MaterialDetail material = new MaterialDetail();
            material.StageId = materialMaster.StageAudits.Select(s => s.Stage.ID).FirstOrDefault();
            material.MarkingNo = materialMaster.MarkingNo;
            material.Block = materialMaster.Block;
            material.Level = materialMaster.Level;
            material.Zone = materialMaster.Zone;
            material.MaterialType = materialMaster.MaterialType.Name;
            material.ID = materialMaster.ID;

            if (materialMaster.MRF != null)
            {
                material.MRFNo = materialMaster.MRF.MRFNo;
                material.ExpectedDelivery = (materialMaster.MaterialInfoAudits == null || materialMaster.MaterialInfoAudits.Count == 0) ? materialMaster.MRF.ExpectedDeliveryDate :
                                                    materialMaster.MaterialInfoAudits.OrderByDescending(a => a.CreatedDate).FirstOrDefault().ExpectedDeliveryDate;
            }

            return material;
        }

        protected List<MRF> CreateCompletedMRFList(int projectID)
        {
            IEnumerable<MRFMaster> mrfs = _context.MRFMaster
                                                  .Include(m => m.Materials)
                                                  .ThenInclude(m => m.Project)
                                                  .Where(m => m.MRFCompletion == 1 && m.Materials.FirstOrDefault().ProjectID == projectID)
                                                  .OrderByDescending(m => m.ExpectedDeliveryDate);

            List<MRF> completedMRFs = new List<MRF>();

            foreach (MRFMaster mrfMaster in mrfs.Take(10))
            {
                MRF mrf = new MRF();
                mrf.MrfNo = mrfMaster.MRFNo;

                List<string> levels = new List<string>();
                List<string> zones = new List<string>();
                List<MaterialTypeMaster> materialTypes = new List<MaterialTypeMaster>();
                foreach (MaterialMaster materialMaster in mrfMaster.Materials)
                {
                    levels.Add(materialMaster.Level);
                    zones.Add(materialMaster.Zone);
                    materialTypes.Add(materialMaster.MaterialType);
                }

                mrf.Block = mrfMaster.Materials.FirstOrDefault().Block;
                mrf.Level = levels.Distinct().ToList();
                mrf.Zone = zones.Distinct().ToList();
                mrf.MaterialTypes = materialTypes.Select(mt => mt.Name).Distinct().ToList();

                completedMRFs.Add(mrf);
            }

            return completedMRFs;
        }

        protected MaterialQCCase GetQCOpenCase(MaterialMaster qcOpenMaterial)
        {
            List<MaterialQCCase> qcCases = qcOpenMaterial.QCCases;

            if (qcCases == null)
                return null;

            foreach (MaterialQCCase qcCase in qcCases)
            {
                if (qcCase.Defects != null)
                {
                    foreach (MaterialQCDefect qcDefect in qcCase.Defects)
                        if (qcDefect.Status < Enums.QCStatus.QC_passed_by_Maincon)
                            return qcCase;
                }
            }
            return null;
        }

        protected async Task<DailyMaterialStatusCount> CreateDailyMaterialStatusCounts(int projectID)
        {
            await SetMaterialStagesID();

            var dailyMaterialStatus = await _context.MaterialStageAudit
                                                                   .Include(a => a.MaterialMaster)
                                                                   .Where(sa => sa.CreatedDate.Date == DateTime.UtcNow.Date 
                                                                             && sa.MaterialMaster.Project.ID == projectID)
                                                                   .Select(ms => new {ms.MaterialMasterID,ms.StageID })
                                                                   .Distinct()
                                                                   .ToListAsync();

            DailyMaterialStatusCount dailyMaterialStatusCount = new DailyMaterialStatusCount();

            dailyMaterialStatusCount.ProducedCount = dailyMaterialStatus.Where(d => d.StageID == producedStageID).Count();
            dailyMaterialStatusCount.DeliveredCount = dailyMaterialStatus.Where(d => d.StageID == deliveredStageID).Count();
            dailyMaterialStatusCount.InstalledCount = dailyMaterialStatus.Where(d => d.StageID == installedStageID).Count();

            return dailyMaterialStatusCount;
        }

        protected async Task<List<MaterialMaster>> GetMaterials(int projectID)
        {
            return await _context.MaterialMaster
                           .Include(m => m.Project)
                           .Include(m => m.StageAudits)
                           .ThenInclude(sa => sa.Stage)
                           .Include(m => m.MRF)
                           .Include(m => m.MaterialType)
                           .Where(m => m.Project.ID == projectID)
                           .ToListAsync();
        }
        
        protected List<MaterialStageAudit> GetMaterialStages(IEnumerable<MaterialMaster> materials)
        {
            return materials
                  .Where(mm => mm.StageAudits != null)
                  .SelectMany(mm => mm.StageAudits)
                  .Distinct().ToList();
        }

        protected string GetLatestBIMSession(int projectID)
        {

            string containerName = AppConfiguration.GetVideoContainer();
            string bimVideoUrl = null;
            BIMSyncAudit bimSync = _context.BIMSyncAudit
                                    .Include(bs => bs.Project)
                                    .Where(bs => bs.Project.ID == projectID)
                                    .OrderByDescending(bs => bs.SyncTime)
                                    .FirstOrDefault();
            if (bimSync != null)
                bimVideoUrl = string.IsNullOrEmpty(bimSync.BIMVideoUrl) ? null : _blobStorage.GetSignedURL(containerName, bimSync.BIMVideoUrl);

            return bimVideoUrl;
        }

        protected List<MaterialOverallProgress> GetMaterialOverallProgress(int projectID, IEnumerable<MaterialStageAudit> materialStages, MaterialStageMaster lastStage, int totalMaterialsCount)
        {
            IEnumerable<MaterialOverallProgress> progressByDate = GetProgressByDate(materialStages, lastStage, totalMaterialsCount);
            List<MaterialOverallProgress> materialOverallProgress = AddProjectStartProgress(projectID);
            return AddProgressByDate(materialOverallProgress, progressByDate);
        }

        protected List<MaterialOverallProgress> AddProjectStartProgress(int projectID)
        {
            List<MaterialOverallProgress> materialOverallProgress = new List<MaterialOverallProgress>();

            // starting from project start date
            materialOverallProgress.Add(new MaterialOverallProgress
            {
                Date = _context.ProjectMaster
                        .Where(p => p.ID == projectID).FirstOrDefault().EstimatedStartDate,
                Progress = 0
            });

            return materialOverallProgress;
        }

        protected IEnumerable<MaterialOverallProgress> GetProgressByDate(IEnumerable<MaterialStageAudit> materialStages, MaterialStageMaster lastStage, int totalMaterialsCount) {
            return materialStages.Where(sa => sa.Stage == lastStage)
                                 .GroupBy(m => m.CreatedDate.Date)
                                 .OrderBy(m => m.Key.Date)
                                 .Select(m => new MaterialOverallProgress
                                 {
                                     Date = m.Key.Date,
                                     Progress = GetProgress(m.Count(), totalMaterialsCount)
                                 });
        }

        protected List<MaterialOverallProgress> AddProgressByDate(List<MaterialOverallProgress> materialOverallProgress, IEnumerable<MaterialOverallProgress> progressByDate)
        {
            double progress = 0;
            foreach (MaterialOverallProgress mop in progressByDate)
            {
                progress += mop.Progress;
                mop.Progress = Math.Round(progress, 2);
                materialOverallProgress.Add(mop);

            }

            return materialOverallProgress;
        }

        protected List<MaterialInProgress> GetMaterialInProgress(IEnumerable<MaterialStageAudit> materialStages, MaterialStageMaster lastStage, IEnumerable<MaterialMaster> materials) {
            // Get installed materials by level (top 3)
            List<MaterialInProgress> materialInProgress = new List<MaterialInProgress>();
            try
            {
                var installedMaterials = materialStages
                                        .Where(msa => msa.Stage == lastStage)
                                        .GroupBy(msa => new { msa.MaterialMaster.Block, msa.MaterialMaster.Level })
                                        .OrderBy(msa => msa.Key.Block)
                                        .ThenByDescending(m => m.Key.Level)
                                        .Select(msa => new { msa.Key.Block, msa.Key.Level, Count = msa.Count() });
                //.ThenByDescending(msa => int.Parse(new string(msa.Key.Level.TakeWhile(Char.IsDigit).ToArray())))
                //                        .ThenByDescending(msa => new string(msa.Key.Level.TakeWhile(Char.IsLetter).ToArray()))
                materialInProgress = installedMaterials.Select(im => new MaterialInProgress
                {
                    Block = im.Block,
                    Level = im.Level,
                    InstalledMaterialCount = im.Count,
                    TotalMaterialCount = (materials.Where(mm => mm.Block == im.Block && mm.Level == im.Level) != null) ? materials.Where(mm => mm.Block == im.Block && mm.Level == im.Level).Count() : 0
                }).ToList();

                materialInProgress.ForEach(tm =>
                    tm.Progress = GetProgress(tm.InstalledMaterialCount, tm.TotalMaterialCount)
                );
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return materialInProgress;
        }
        #endregion
    }
}

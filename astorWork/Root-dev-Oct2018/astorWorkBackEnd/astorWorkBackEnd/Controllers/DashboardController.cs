using astorWorkBackEnd.Common;
using astorWorkBackEnd.Models;
using astorWorkDAO;
using astorWorkShared.GlobalResponse;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkBackEnd.Controllers
{
    /// <summary>
    /// <route> projects/{project_id}/dashboard</route>
    /// all dashboard related controllers are available
    /// </summary>
    [Produces("application/json")]
    [Route("projects/{project_id}/dashboard")]
    public class DashboardController : CommonDashboardController
    {
        #region Declarations

        private IAstorWorkBlobStorage _blobStorage;

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="blobStorage"></param>
        public DashboardController(astorWorkDbContext context, IAstorWorkBlobStorage blobStorage) : base(context)
        {
            _blobStorage = blobStorage;
        }
        #endregion

        #region API methods

        /// <summary>
        ///  
        /// </summary>
        /// <param name="project_id"></param>
        /// <returns></returns>
        [Route("progress")]
        public APIResponse GetProjectProgress([FromRoute] int project_id)
        {
            try
            {
                var containerName = AppConfiguration.GetVideoContainer();

                if (!ModelState.IsValid)
                    return new APIBadRequest();

                var lstMaterialMaster = _context.MaterialMaster
                    .Include(mm => mm.Project)
                    .Include(mm => mm.StageAudits)
                    .ThenInclude(sa => sa.Stage)
                    .Where(m => m.Project.ID == project_id)
                    .ToList();

                var lstMaterialStageAudits = lstMaterialMaster
                    .Where(mm => mm.StageAudits != null)
                    .SelectMany(mm => mm.StageAudits)
                    .Distinct()
                    .ToList();

                var lastStage = _context.MaterialStageMaster.OrderBy(msm => msm.Order).Last();

                // Get latest BIM session
                string bimVideoUrl = null;
                var bimSync = _context.BIMSyncAudit
                    .Include(bs => bs.Project)
                    .Where(bs => bs.Project.ID == project_id)
                    .OrderByDescending(bs => bs.SyncTime)
                    .FirstOrDefault();
                if (bimSync != null)
                    bimVideoUrl = string.IsNullOrEmpty(bimSync.BIMVideoUrl) ?
                        null : _blobStorage.GetSignedURL(containerName, bimSync.BIMVideoUrl);

                // Get overall progress

                // Get Total Material Count by Level
                int TotalMaterialsCount = lstMaterialMaster.Count();

                var progressByDate = lstMaterialStageAudits
                    .Where(sa => sa.Stage == lastStage)
                    .GroupBy(m => m.CreatedDate.Date)
                    .OrderBy(m => m.Key.Date)
                    .Select(m => new MaterialOverallProgress
                    {
                        Date = m.Key.Date,
                        Progress = Math.Round((((double)m.Count()) / ((double)TotalMaterialsCount) * 100), 2)
                    });

                var materialOverallProgresses = new List<MaterialOverallProgress>();

                // starting from project start date
                materialOverallProgresses.Add(new MaterialOverallProgress
                {
                    Date = _context.ProjectMaster
                    .Where(p => p.ID == project_id).FirstOrDefault().EstimatedStartDate,
                    Progress = 0
                });

                double progress = 0;
                foreach (MaterialOverallProgress mop in progressByDate)
                {
                    progress += mop.Progress;
                    mop.Progress = progress;
                    materialOverallProgresses.Add(mop);
                }

                // Get in progress
                // Get installed materials by level (top 3)
                var installedMaterials = lstMaterialStageAudits
                                    .Where(msa => msa.Stage == lastStage)
                                    .GroupBy(msa => new { msa.MaterialMaster.Block, msa.MaterialMaster.Level })
                                    .OrderBy(msa => msa.Key.Block)
                                    .ThenByDescending(msa => int.Parse(new string(msa.Key.Level.TakeWhile(Char.IsDigit).ToArray())))
                                    .ThenByDescending(msa => new string(msa.Key.Level.TakeWhile(Char.IsLetter).ToArray()))
                                    .Select(msa => new { msa.Key.Block, msa.Key.Level, Count = msa.Count() })
                                    .ToList();

                var materialOverallProgressList = installedMaterials.Select(im => new MaterialInProgress
                {
                    Block = im.Block,
                    Level = im.Level,
                    InstalledMaterialCount = im.Count,
                    TotalMaterialCount = lstMaterialMaster.Where(mm => mm.Block == im.Block && mm.Level == im.Level).Count()
                }).ToList();

                materialOverallProgressList.ForEach(tm =>
                tm.Progress = Math.Round(((double)tm.InstalledMaterialCount) / ((double)tm.TotalMaterialCount) * 100, 2));

                return new APIResponse(0, new
                {
                    OverallProgress = materialOverallProgresses,
                    InProgress = materialOverallProgressList,
                    bimVideoUrl
                });
            }
            catch (Exception exc)
            {
                return new APIResponse(500, null, exc.Message);
            }
        }

        /// <summary>
        /// <URL>GET: projects/{projectID}/dashboard/Stats</URL>
        ///  returs you current stats count and progress data for dashboard
        /// </summary>
        /// <param name="project_id"></param>
        /// <returns></returns>
        [HttpGet("stats")]
        public APIResponse GetStats([FromRoute] int project_id)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            return new APIResponse(0, CreateStats(project_id));
        }

        /// <summary>
        /// <URL>GET: projects/{projectID}/dashboard/qc-open-and-daily-status</URL>
        ///  returs you current day progress and QC fail material data for dashboard
        /// </summary>
        /// <param name="project_id"></param>
        /// <returns></returns>
        [HttpGet("qc-open-and-daily-status")]
        public APIResponse GetQCOpenMaterialsAndDailyStatus([FromRoute] int project_id)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            var materials = _context.MaterialMaster
                .Include(m => m.Project)
                .Include(m => m.StageAudits)
                .ThenInclude(sa => sa.Stage)
                .Where(m => m.Project.ID == project_id)
                .ToList();

            var QCOpenMaterialsList = CreateQCOpenMaterialsList(project_id);
            var DeliveredMaterialList = CreateDeliveredInstalledMaterialsList(materials, deliveredStageId);
            var InstalledMaterialList = CreateDeliveredInstalledMaterialsList(materials, installedStageId);
            List<MRF> CompletedMRFList = CreateCompletedMRFList(project_id);
            DailyMaterialStatusCount DailyMaterialStatusCount = CreateDailyMaterialStatusCounts(project_id);

            return new APIResponse(0, new { DeliveredMaterialList, InstalledMaterialList, QCOpenMaterialsList, CompletedMRFList, DailyMaterialStatusCount });
        }
        #endregion
    }
}

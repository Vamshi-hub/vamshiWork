using astorWorkMaterialTracking.Common;
using astorWorkMaterialTracking.Models;
using astorWorkDAO;
using astorWorkShared.GlobalExceptions;
using astorWorkShared.GlobalResponse;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Controllers
{
    /// <summary>
    /// <route> projects/{project_id}/dashboard</route>
    /// all dashboard related controllers are available
    /// </summary>
    [Produces("application/json")]
    [Route("projects/{project_id}/dashboard")]
    public class DashboardController : CommonDashboardController
    {
        private string[] MATERIAL_TYPES = new string[] { "CLT", "Steel", "Facade", "MEP" };

        #region Constructor
        public DashboardController(astorWorkDbContext context, IAstorWorkBlobStorage blobStorage) : base(context)
        {
            _blobStorage = blobStorage;
        }
        #endregion

        #region API methods

        [HttpGet("progress")]
        public async Task<ProjectProgress> GetProjectProgress([FromRoute] int project_id)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

                List<MaterialMaster> materials = await GetMaterials(project_id);
                List<MaterialStageAudit> materialStages = GetMaterialStages(materials);
                MaterialStageMaster lastStage = await GetLastStage();

                string bimVideoUrl = GetLatestBIMSession(project_id);
                List<MaterialOverallProgress> materialOverallProgress = GetMaterialOverallProgress(project_id, materialStages, lastStage, materials.Count());
                List<MaterialInProgress> materialInProgress = GetMaterialInProgress(materialStages, lastStage, materials);

                return new ProjectProgress
                {
                    BimVideoUrl = bimVideoUrl,
                    OverallProgress = materialOverallProgress.Take(10),
                    InProgress = materialInProgress
                };
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// <URL>GET: projects/{projectID}/dashboard/Stats</URL>
        ///  returs you current stats count and progress data for dashboard
        /// </summary>
        /// <param name="project_id"></param>
        /// <returns></returns>
        [HttpGet("stats")]
        public async Task<MaterialStats> GetStats([FromRoute] int project_id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            return await CreateStats(project_id);
        }
        [HttpGet("material-stage-stats")]
        public List<Materialstagedetails> GetmaterialstageStats([FromRoute] int project_id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            return CreateMaterialStageStats(project_id);
        }
        [HttpGet("Readytoprojectstatus")]
        public async Task<List<MaterialDetail>> GetReadytoprojectStats([FromRoute] int project_id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            return await CreateReadytoprojectStats(project_id);
        }

        /// <summary>
        /// <URL>GET: projects/{projectID}/dashboard/qc-open-and-daily-status</URL>
        ///  returs you current day progress and QC fail material data for dashboard
        /// </summary>
        /// <param name="project_id"></param>
        /// <returns></returns>
        [HttpGet("qc-open-and-daily-status")]
        public async Task<QCOpenAndDailyStatus> GetQCOpenMaterialsAndDailyStatus([FromRoute] int project_id)
        {
            await SetMaterialStagesID();

            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            List<MaterialMaster> materials = await GetMaterials(project_id);

            return new QCOpenAndDailyStatus
            {
                DeliveredMaterials = CreateDeliveredInstalledMaterialsList(materials, deliveredStageID),
                InstalledMaterials = CreateDeliveredInstalledMaterialsList(materials, installedStageID),
                QcOpenMaterials = await CreateQCOpenMaterialsList(project_id),
                CompletedMRFs = CreateCompletedMRFList(project_id),
                DailyMaterialStatusCount = await CreateDailyMaterialStatusCounts(project_id)
            };
        }
        [HttpGet("ready-to-delivered-materials")]
        public async Task<QCOpenAndDailyStatus> GetReadyToDeliveredMaterials([FromRoute] int project_id)
        {
            await SetMaterialStagesID();

            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            List<MaterialMaster> materials = await GetMaterials(project_id);

            return new QCOpenAndDailyStatus
            {
                ReadyToDeliveredMaterials = ReadyToDeliverdedMaterialsList(materials, producedStageID, startDeliveryStageID)
            };
        }

        [Route("delayed-materials")]
        public async Task<DelayedMaterials> GetDelayedMaterials([FromRoute] int project_id)
        {
            List<MaterialMaster> materials = await _context.MaterialMaster.Include(mm => mm.MRF).Include(mm => mm.MaterialType)
                                                           .Where(mm => mm.ProjectID == project_id && mm.MRF != null)
                                                           .ToListAsync();

            List<DelayedMaterial> delayedProductionMaterials = new List<DelayedMaterial>();
            List<DelayedMaterial> delayedDeliveryMaterials = new List<DelayedMaterial>();
            List<DelayedMaterial> delayedInstallationMaterials = new List<DelayedMaterial>();
            int index = 0;

            foreach (MaterialMaster material in materials)
            {

                Random rndMode = new Random(12345);
                DelayedMaterial delayedMaterial = CreateDelayedMaterial(material, index, rndMode, MATERIAL_TYPES);

                switch (rndMode.Next(0, 10))
                {
                    case 0:
                        delayedProductionMaterials.Add(delayedMaterial);
                        break;
                    case 1:
                        delayedDeliveryMaterials.Add(delayedMaterial);
                        break;
                    case 2:
                        delayedInstallationMaterials.Add(delayedMaterial);
                        break;
                    default:
                        break;
                }

                index++;
            }

            return new DelayedMaterials
            {
                DelayedProductionMaterials = delayedProductionMaterials,
                DelayedDeliveryMaterials = delayedDeliveryMaterials,
                DelayedInstallationMaterials = delayedInstallationMaterials
            };
        }
        #endregion
    }
}
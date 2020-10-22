using astorWorkDAO;
using astorWorkJobTracking.Common;
using astorWorkJobTracking.Models;
using astorWorkShared.GlobalExceptions;
using astorWorkShared.GlobalResponse;
using astorWorkShared.Services;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static astorWorkShared.Utilities.Enums;
using System.Globalization;
using astorWorkShared.Utilities;

namespace astorWorkJobTracking.Controllers
{
    [Produces("application/json")]
    [Route("projects/{project_id}/job-schedule")]
    [ApiController]
    public class JobScheduleController : CommonController
    {
        protected IAstorWorkImport _importService;

        public JobScheduleController(astorWorkDbContext context, IAstorWorkImport importService)
        {
            _context = context;
            _importService = importService;
        }

        // GET: projects/{projectID}/job-schedule/location?block={block}
        [HttpGet("location")]
        public async Task<List<InstallationLocation>> GetInstallationLocations([FromRoute] int project_id, [FromQuery] string block = "")
        {
            List<BlockInfo> blockInfos = new List<BlockInfo>();
            try
            {
                if (!ModelState.IsValid)
                    throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

                blockInfos = await GetBlockInfoList(project_id, block);

                if (blockInfos == null)
                    throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg("Location", "Block", block));
            }
            catch (Exception ex) { throw ex; }

            return CreateLocationsList(blockInfos);
        }

        protected async Task<List<BlockInfo>> GetBlockInfoList(int projectID, string block = "")
        {
            return await _context.MaterialMaster
                                 .Where(m => m.Project.ID == projectID && block.Contains(m.Block))
                                 .Select(m => new BlockInfo { Level = m.Level, Zone = m.Zone })
                                 .Distinct()
                                 .OrderBy(m => m.Level.PadLeft(3))
                                 .ToListAsync();
        }

        protected List<InstallationLocation> CreateLocationsList(List<BlockInfo> blockInfos)
        {
            List<InstallationLocation> installationLocations = new List<InstallationLocation>();
            try
            {
                if (blockInfos.Count() == 0)
                    return installationLocations;

                InstallationLocation currLevel = null;

                foreach (BlockInfo blockInfo in blockInfos)
                {
                    // New Level
                    if (currLevel == null)
                        currLevel = CreateNewLocation(currLevel, blockInfo.Level, blockInfo.Zone);
                    else if (currLevel.Level != blockInfo.Level)
                    {
                        installationLocations.Add(currLevel);
                        currLevel = CreateNewLocation(currLevel, blockInfo.Level, blockInfo.Zone);
                    }
                    // Zone is within current level
                    else
                        currLevel.Zones.Add(blockInfo.Zone);
                }

                // Add the Level with the corresponding Zones to the Locations list
                installationLocations.Add(currLevel);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return installationLocations;
        }

        protected InstallationLocation CreateNewLocation(InstallationLocation currLevel, string level, string zone)
        {
            currLevel = new InstallationLocation();
            try
            {
                currLevel.Level = level;
                currLevel.Zones = new List<string>();
                currLevel.Zones.Add(zone);
            }
            catch (Exception ex)
            {

                throw ex;
            }


            return currLevel;
        }

        /// <summary>
        /// Used in Web jobschedule tabel binding with lazy loading.
        /// </summary>
        /// <param name="project_id"></param>
        /// <param name="lastMaterialIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="block"></param>
        /// <param name="level"></param>
        /// <param name="materialType"></param>
        /// <param name="subcon"></param>
        /// <returns> jobschedule list</returns>
        [HttpGet("jobschedulelist")]
        public async Task<List<JobScheduleDetails>> ListScheduleJobs([FromRoute] int project_id, [FromQuery] int lastMaterialIndex, [FromQuery] int pageSize, [FromQuery] string block, [FromQuery] string level,
            [FromQuery] string materialType, [FromQuery] string subcon, [FromQuery] int job_Status)
        {
            List<JobScheduleDetails> result = new List<JobScheduleDetails>();
            try
            {
                project_id = await GetProjectID(project_id);
                if (project_id > 0)
                {
                    List<TradeMaterialTypeAssociation> tradeMaterialTypeAssociations = await GetTradeMaterialTypeAssociations();
                    if (tradeMaterialTypeAssociations == null || tradeMaterialTypeAssociations.Count == 0)
                        throw new GenericException(ErrorMessages.DbRecordNotFound, "No job association");

                    List<MaterialMaster> materials = await GetMaterialMasters(project_id);
                    if (materials == null || materials.Count == 0)
                        throw new GenericException(ErrorMessages.DbRecordNotFound, "No materials available");

                    materials = GetMaterialsWithTradeAssociation(tradeMaterialTypeAssociations, materials);
                    if (materials == null || materials.Count == 0)
                        throw new GenericException(ErrorMessages.DbRecordNotFound, "Trade to material type association is missing");
                    if (block != "" && block != null)
                    {
                        materials = GetMaterialsByBlock(materials, block);
                        if (materials == null || materials.Count == 0)
                            throw new GenericException(ErrorMessages.DbRecordNotFound, $"No materials available for Block {block} ");
                    }
                    if (level != "" && level != null)
                    {
                        materials = GetMaterialsByLevel(materials, level);
                        if (materials == null || materials.Count == 0)
                            throw new GenericException(ErrorMessages.DbRecordNotFound, $"No materials available for Level {level} ");

                    }
                    if (materialType != "" && materialType != null)
                    {
                        materials = GetMaterialsByMaterialType(materials, materialType);
                        if (materials == null || materials.Count == 0)
                            throw new GenericException(ErrorMessages.DbRecordNotFound, $"No materials available for Material Type {materialType} ");
                    }

                    List<JobSchedule> existingSchedules = await GetExistingJobSchedules(materials, 0, subcon);

                    if (existingSchedules != null && existingSchedules.Count > 0)
                        result.AddRange(GetJobScheduleDetails(existingSchedules));

                    // Add unassigned jobs
                    if (subcon == null)
                        result = AddUnassignedJobs(materials, tradeMaterialTypeAssociations, result);

                    if (result == null || result.Count == 0)
                        throw new GenericException(ErrorMessages.DbRecordNotFound, $"No materials available for Sub Contractor ");
                }

                if (result == null || result.Count == 0)
                    throw new GenericException(ErrorMessages.DbRecordNotFound, $"No schedule");

                //***** Code for Mobile LazyLoading With Jobstatus Filter *****//
                // -1 for api call from  web  to skip below conditions
                if (job_Status != -1)
                {
                    if (job_Status == 0)//un assigned jobs
                    {
                        result = result.Where(j => j.StatusCode == (int)JobStatus.Job_not_assigned || j.StatusCode == (int)JobStatus.Job_not_scheduled).ToList();
                    }
                    else if (job_Status == 3)//Delayed Jobs
                    {
                        result = result.Where(j => j.StatusCode == (int)JobStatus.Job_delayed).ToList();
                    }
                    else if (job_Status == 4) //Ongoing jobs
                    {
                        result = result.Where(j => j.StatusCode == (int)JobStatus.Job_started).ToList();
                    }
                    else if (job_Status == 5)//Qc Pending Jobs
                    {
                        result = result.Where(j => j.StatusCode == (int)JobStatus.Job_started || j.StatusCode == (int)JobStatus.Job_not_started || j.StatusCode == (int)JobStatus.Job_completed).ToList();
                    }
                    else if (job_Status == 6)//jobs in QC
                    {
                        result = result.Where(j => j.StatusCode > (int)JobStatus.Job_completed && j.StatusCode < (int)JobStatus.All_QC_passed).ToList();
                    }
                    else if (job_Status == 12)//Qc Completed Jobs
                    {
                        result = result.Where(j => j.StatusCode == (int)JobStatus.All_QC_passed).ToList();
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            if (pageSize != 0)
                return result.Skip(lastMaterialIndex).Take(pageSize).ToList();
            else
                return result.ToList();
        }

        /// <summary>
        /// Used in jobschedule dashboard in mobile
        /// </summary>
        /// <returns>jobschedule counts</returns>
        [HttpGet("jobschedule-count")]
        public async Task<JobScheduleCounts> GetJobschedulescounts()
        {
            JobScheduleCounts jobScheduleCounts = new JobScheduleCounts();
            int project_id = await GetProjectID(0);
            List<JobScheduleDetails> result = new List<JobScheduleDetails>();
            if (project_id > 0)
            {
                List<TradeMaterialTypeAssociation> tradeMaterialTypeAssociations = await GetTradeMaterialTypeAssociations();
                if (tradeMaterialTypeAssociations == null || tradeMaterialTypeAssociations.Count == 0)
                    throw new GenericException(ErrorMessages.DbRecordNotFound, "No job association");

                List<MaterialMaster> materials = await GetMaterialMasters(project_id);
                if (materials == null || materials.Count == 0)
                    throw new GenericException(ErrorMessages.DbRecordNotFound, "No materials available");

                materials = GetMaterialsWithTradeAssociation(tradeMaterialTypeAssociations, materials);
                if (materials == null || materials.Count == 0)
                    throw new GenericException(ErrorMessages.DbRecordNotFound, "Trade to material type association is missing");
                List<JobSchedule> existingSchedules = await GetExistingJobSchedules(materials, 0, "");

                if (existingSchedules != null && existingSchedules.Count > 0)
                    result.AddRange(GetJobScheduleDetails(existingSchedules));

                result = AddUnassignedJobs(materials, tradeMaterialTypeAssociations, result);
                if (result.Count > 0)
                {
                    jobScheduleCounts.UnassignedJobs = result.Where(j => j.StatusCode == (int)JobStatus.Job_not_assigned || j.StatusCode == (int)JobStatus.Job_not_scheduled).ToList().Count();
                    jobScheduleCounts.DelayedJobs = result.Where(j => j.StatusCode == (int)JobStatus.Job_delayed).ToList().Count();
                    jobScheduleCounts.OngoingJobs = result.Where(j => j.StatusCode == (int)JobStatus.Job_started).ToList().Count();
                    jobScheduleCounts.QCPendingJobs = result.Where(j => j.StatusCode == (int)JobStatus.Job_started || j.StatusCode == (int)JobStatus.Job_not_started || j.StatusCode == (int)JobStatus.Job_completed).ToList().Count();
                    jobScheduleCounts.JobsinQC = result.Where(j => j.StatusCode > (int)JobStatus.Job_completed && j.StatusCode < (int)JobStatus.All_QC_passed).ToList().Count();
                    jobScheduleCounts.QCCompletedJobs = result.Where(j => j.StatusCode == (int)JobStatus.All_QC_passed).ToList().Count();
                }
            }
            return jobScheduleCounts;

        }
        private async Task<int> GetProjectID(int projectID)
        {
            if (projectID == 0)
            {
                UserMaster user = await _context.GetUserFromHttpContext(HttpContext);
                return user.ProjectID ?? 0;
            }
            return projectID;
        }

        private async Task<List<TradeMaterialTypeAssociation>> GetTradeMaterialTypeAssociations()
        {
            return await _context.TradeMaterialTypeAssociation
                        .Include(tmta => tmta.Trade)
                        .Include(tmta => tmta.MaterialType)
                        .ToListAsync();
        }

        private List<MaterialMaster> GetMaterialsWithTradeAssociation(List<TradeMaterialTypeAssociation> tradeMaterialTypeAssociations, List<MaterialMaster> materials)
        {
            List<MaterialTypeMaster> materialTypes = tradeMaterialTypeAssociations.Select(t => t.MaterialType).Distinct().ToList();
            return materials.Where(mm => materialTypes.Contains(mm.MaterialType)).ToList();
        }

        private async Task<List<MaterialMaster>> GetMaterialMasters(int projectID)
        {
            return await _context.MaterialMaster
                        .Include(mm => mm.MaterialType)
                        .Include(mm => mm.Project)
                        .Where(mm => mm.ProjectID == projectID).ToListAsync();
        }

        private List<MaterialMaster> GetMaterialsByBlock(List<MaterialMaster> materials, string block = "")
        {
            if (!string.IsNullOrEmpty(block))
                materials = materials.Where(mm => mm.Block == block).ToList();

            return materials;
        }

        private List<MaterialMaster> GetMaterialsByLevel(List<MaterialMaster> materials, string level = "")
        {
            if (!string.IsNullOrEmpty(level))
                materials = materials.Where(mm => mm.Level == level).ToList();

            return materials;
        }

        private List<MaterialMaster> GetMaterialsByMaterialType(List<MaterialMaster> materials, string materialType = "")
        {
            if (!string.IsNullOrEmpty(materialType))
                materials = materials.Where(mm => mm.MaterialType.Name == materialType).ToList();

            return materials;
        }

        private async Task<List<JobSchedule>> GetExistingJobSchedules(List<MaterialMaster> materials, int lastMaterialIndex, string subcon = "")
        {
            List<JobSchedule> existingSchedules = await _context.JobSchedule
                                                                .Include(js => js.Material)
                                                                .ThenInclude(mm => mm.MaterialType)
                                                                .Include(js => js.Material.Project)
                                                                .Include(js => js.Trade)
                                                                .Include(js => js.Subcon)
                                                                .Where(js => materials.Contains(js.Material))
                                                                //.Skip(lastMaterialIndex)
                                                                //.Take(100)
                                                                .ToListAsync();

            if (!string.IsNullOrEmpty(subcon))
                existingSchedules = existingSchedules.Where(js => js.Subcon.Name == subcon).ToList();

            return existingSchedules;
        }

        private IEnumerable<JobScheduleDetails> GetJobScheduleDetails(List<JobSchedule> jobSchedules)
        {
            return jobSchedules.Select(js => new JobScheduleDetails
            {
                ID = js.ID,
                MaterialID = js.MaterialID,
                MarkingNo = js.Material.MarkingNo,
                Zone = js.Material.Zone,
                Level = js.Material.Level,
                Block = js.Material.Block,
                MaterialType = js.Material.MaterialType.Name,
                TradeID = js.Trade.ID,
                TradeName = js.Trade.Name,
                SubConID = js.Subcon == null ? 0 : js.Subcon.ID,
                SubConName = js.Subcon == null ? string.Empty : js.Subcon.Name,
                StatusCode = (js.Status == JobStatus.Job_not_started
                                        && js.PlannedStartDate != null
                                        && js.PlannedStartDate.Value.AddMinutes(js.Material.Project.TimeZoneOffset).Date < DateTime.UtcNow.Date) ?
                                           (int)JobStatus.Job_delayed :
                                           (int)js.Status,
                Status = (js.Status == JobStatus.Job_not_started && js.PlannedStartDate != null && js.PlannedStartDate.Value.AddMinutes(js.Material.Project.TimeZoneOffset).Date < DateTime.UtcNow.Date)
                                    ? JobStatus.Job_delayed.ToString()
                                    : js.Status.ToString(),
                ActualStartDate = js.ActualStartDate,
                ActualEndDate = js.ActualEndDate,
                Start = js.PlannedStartDate,
                End = js.PlannedEndDate,
                ProjectID = js.Material.Project.ID,
                ProjectName = js.Material.Project.Name,
                RouteToRTO = !string.IsNullOrEmpty(js.Trade.RouteTo)
            });
        }

        private List<JobScheduleDetails> AddUnassignedJobs(List<MaterialMaster> materials, List<TradeMaterialTypeAssociation> tradeMaterialTypeAssociations, List<JobScheduleDetails> jobSchedules)
        {
            foreach (MaterialMaster material in materials)
            {
                foreach (TradeMaterialTypeAssociation tradeMaterialTypeAssociation in tradeMaterialTypeAssociations.Where(t => t.MaterialType == material.MaterialType))
                {
                    if (jobSchedules.Where(r => r.MaterialID == material.ID && r.TradeID == tradeMaterialTypeAssociation.TradeID).Count() == 0) // check that these are no duplicates
                    {
                        jobSchedules.Add(
                            new JobScheduleDetails
                            {
                                MaterialID = material.ID,
                                MarkingNo = material.MarkingNo,
                                Zone = material.Zone,
                                Level = material.Level,
                                Block = material.Block,
                                MaterialType = material.MaterialType.Name,
                                TradeID = tradeMaterialTypeAssociation.Trade != null ? tradeMaterialTypeAssociation.Trade.ID : 0,
                                TradeName = tradeMaterialTypeAssociation.Trade != null ? tradeMaterialTypeAssociation.Trade.Name : string.Empty,
                                StatusCode = (int)JobStatus.Job_not_assigned,
                                Status = JobStatus.Job_not_assigned.ToString(),
                                ProjectID = material.Project.ID,
                                ProjectName = material.Project.Name,
                                RouteToRTO = !string.IsNullOrEmpty(tradeMaterialTypeAssociation.Trade.RouteTo)
                            }
                        );
                    }
                }
            }

            return jobSchedules;
        }

        //Route: projects/1/job-schedule/by-subcon?subcon_id=1&tag=123
        [HttpGet("by-subcon")]
        public async Task<List<JobScheduleDetails>> ListJobSchedulesBySubcon([FromRoute] int project_id, [FromQuery]int subcon_id, [FromQuery] string tag)
        {
            List<JobSchedule> jobSchedules = new List<JobSchedule>();

            try
            {
                if (!string.IsNullOrEmpty(tag) && subcon_id != 0)
                {
                    MaterialMaster material = _context.TrackerMaster.Include(tm => tm.Material).Where(t => tag == t.Tag).FirstOrDefault().Material;

                    if (material == null)
                        throw new GenericException(ErrorMessages.DbRecordNotFound, "Invalid Tag");

                    jobSchedules = await _context.JobSchedule
                                           .Include(js => js.Trade)
                                           .Include(js => js.Material)
                                           .ThenInclude(m => m.Project)
                                           .Include(js => js.Material)
                                           .ThenInclude(m => m.MaterialType)
                                           .Include(js => js.Material.StageAudits)
                                           .ThenInclude(sa => sa.Stage)
                                           .Include(p => p.Subcon)
                                           .Where(js => material == js.Material && js.SubconID == subcon_id
                                                                 && js.Material.StageAudits != null
                                                                 && js.Material.StageAudits.Count > 0
                                                                 && js.Material.StageAudits.Max(sa => sa.Stage).MilestoneID > 1)
                                           .ToListAsync();
                }
                else if (!string.IsNullOrEmpty(tag) && subcon_id == 0)
                {
                    MaterialMaster material = _context.TrackerMaster.Include(tm => tm.Material).Where(t => tag == t.Tag).FirstOrDefault().Material;

                    if (material == null)
                        throw new GenericException(ErrorMessages.DbRecordNotFound, "Invalid Tag");
                    jobSchedules = await _context.JobSchedule
                                            .Include(js => js.Trade)
                                            .Include(js => js.Material)
                                            .ThenInclude(m => m.Project)
                                            .Include(js => js.Material)
                                            .ThenInclude(m => m.MaterialType)
                                            .Include(js => js.Material.StageAudits)
                                            .ThenInclude(sa => sa.Stage)
                                            .Include(p => p.Subcon)
                                            .Where(js => material == js.Material
                                                                  && js.Material.StageAudits != null
                                                                  && js.Material.StageAudits.Count > 0
                                                                  && js.Material.StageAudits.Max(sa => sa.Stage).MilestoneID > 1)
                                            .ToListAsync();
                }
                else if (string.IsNullOrEmpty(tag) && subcon_id != 0)
                {
                    jobSchedules = await _context.JobSchedule
                                          .Include(js => js.Trade)
                                          .Include(js => js.Material)
                                          .ThenInclude(m => m.Project)
                                          .Include(js => js.Material)
                                          .ThenInclude(m => m.MaterialType)
                                          .Include(js => js.Material.StageAudits)
                                          .ThenInclude(sa => sa.Stage)
                                          .Include(p => p.Subcon)
                                          .Where(js => js.SubconID == subcon_id
                                                    && js.Material.ProjectID == project_id
                                                    && js.Material.StageAudits != null
                                                    && js.Material.StageAudits.Count > 0
                                                    && js.Material.StageAudits.Max(sa => sa.Stage).MilestoneID > 1)
                                          .ToListAsync();
                }

                if (jobSchedules == null || jobSchedules.Count == 0)
                    throw new GenericException(ErrorMessages.DbRecordNotFound, "No assigned jobs");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return CreateJobScheduleDetails(jobSchedules);
        }

        private List<JobScheduleDetails> CreateJobScheduleDetails(List<JobSchedule> jobSchedules)
        {
            List<JobScheduleDetails> jobScheduleDetails = new List<JobScheduleDetails>();

            foreach (JobSchedule jobSchedule in jobSchedules)
            {
                JobScheduleDetails jobScheduleDetail = new JobScheduleDetails
                {
                    ID = jobSchedule.ID,
                    ProjectID = jobSchedule.Material.ProjectID,
                    ProjectName = jobSchedule.Material.Project.Name,
                    MaterialID = jobSchedule.Material.ID,
                    MarkingNo = jobSchedule.Material.MarkingNo,
                    Block = jobSchedule.Material.Block,
                    Level = jobSchedule.Material.Level,
                    Zone = jobSchedule.Material.Zone,
                    MaterialType = jobSchedule.Material.MaterialType.Name,
                    TradeID = jobSchedule.TradeID,
                    TradeName = jobSchedule.Trade.Name,
                    SubConName = jobSchedule.Subcon.Name,
                    SubConID = jobSchedule.Subcon.ID,
                    RouteToRTO = !string.IsNullOrEmpty(jobSchedule.Trade.RouteTo),
                    Start = jobSchedule.PlannedStartDate != null ? jobSchedule.PlannedStartDate.Value.AddMinutes(jobSchedule.Material.Project.TimeZoneOffset) : jobSchedule.PlannedStartDate,
                    End = jobSchedule.PlannedEndDate != null ? jobSchedule.PlannedEndDate.Value.AddMinutes(jobSchedule.Material.Project.TimeZoneOffset) : jobSchedule.PlannedEndDate,
                    StatusCode = (jobSchedule.Status == JobStatus.Job_not_started && jobSchedule.PlannedStartDate != null && jobSchedule.PlannedStartDate.Value.AddMinutes(jobSchedule.Material.Project.TimeZoneOffset).Date < DateTime.UtcNow.Date)
                                ? (int)JobStatus.Job_delayed
                                : (int)jobSchedule.Status,
                    Status = (jobSchedule.Status == JobStatus.Job_not_started && jobSchedule.PlannedStartDate != null && jobSchedule.PlannedStartDate.Value.AddMinutes(jobSchedule.Material.Project.TimeZoneOffset).Date < DateTime.UtcNow.Date)
                                ? JobStatus.Job_delayed.ToString()
                                : jobSchedule.Status.ToString(),
                    ActualStartDate = jobSchedule.ActualStartDate != null ? jobSchedule.ActualStartDate.Value.AddMinutes(jobSchedule.Material.Project.TimeZoneOffset) : jobSchedule.ActualStartDate,
                    ActualEndDate = jobSchedule.ActualEndDate != null ? jobSchedule.ActualEndDate.Value.AddMinutes(jobSchedule.Material.Project.TimeZoneOffset) : jobSchedule.ActualEndDate,
                    RtoInspectionStatusCode=(int)jobSchedule.Trade.RTOInspection
                    
                };

                jobScheduleDetails.Add(jobScheduleDetail);
            }

            return jobScheduleDetails;
        }

        //Route: projects/1/job-schedule/update-status/1?status_id = 3
        [HttpPut("update-status/{id}")]
        public async Task<string> UpdateJobStatus([FromRoute]int id, [FromQuery] int status_id)
        {
            try
            {
                JobSchedule jobSchedule = await _context.JobSchedule
                    .Include(j => j.Material).Where(js => js.ID == id).FirstOrDefaultAsync();
                jobSchedule.Status = (JobStatus)status_id;

                if (status_id == Convert.ToInt32(JobStatus.Job_started))
                    jobSchedule.ActualStartDate = DateTime.UtcNow;

                if (status_id == Convert.ToInt32(JobStatus.Job_completed))
                {
                    jobSchedule.ActualEndDate = DateTime.UtcNow;
                    //  List<UserMaster> receipients = await GetMainContractor(jobSchedule); // Commented and notification sending to MainconQC
                    //   receipients.Add(await _context.GetUserFromHttpContext(HttpContext));

                    List<UserMaster> receipients = await GetMainContractorJobQC(jobSchedule);
                    await UpdateNotificationAudit(receipients, (int)NotificationCode.JobCompleted, 0, jobSchedule.ID.ToString());
                }

                JobAudit jobAudit = new JobAudit
                {
                    JobSchedule = jobSchedule,
                    Status = jobSchedule.Status,
                    CreatedDate = DateTime.Now,
                    CreatedBy = await _context.GetUserFromHttpContext(HttpContext)
                };

                await _context.JobAudit.AddAsync(jobAudit);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new GenericException(ErrorMessages.DbRecordNotFound, "Job update faild");
            }

            return "Sucess!";
        }

        protected async Task<List<UserMaster>> GetMainContractorJobQC(JobSchedule jobSchedule)
        {
            return await _context.UserMaster
                                 .Include(u => u.Organisation)
                                 .Include(u => u.Role)
                                 .Where(u => u.ProjectID == jobSchedule.Material.ProjectID
                                          && u.RoleID == (int)RoleType.MainConQC)
                                 .Distinct()
                                 .ToListAsync();
        }

        //Route: projects/1/job-schedule/assign
        [HttpPut("assign")]
        public async Task<JobSchedule> AssignJob([FromBody] JobScheduleDetails jobScheduleDetail)
        {
            try
            {
                JobSchedule jobSchedule = new JobSchedule
                {
                    MaterialID = jobScheduleDetail.MaterialID,
                    TradeID = jobScheduleDetail.TradeID,
                    SubconID = jobScheduleDetail.SubConID,
                    Status = JobStatus.Job_not_started,
                    PlannedStartDate = jobScheduleDetail.Start,
                    PlannedEndDate = jobScheduleDetail.End
                };

                await _context.JobSchedule.AddAsync(jobSchedule);
                await _context.SaveChangesAsync();

                List<UserMaster> receipients = await GetSubContractor(jobSchedule);
                // receipients.Add(await _context.GetUserFromHttpContext(HttpContext));  // Restrict Multiple mails 
                await UpdateNotificationAudit(receipients, (int)NotificationCode.JobAssigned, 0, jobSchedule.ID.ToString());

                return jobSchedule;
            }
            catch (Exception)
            {
                throw new GenericException(ErrorMessages.DbRecordNotFound, "Job Assignment failed");
            }
        }

        //Route: projects/1/job-schedule
        [HttpPost()]
        public async Task<List<JobScheduleDetails>> SaveListofJobSchedule([FromBody] List<JobScheduleDetails> lstJobScheduleDetails, [FromQuery] int project_id)
        {
            List<JobSchedule> lstInsertJobSchedule = new List<JobSchedule>();
            List<JobSchedule> lstUpdateJobSchedule = new List<JobSchedule>();

            //TODO
            try
            {
                if (!ModelState.IsValid || lstJobScheduleDetails == null || lstJobScheduleDetails.Count < 1)
                    throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

                List<JobScheduleDetails> lstUpdateJobScheduleDetails = lstJobScheduleDetails.Where(js => js.ID > 0 && js.IsUpdated).ToList();

                if (lstUpdateJobScheduleDetails != null && lstUpdateJobScheduleDetails.Count > 0)
                {
                    List<JobSchedule> lstFromTableJobSchedule = _context.JobSchedule.Where(js => lstUpdateJobScheduleDetails.Select(jd => jd.ID).Contains(js.ID)).ToList();
                    foreach (JobScheduleDetails jobScheduleDetail in lstUpdateJobScheduleDetails)
                    {
                        JobSchedule jobSchedule = lstFromTableJobSchedule.Where(js => js.ID == jobScheduleDetail.ID).FirstOrDefault();
                        if (jobSchedule != null)
                        {
                            int subConID = jobScheduleDetail.SubConID;

                            jobSchedule.Subcon = await _context.OrganisationMaster.Where(s => s.ID == subConID).FirstOrDefaultAsync();
                            jobSchedule.PlannedStartDate = jobScheduleDetail.Start;
                            jobSchedule.PlannedEndDate = jobScheduleDetail.End;
                            jobSchedule.Status = jobScheduleDetail.SubConID != 0 ? JobStatus.Job_not_started : JobStatus.Job_not_assigned;
                            jobSchedule.SubconID = subConID;
                            //lstUpdateJobSchedule.Add(jobSchedule);

                            // new subcon assigned
                            //  if (jobSchedule.SubconID != subConID)// to fix jobassign mail notification
                            // {
                            jobSchedule.SubconID = subConID;
                            List<UserMaster> receipients = await GetSubContractor(jobSchedule);
                            receipients.Add(await _context.GetUserFromHttpContext(HttpContext));
                            await UpdateNotificationAudit(receipients, (int)NotificationCode.JobAssigned, 0, jobSchedule.ID.ToString());
                            // }
                            // else
                            //   jobSchedule.SubconID = subConID;
                        }
                    }
                }

                await _context.SaveChangesAsync();

                List<JobScheduleDetails> lstNewJobScheduleDetails = lstJobScheduleDetails.Where(js => js.ID == 0).ToList();
                if (lstNewJobScheduleDetails != null && lstNewJobScheduleDetails.Count > 0)
                {
                    foreach (JobScheduleDetails jobScheduleDetail in lstNewJobScheduleDetails)
                    {
                        JobSchedule jobSchedule = new JobSchedule
                        {
                            MaterialID = jobScheduleDetail.MaterialID,
                            TradeID = jobScheduleDetail.TradeID,
                            SubconID = jobScheduleDetail.SubConID,
                            Status = jobScheduleDetail.SubConID != 0 ? JobStatus.Job_not_started : (JobStatus)jobScheduleDetail.StatusCode,
                            PlannedStartDate = jobScheduleDetail.Start,
                            PlannedEndDate = jobScheduleDetail.End
                        };
                        lstInsertJobSchedule.Add(jobSchedule);
                    }
                }

                if (lstInsertJobSchedule != null || lstInsertJobSchedule.Count > 0)
                {
                    using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
                    {
                        await _context.BulkInsertAsync(lstInsertJobSchedule);
                        transaction.Commit();
                    }
                }
            }
            catch (Exception)
            {
                throw new GenericException(ErrorMessages.DbRecordNotFound, "Job scheduling unsuccessfull");
            }
            return lstJobScheduleDetails;
        }

        [HttpPost("import-JobSchedule")]
        public async Task<List<JobUploadStatus>> ImportJobSchedule(IFormFile file, string project_id)
        {
            List<JobUploadStatus> existingJobs = new List<JobUploadStatus>();
            try
            {
                if (!ModelState.IsValid || string.IsNullOrEmpty(project_id))
                    throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

                IEnumerable<string> jobs = _importService.GetRowsFromFile(file).Distinct();

                List<string> addedjobschedule = new List<string>();

                for (int i = 1; i < jobs.Count(); i++)
                {
                    JobSchedule jobschedulelist = CreateJobSchedule(jobs.ElementAt(i), int.Parse(project_id));
                    if (JobScheduleExists(jobschedulelist))
                    {
                        UpdateJobSchedule(jobschedulelist, int.Parse(project_id));
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        if (jobschedulelist.MaterialID != 0 && jobschedulelist.TradeID != 0)
                            _context.JobSchedule.Add(jobschedulelist);
                        else
                            existingJobs = AddJobScheduleUploadStatus(existingJobs, jobs.ElementAt(i), " No Material found for the following  Block,Level,Zone and Trade with Project ID :" + project_id + "");
                    }

                }

                await _context.SaveChangesAsync();
            }
            catch (Exception) { }

            if (existingJobs.Count == 0)
                return new List<JobUploadStatus>();
            else
                return existingJobs;
        }

        private List<JobUploadStatus> AddJobScheduleUploadStatus(List<JobUploadStatus> existingJobs, string jobscheduleinfo, string Message)
        {

            JobUploadStatus jobUploadStatus = new JobUploadStatus();
            string[] jobAttributes = jobscheduleinfo.Trim().Split(',');

            jobUploadStatus.Message = Message;
            jobUploadStatus.Block = jobAttributes[0];
            jobUploadStatus.Level = jobAttributes[1];
            jobUploadStatus.Zone = jobAttributes[2];
            jobUploadStatus.MaterialType = jobAttributes[3];
            jobUploadStatus.Name = jobAttributes[4];
            existingJobs.Add(jobUploadStatus);
            return existingJobs;
        }

        protected bool JobScheduleExists(JobSchedule jobschedule)
        {
            return _context.JobSchedule.Any(j => j.MaterialID == jobschedule.MaterialID && j.TradeID == jobschedule.TradeID);
        }

        protected JobSchedule CreateJobSchedule(string jobscheduleinfo, int project_id)
        {
            JobSchedule jobSchedulelist = new JobSchedule();
            try
            {
                string[] jobAttributes = jobscheduleinfo.Trim().Split(',');
                MaterialTypeMaster materialTypeMaster = _context.MaterialTypeMaster.Where(mt => mt.Name == jobAttributes[3]).FirstOrDefault();

                if (materialTypeMaster.ID > 0)
                {
                    MaterialMaster materialMaster = _context.MaterialMaster
                                                            .Where(m => m.Block == jobAttributes[0]
                                                                     && m.Level == jobAttributes[1]
                                                                     && m.Zone == jobAttributes[2]
                                                                     && m.MaterialType.ID == materialTypeMaster.ID
                                                                     && m.ProjectID == project_id)
                                                            .FirstOrDefault();

                    if (materialMaster.ID > 0)
                        jobSchedulelist.MaterialID = materialMaster.ID;
                }

                TradeMaster tradeMaster = _context.TradeMaster
                                                  .Where(t => t.Name == jobAttributes[4])
                                                  .FirstOrDefault();

                if (tradeMaster.ID > 0)
                    jobSchedulelist.TradeID = tradeMaster.ID;

                if (jobAttributes[5].Length > 0)
                {
                    OrganisationMaster organisation = _context.OrganisationMaster
                                                              .Where(o => o.Name == jobAttributes[5].Trim()
                                                                       && o.OrganisationType == OrganisationType.Subcon)
                                                              .FirstOrDefault();

                    if (organisation.ID > 0)
                    {
                        jobSchedulelist.SubconID = organisation.ID;
                        jobSchedulelist.Status = JobStatus.Job_not_started;
                    }
                }
                else
                {
                    jobSchedulelist.Subcon = null;
                    jobSchedulelist.Status = JobStatus.Job_not_assigned;
                }

                if (jobAttributes[6].Length > 0)
                    if (!string.IsNullOrEmpty(jobAttributes[6]))
                    {
                        string[] arr = new string[6];
                        DateTime dateTime;
                        //user may choose any date format in Jobschedule template while uploading,so converting any date format from Jobschedule template in datetime format (MM/dd/yyyy)
                        DateTime.TryParseExact(jobAttributes[6], new string[6] { "MM/dd/yyyy", "dd/MM/yyyy", "MM-dd-yyyy", "dd-MM-yyyy", "M/d/yyyy", "M/dd/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
                        jobSchedulelist.PlannedStartDate = dateTime;
                    }

                if (jobAttributes[7].Length > 0)
                    if (!string.IsNullOrEmpty(jobAttributes[7]))
                    {

                        string[] arr = new string[6];
                        DateTime dateTime;
                        DateTime.TryParseExact(jobAttributes[7], new string[6] { "MM/dd/yyyy", "dd/MM/yyyy", "MM-dd-yyyy", "dd-MM-yyyy", "M/d/yyyy", "M/dd/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
                        jobSchedulelist.PlannedEndDate = dateTime;
                    }

            }
            catch (Exception)
            {

            }
            return jobSchedulelist;
        }

        protected void UpdateJobSchedule(JobSchedule jobSchedules, int project_id)
        {
            JobSchedule jobSchedule = _context.JobSchedule
                                              .Where(j => j.MaterialID == jobSchedules.MaterialID
                                                       && j.TradeID == jobSchedules.TradeID)
                                              .FirstOrDefault();

            jobSchedule.SubconID = jobSchedules.SubconID;
            jobSchedule.PlannedStartDate = jobSchedules.PlannedStartDate;
            jobSchedule.PlannedEndDate = jobSchedules.PlannedEndDate;

            _context.Entry(jobSchedule).State = EntityState.Modified;
        }

        [HttpGet("jobs-by-rto")]
        public async Task<List<JobScheduleDetails>> JobSchedulesbyRTO()
        {
            List<JobScheduleDetails> jobScheduleDetails = new List<JobScheduleDetails>();
            try
            {
                UserMaster user = await _context.GetUserFromHttpContext(HttpContext);
                jobScheduleDetails = _context.ChecklistAudit
                     .Include(ca => ca.JobSchedule)
                     .Include(ca => ca.JobSchedule.Material)
                     .ThenInclude(mm => mm.MaterialType)
                     .Include(ca => ca.JobSchedule.Trade)
                     .Include(ca => ca.JobSchedule.Subcon)
                     .Include(ca => ca.JobSchedule.Material.Project)
                     .Where(ca => ca.Status >= QCStatus.QC_routed_to_RTO
                               && ca.JobSchedule != null
                               && ca.JobSchedule.Status >= JobStatus.QC_routed_to_RTO
                               && ca.RouteTo.ID == user.ID)
                     .Select(list => new JobScheduleDetails
                     {
                         Block = list.JobSchedule.Material.Block,
                         Level = list.JobSchedule.Material.Level,
                         Zone = list.JobSchedule.Material.Zone,
                         MaterialType = list.JobSchedule.Material.MaterialType.Name,
                         TradeName = (list.JobSchedule.Trade.Name.Length > 0) ? list.JobSchedule.Trade.Name : list.MaterialStageAudit.Stage.Name,
                         SubConName = list.JobSchedule.Subcon.Name,
                         Start = list.JobSchedule.PlannedStartDate,
                         End = list.JobSchedule.PlannedEndDate,
                         ActualStartDate = list.JobSchedule.ActualStartDate,
                         ActualEndDate = list.JobSchedule.ActualEndDate,
                         ProjectID = list.JobSchedule.Material.ProjectID,
                         ProjectName = list.JobSchedule.Material.Project.Name,
                         Status = list.JobSchedule.Status.ToString(),
                         StatusCode = (int)list.JobSchedule.Status,
                         ID = list.JobSchedule.ID,
                         TradeID = list.JobSchedule.Trade.ID,
                         SubConID = list.JobSchedule.Subcon.ID,
                         MarkingNo = list.JobSchedule.Material.MarkingNo,
                         MaterialID = list.JobSchedule.MaterialID
                     })
                     .ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return jobScheduleDetails;
        }

        [HttpGet("jobs-by-subcon")]
        public async Task<List<JobScheduleDetails>> GetJobSchedulesbySubCon()
        {
            List<JobScheduleDetails> jobScheduleDetails = new List<JobScheduleDetails>();

            try
            {
                UserMaster user = await _context.GetUserFromHttpContext(HttpContext);
                jobScheduleDetails = _context.ChecklistAudit
                     .Include(ca => ca.JobSchedule)
                     .Include(ca => ca.JobSchedule.Material)
                     .ThenInclude(mm => mm.MaterialType)
                     .Include(ca => ca.JobSchedule.Trade)
                     .Include(ca => ca.JobSchedule.Subcon)
                     .Include(ca => ca.JobSchedule.Material.Project).Where(ca => ca.JobSchedule.Status == JobStatus.QC_failed_by_Maincon
                                                                                                    && ca.JobSchedule != null
                                                                                                    && ca.JobSchedule.SubconID == user.ID)
                     .Select(list => new JobScheduleDetails
                     {
                         Block = list.JobSchedule.Material.Block,
                         Level = list.JobSchedule.Material.Level,
                         Zone = list.JobSchedule.Material.Zone,
                         MaterialType = list.JobSchedule.Material.MaterialType.Name,
                         TradeName = list.JobSchedule.Trade.Name,
                         SubConName = list.JobSchedule.Subcon.Name,
                         Start = list.JobSchedule.PlannedStartDate,
                         End = list.JobSchedule.PlannedEndDate,
                         ActualStartDate = list.JobSchedule.ActualStartDate,
                         ActualEndDate = list.JobSchedule.ActualEndDate,
                         ProjectID = list.JobSchedule.Material.ProjectID,
                         ProjectName = list.JobSchedule.Material.Project.Name,
                         Status = list.JobSchedule.Status.ToString(),
                         StatusCode = (int)list.JobSchedule.Status,
                         ID = list.JobSchedule.ID,
                         TradeID = list.JobSchedule.Trade.ID,
                         SubConID = list.JobSchedule.Subcon.ID,
                         MarkingNo = list.JobSchedule.Material.MarkingNo,
                         MaterialID = list.JobSchedule.MaterialID
                     }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return jobScheduleDetails;
        }

        [HttpGet("jobs-by-materialtag")]
        public async Task<List<JobScheduleDetails>> GetJobsbyMaterial([FromQuery] string[] tags)
        {
            List<JobScheduleDetails> JobScheduleDetails = new List<JobScheduleDetails>();
            try
            {
                if (!ModelState.IsValid)
                    throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());
                List<int> trackerIDs = GetTrackerIDs(tags);//tags1
                List<MaterialMaster> materials = GetMaterials(trackerIDs);

                JobScheduleDetails = await _context.JobSchedule
                  .Include(j => j.Trade)
                  .Include(j => j.Material)
                  .ThenInclude(m => m.MaterialType)
                  .Include(j => j.Material.Project)
                  .Include(j => j.Subcon)
                  .Where(j => materials.Contains(j.Material))
                  .Select(list => new JobScheduleDetails
                  {
                      Block = list.Material.Block,
                      Level = list.Material.Level,
                      Zone = list.Material.Zone,
                      MaterialType = list.Material.MaterialType.Name,
                      TradeName = list.Trade.Name,
                      SubConName = list.Subcon.Name,
                      Start = list.PlannedStartDate,
                      RouteToRTO = !string.IsNullOrEmpty(list.Trade.RouteTo),
                      End = list.PlannedEndDate,
                      ActualStartDate = list.ActualStartDate,
                      ActualEndDate = list.ActualEndDate,
                      ProjectID = list.Material.ProjectID,
                      ProjectName = list.Material.Project.Name,
                      Status = list.Status.ToString(),
                      StatusCode = (int)list.Status,
                      ID = list.ID,
                      TradeID = list.Trade.ID,
                      SubConID = list.Subcon.ID,
                      MarkingNo = list.Material.MarkingNo,
                      MaterialID = list.MaterialID,
                      RtoInspectionStatusCode=(int)list.Trade.RTOInspection
                  }).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return JobScheduleDetails;
        }

        private List<MaterialMaster> GetMaterials(List<int> trackerIDs)
        {
            return _context.MaterialMaster
                   .Include(mm => mm.Project)
                   .Include(mm => mm.Trackers)
                   .Include(mm => mm.MRF)
                   .Include(mm => mm.Organisation)
                   .Include(mm => mm.QCCases)
                   .ThenInclude(qc => qc.Defects)
                   .Include(mm => mm.StageAudits)
                   .ThenInclude(sa => sa.Location)
                   .Include(mm => mm.StageAudits)
                   .ThenInclude(sa => sa.Stage)
                   .Include(mm => mm.Elements)
                   .ThenInclude(elm => elm.ForgeModel)
                   .Include(mm => mm.MaterialType)
                   .Where(mm => mm.Trackers.Where(t => trackerIDs.Contains(t.ID) && t.Material != null).Count() > 0)
                   .ToList();
        }

        private List<int> GetTrackerIDs(string[] tags)
        {
            return _context.TrackerMaster
                           .Where(tm => tags.Contains(tm.Tag))
                           .Select(t => t.ID)
                           .ToList();
        }
    }
}
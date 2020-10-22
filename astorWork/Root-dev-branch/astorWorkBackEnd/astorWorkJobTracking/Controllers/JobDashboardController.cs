using astorWorkDAO;
using astorWorkJobTracking.Common;
using astorWorkJobTracking.Models;
using astorWorkShared.GlobalExceptions;
using astorWorkShared.GlobalResponse;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static astorWorkShared.Utilities.Enums;

namespace astorWorkJobTracking.Controllers
{
    [Produces("application/json")]
    [Route("projects/{project_id}/job-dashboard")]
    [ApiController]
    public class JobDashboardController : CommonController
    {
        protected astorWorkDbContext _context;
        protected IAstorWorkImport _importService;
        double progress = 0;
        public JobDashboardController(astorWorkDbContext context, IAstorWorkImport importService, IAstorWorkBlobStorage blobStorage)
        {
            _context = context;
            _importService = importService;
            _blobStorage = blobStorage;
        }
       
        [HttpGet("job-status")]
        public async Task<JobStats> GetStats([FromRoute] int project_id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            return await CreateStatus(project_id);
        }
      
        protected async Task<JobStats> CreateStatus(int projectID)
        {
            JobStats Jobstatus = null;
            try
            {
                List<JobSchedule> Jobs = new List<JobSchedule>();
                Jobs = await _context.JobSchedule
                    .Include(j => j.Material)
                    .ThenInclude(m => m.Project).ToListAsync();
                int ScheduledJobscount = Jobs.Where(j => j.Material.ProjectID == projectID).Count();
                int compltedjobscount = Jobs.Where( d => d.ActualEndDate != null && d.Material.ProjectID == projectID).Count();
                int StartedJobscount = Jobs.Where(j => j.Status == JobStatus.Job_started && j.Material.ProjectID == projectID).Count();
                int DelayedJobscount = Jobs.Where(j => j.Status == JobStatus.Job_not_started && j.PlannedStartDate != null && j.PlannedStartDate.Value.AddMinutes(j.Material.Project.TimeZoneOffset).Date < DateTime.UtcNow.Date && j.Material.ProjectID == projectID).Count();

                Jobstatus = new JobStats
                {
                    ScheduledJobsCount = ScheduledJobscount,
                    CompletedJobsCount = compltedjobscount,
                    StartedJobsCount = StartedJobscount,
                    DelayedJobsCount = DelayedJobscount,
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Jobstatus;
        }

        protected double GetProgress(double stageCount, double total)
        {
            if (total > 0)
            {
                double percent = stageCount / total * 100;
                progress = Math.Round(percent, 2);
                return Math.Round(percent, 2);
            }
            return 0;
        }

        [HttpGet("job-statuslist")]
        public async Task<JobstatusList> GetAllJobs([FromRoute] int project_id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            List<JobSchedule> Jobs = await GetJobs(project_id);

            return new JobstatusList
            {
                startedJobs = CreateJobsList(Jobs.Where(j => j.Status == JobStatus.Job_started).ToList()),
                compltedJobs = CreateJobsList(Jobs.Where(d => d.ActualEndDate != null).ToList()),
            delayedJobs = CreateJobsList(Jobs.Where(j => j.Status == JobStatus.Job_not_started && j.PlannedStartDate != null && j.PlannedStartDate.Value.AddMinutes(j.Material.Project.TimeZoneOffset).Date < DateTime.UtcNow.Date).ToList()),
            };
        }
 
        protected async Task<List<JobSchedule>> GetJobs(int projectID)
        {
            return await _context.JobSchedule
                           .Include(j => j.Material)
                           .Include(j => j.Trade)
                           .Include(j => j.Subcon)
                           .Include(j => j.Material.Project)
                           .Include(j=>j.Material.MaterialType)
                           .Where(j => j.Material.ProjectID == projectID)
                           .ToListAsync();
        }

        protected List<JobScheduleDetails> CreateJobsList(List<JobSchedule> Jobs)
        {
            List<JobScheduleDetails> JobsList = new List<JobScheduleDetails>();
            JobsList = Jobs.Take(10).Select(j => new JobScheduleDetails()
            {
                TradeName = j.Trade.Name,
                MarkingNo = $"{j.Material.Block}-L{j.Material.Level}-{j.Material.Zone}-{j.Material.MaterialType.Name}",
                SubConName = j.Subcon.Name,
                PlannedStartDate = j.PlannedStartDate,
                ActualStartDate = j.ActualStartDate,
                ActualEndDate = j.ActualEndDate,
                ID=j.ID,
            }).OrderByDescending(j => j.LastModifiedDate).ToList();
            return JobsList;
        }

      

        protected JobScheduleDetails CreatejobDetail(JobSchedule job)
        {
            JobScheduleDetails jobs = new JobScheduleDetails();
            jobs.TradeName = job.Trade.Name;
            jobs.MarkingNo = $"{job.Material.Block},Level  {job.Material.Level},Zone{job.Material.Zone}";
            jobs.SubConName = job.Subcon.Name;
            jobs.PlannedStartDate = job.PlannedStartDate;
            jobs.ActualStartDate = job.ActualStartDate;
            jobs.ActualEndDate = job.ActualEndDate;
            return jobs;
        }

        [HttpGet("daily-job-status")]
        public async Task<JobStats> GetDailyJobStatus([FromRoute] int project_id)
        {
            JobStats jobStatus = new JobStats();
            try
            {
                if (!ModelState.IsValid)
                    throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());
                var dailyJobStatus = await _context.JobSchedule
                     .Include(j => j.Material)
                     .Include(j => j.Material.Project)
                     .Where(sa => sa.Material.ProjectID == project_id && sa.PlannedStartDate != null)
                     .ToListAsync();

                jobStatus.ScheduledCount = dailyJobStatus.Where(d => d.PlannedStartDate != null && d.PlannedStartDate.Value.AddMinutes(d.Material.Project.TimeZoneOffset).Date == DateTime.UtcNow.Date).Count();
                jobStatus.OngoingCount = dailyJobStatus.Where(d =>d.Status ==JobStatus.Job_started && d.ActualStartDate != null && d.ActualStartDate.Value.AddMinutes(d.Material.Project.TimeZoneOffset).Date == DateTime.UtcNow.Date).Count();
                jobStatus.CompletedCount = dailyJobStatus.Where(d => d.ActualEndDate != null && d.ActualEndDate.Value.AddMinutes(d.Material.Project.TimeZoneOffset).Date == DateTime.UtcNow.Date).Count();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return jobStatus;

        }

        [HttpGet("overall-job-progress")]
        public async Task<ProjectProgress> GetOverallJobProgress([FromRoute] int project_id)
        {

            List<JobSchedule> listJobSchedules = await _context.JobSchedule
                                           .Include(j => j.Material.Project)
                                           .Where(j => j.Material.ProjectID == project_id && j.PlannedStartDate != null)
                                           .ToListAsync();
    
            List<JobOverallProgress> listJobOverallProgress = GetallJobProgress(listJobSchedules, listJobSchedules.Count());
            List<JobOverallProgress> jobOverallProgress = AddProjectStartProgress(project_id);
            List<JobOverallProgress> finalistOveralljobprogress= AddProgressByDate(jobOverallProgress, listJobOverallProgress);
            return new ProjectProgress
            {
                OverallProgress= finalistOveralljobprogress.OrderBy(j => j.Date).Take(7)

            };
        }

        protected List<JobOverallProgress> GetallJobProgress(IEnumerable<JobSchedule> listJobSchedules, int totalScheduleCount)
        {
            progress = 0;
            return listJobSchedules.Where(j => j.Status == JobStatus.Job_completed)
                .OrderBy(j => j.ActualEndDate)
                .GroupBy(j => DateTimeOffset.Parse(j.ActualEndDate.ToString()).Date)
                .Select(m => new JobOverallProgress
                {
                    Date = m.Key.Date,
                    Progress = GetProgress(m.Count(), totalScheduleCount),
                }).ToList();

        }
        protected List<JobOverallProgress> AddProjectStartProgress(int projectID)
        {
            List<JobOverallProgress> jobprogress = new List<JobOverallProgress>();
            jobprogress.Add(new JobOverallProgress
            {
                Date = _context.ProjectMaster
                        .Where(p => p.ID == projectID).FirstOrDefault().EstimatedStartDate,
                Progress = 0
            });

            return jobprogress;
        }
        protected List<JobOverallProgress> AddProgressByDate(List<JobOverallProgress> jobOverallProgress, List<JobOverallProgress> progressByDate)
        {
            double progress = 0;
            foreach (JobOverallProgress jop in progressByDate)
            {
                progress += jop.Progress;
                jop.Progress = Math.Round(progress,2);
                jobOverallProgress.Add(jop);
            }
            return jobOverallProgress;
        }

        //Getting all the jobs having checklists but which are not pending status
        [HttpGet("archiqc-checklists-jobs")]
        public List<JobQCDetails> GetQCJobslist([FromRoute] int project_id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());
            List<ChatData> chats = _context.ChatData.Where(s => s.HasAttachment).ToList();
            List<JobQCDetails> qcJobs = ListQCJobs(project_id, chats);
            return qcJobs;
        }
        [HttpGet("struct-checklist")]
        public List<JobQCDetails> GetQCmaterialslist([FromRoute] int project_id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());
            List<JobQCDetails> QCFailedMaterials = ListQCFailedMaterials(project_id);
            return QCFailedMaterials;
        }
        protected List<JobQCDetails> ListQCFailedMaterials(int project_id)
        {
            List<JobQCDetails> QCFailedMaterials = new List<JobQCDetails>();
            List<JobQCDetails> result = new List<JobQCDetails>();
            try
            {
                List<ChatData> chats = _context.ChatData.Where(s => s.HasAttachment).ToList();
                QCFailedMaterials = _context.ChecklistAudit
                              .Include(j => j.Checklist)
                              .Include(c => c.CreatedBy)
                              .Include(j => j.MaterialStageAudit)
                              .ThenInclude(j => j.MaterialMaster)
                              .ThenInclude(j => j.MaterialType)
                              .Include(c => c.MaterialStageAudit.Stage)
                             .Where(j => j.JobScheduleID == null && j.MaterialStageAudit != null && j.MaterialStageAudit.MaterialMaster.ProjectID == project_id
                        )
                .Select(list => new JobQCDetails
                {
                    Block = list.MaterialStageAudit.MaterialMaster.Block,
                    Level = list.MaterialStageAudit.MaterialMaster.Level,
                    Zone = list.MaterialStageAudit.MaterialMaster.Zone,
                    CheckListName = list.Checklist.Name,
                    QcFailedBy = list.CreatedBy.PersonName,
                    QCStartDate = list.CreatedDate,
                    type = list.MaterialStageAudit.MaterialMaster.MaterialType.Name,
                    MarkingNo = $"{list.MaterialStageAudit.MaterialMaster.Block}-{list.MaterialStageAudit.MaterialMaster.Level}-{list.MaterialStageAudit.MaterialMaster.Zone}-{list.MaterialStageAudit.MaterialMaster.MarkingNo}",
                    status = list.MaterialStageAudit.QCStatus.ToString(),
                    ChecklistID = list.ChecklistID,
                    ChecklistAuditID = list.ID,
                    ID = list.MaterialStageAudit.ID,
                    StageID = list.MaterialStageAudit.StageID,
                    StageName = list.MaterialStageAudit.Stage.Name,
                    ChecklistStatus = list.Status.ToString()
                }).Distinct().ToList();

                result = QCFailedMaterials.GroupBy(s => new { s.MarkingNo, s.ProjectID, s.CheckListName, s.ChecklistID, s.ID,  s.subConName, s.Block, s.Level, s.Zone, s.type,s.StageID,s.StageName })
                   .Select(list => new JobQCDetails
                   {
                      
                       MarkingNo = list.Key.MarkingNo,
                       type = list.Key.type,
                       PlannedStartDate = list.FirstOrDefault().PlannedStartDate,
                       plannedEndDate = list.FirstOrDefault().plannedEndDate,
                       ProjectID = list.Key.ProjectID,
                       QCStartDate = list.FirstOrDefault().QCStartDate,
                       qcEndDate = list.OrderByDescending(s => s.QCStartDate).FirstOrDefault().QCStartDate,
                       CheckListName = list.Key.CheckListName,
                       QcFailedBy = list.OrderByDescending(s => s.QCStartDate).FirstOrDefault().QcFailedBy,
                       ChecklistStatus = list.OrderByDescending(s => s.QCStartDate).FirstOrDefault().ChecklistStatus,
                       status=list.FirstOrDefault().status,
                       ID = list.Key.ID,
                       ChecklistID = list.Key.ChecklistID,
                       countPhotos = chats.Where(s => s.ChecklistID == list.Key.ChecklistID && s.MaterialStageAuditID == list.Key.ID).Count(),
                       subConName = list.Key.subConName,
                       TotalQCCount = QCFailedMaterials.Count(),
                       Block = list.Key.Block,
                       Level = list.Key.Level,
                       Zone = list.Key.Zone,
                       StageID=list.Key.StageID,
                       StageName=list.Key.StageName
                   }).Distinct().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        protected List<JobQCDetails> ListQCJobs(int project_id,List<ChatData> chats)
        {
            List<JobQCDetails> QCJobs = new List<JobQCDetails>();
            List<JobQCDetails> result=new List<JobQCDetails>();
            try
            {
                QCJobs = _context.ChecklistAudit
                           .Include(c => c.JobSchedule)
                           .ThenInclude(j => j.Subcon)
                           .Include(c => c.JobSchedule.Material)
                           .ThenInclude(m => m.MaterialType)
                           .Include(c => c.JobSchedule.Material.Project)
                           .Include(c => c.CreatedBy)
                           .Include(j => j.Checklist)
                           .Where(j => j.JobScheduleID != null && j.JobSchedule.Material.ProjectID == project_id
                           )
                .Select(list => new JobQCDetails
                {
                    TradeName = list.JobSchedule.Trade.Name,
                    MarkingNo = $"{list.JobSchedule.Material.Block}-L{list.JobSchedule.Material.Level}-{list.JobSchedule.Material.Zone}-{list.JobSchedule.Material.MaterialType.Name}",
                    PlannedStartDate = list.JobSchedule.ActualStartDate,
                    plannedEndDate = list.JobSchedule.ActualEndDate,
                    ProjectID = list.JobSchedule.Material.ProjectID,
                    ChecklistID = list.ChecklistID,
                    QCStartDate = list.CreatedDate,
                    subConName = list.JobSchedule.Subcon.Name,
                    CheckListName = list.Checklist.Name,
                    QcFailedBy = list.CreatedBy.PersonName,
                    status = list.JobSchedule.Status.ToString(),
                    ChecklistStatus = list.Status.ToString(),
                    TotalQCCount=QCJobs.Count(),
                    ID = list.JobSchedule.ID,
                    ChecklistAuditID = list.ID,
                    TradeID=list.JobSchedule.Trade.ID,
                    Block=list.JobSchedule.Material.Block,
                    Level=list.JobSchedule.Material.Level,
                    Zone=list.JobSchedule.Material.Zone,
                
                }).Distinct().ToList();
                result = QCJobs.GroupBy(s => new { s.TradeName, s.ProjectID, s.CheckListName, s.ChecklistID, s.ID, s.TradeID,s.subConName })
                    .Select(list => new JobQCDetails
                    {
                        TradeName = list.Key.TradeName,
                        MarkingNo = list?.FirstOrDefault()?.MarkingNo,
                        PlannedStartDate = list.FirstOrDefault().PlannedStartDate,
                        plannedEndDate = list.FirstOrDefault().plannedEndDate,
                        ProjectID = list.Key.ProjectID,
                        QCStartDate = list.OrderBy(s=>s.QCStartDate).FirstOrDefault().QCStartDate,
                        qcEndDate = list.OrderByDescending(s => s.QCStartDate).FirstOrDefault().QCStartDate,
                        CheckListName = list.Key.CheckListName,
                        QcFailedBy = list.OrderByDescending(s => s.QCStartDate).FirstOrDefault().QcFailedBy,
                        ChecklistStatus = list.OrderByDescending(s => s.QCStartDate).FirstOrDefault().ChecklistStatus,
                        status=list.FirstOrDefault().status,
                        ID = list.Key.ID,
                        ChecklistID = list.Key.ChecklistID,
                        TradeID = list.Key.TradeID,
                        countPhotos =chats.Where(s => s.ChecklistID == list.Key.ChecklistID && s.JobScheduleID == list.Key.ID).Count(),
                        subConName=list.Key.subConName,
                        TotalQCCount = QCJobs.Count(),
                        Block=list?.FirstOrDefault()?.Block,
                        Level=list?.FirstOrDefault()?.Level,
                        Zone=list?.FirstOrDefault()?.Zone
                    }).Distinct().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }


        [HttpGet("photo")]
        public IEnumerable<JobQCDetails> GetArchiQCchecklistPhotos([FromRoute] int project_id, [FromQuery] int defect_id,[FromQuery] int jobscheduleID,[FromQuery] int MaterialStageAuditID)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());
            List<JobQCDetails> JobQCPhotos = new List<JobQCDetails>();
            try
            {
                string token = _blobStorage.GetContainerAccessToken();
                if (jobscheduleID != 0)
                {
                    JobQCPhotos = GetJobQCPhotos(defect_id, token, project_id, jobscheduleID,MaterialStageAuditID);
                }
                else
                {
                    JobQCPhotos = GetJobQCPhotos(defect_id, token, project_id, jobscheduleID,MaterialStageAuditID);
                }
                if (JobQCPhotos == null)
                    throw new GenericException(ErrorMessages.DbRecordNotFound, "No Photo(s) found for this defect!");
            }

            catch(Exception ex)
            {
                throw ex;
            }
            return JobQCPhotos;
        }

        protected List<JobQCDetails> GetJobQCPhotos(int defect_id,string token,int project_id,int jobscheduleID,int MaterialStageAuditID)
        {
            List<JobQCDetails> QCphotos = new List<JobQCDetails>();
            try
            {

                if (jobscheduleID != 0)
                {
                    QCphotos = _context.ChatData
                                .Include(j => j.JobSchedule)
                                .Include(j => j.Checklist)
                                .Include(c => c.ChecklistItem)
                                .Include(j => j.User)
                                .Where(j => j.ChecklistID == defect_id && j.JobSchedule.Material.ProjectID == project_id && j.ThumbnailUrl != null && j.JobScheduleID == jobscheduleID)
                               .Select(list => new JobQCDetails
                               {
                                   ChecklistID = list.Checklist.ID,
                                   ID = list.ID,
                                   Remarks = list.Message,
                                   URL = !string.IsNullOrEmpty(list.OriginalAttachmentUrl) ? list.OriginalAttachmentUrl + token : list.ThumbnailUrl + token,
                                   CreatedDate = list.TimeStamp,
                                   CreatedBy = list.User.UserName
                               }).Distinct().ToList();
                }
                else
                {
                    QCphotos = _context.ChatData
                                .Include(j => j.JobSchedule)
                                .Include(j => j.Checklist)
                                .Include(c => c.ChecklistItem)
                                .Include(j => j.User)
                                .Where(j => j.ChecklistID == defect_id && j.MaterialStageAudit.MaterialMaster.Project.ID == project_id && j.ThumbnailUrl != null && j.MaterialStageAuditID == MaterialStageAuditID)
                               .Select(list => new JobQCDetails
                               {
                                   ChecklistID = list.Checklist.ID,
                                   ID = list.ID,
                                   Remarks = list.Message,
                                   URL = !string.IsNullOrEmpty(list.OriginalAttachmentUrl) ? list.OriginalAttachmentUrl + token : list.ThumbnailUrl + token,
                                   CreatedDate = list.TimeStamp,
                                   CreatedBy = list.User.UserName
                               }).Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return QCphotos;
        }

    }
}
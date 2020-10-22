using astorWorkDAO;
using astorWorkMaterialTracking.Common;
using astorWorkMaterialTracking.Models;
using astorWorkShared.GlobalExceptions;
using astorWorkShared.GlobalResponse;
using astorWorkShared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Controllers
{
    [Produces("application/json")]
    [Route("notification-timer")]
    [ApiController]
    public class NotificationTimerController : CommonNotificationTimerController
    {
        public NotificationTimerController(astorWorkDbContext context) : base(context)
        {
            _context = context;
        }

        // GET: notification-timer/5?type=project
        [HttpGet("{id}")]
        public List<Notification> GetTimer(int id, [FromQuery]string type)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

                List<Notification> notifications = GetNotificationDetails(id, type);
                List<NotificationTimerMaster> timers = GetTimers(id, type);

                if ((timers != null && timers.Count != 0))
                    notifications = SetNotifications(notifications, timers);

                return notifications;
            }
            catch (Exception ex)
            {
                throw new GenericException(500, ex.Message);
            }
        }

        // POST: notification-timer
        [HttpPost]
        public async Task<List<NotificationTimerMaster>> AddUpdateNotifications([FromBody] List<Notification> notifications)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            try
            {
                List<NotificationTimerMaster> timers = GetTimers(notifications);
                foreach (Notification notification in notifications)
                {
                    if (notification.ID == 0)
                        timers = AddNewTimer(timers, notification);
                    else
                        timers = SetTimer(timers, notification);
                }

                _context.NotificationTimerMaster.UpdateRange(timers);
                await _context.SaveChangesAsync();
                return timers;

            }
            catch (DbUpdateException ex)
            {
                throw new GenericException(ErrorMessages.DbDuplicateRecord, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
            catch (Exception ex)
            {
                throw new GenericException(ErrorMessages.UnkownError, ex.Message);
            }
        }

        [HttpGet("notifaction-details")]
        public async Task<List<NotificationDetails>> GetNotificationDetails([FromQuery] int user_id)
        {
            List<NotificationDetails> notificationDetails = new List<NotificationDetails>();
            try
            {
                List<UserNotificationAssociation> userNotificationAssociation = new List<UserNotificationAssociation>();

                userNotificationAssociation = await _context.UserNotificationAssociation
                    .Include(n => n.Notification)
                    .ThenInclude(un => un.NotificationSeenBy).Where(un => un.UserID == user_id).ToListAsync();

                List<MaterialMaster> materialMasters = await _context.MaterialMaster
                                                      .Include(mm=>mm.StageAudits)
                                                      .Include(mm => mm.MRF)
                                                       .ThenInclude(mr => mr.CreatedBy)
                                                      .Include(mm => mm.Trackers)
                                                      .Include(mm => mm.Project)
                                                      .Include(mm => mm.Organisation)
                                                      .Include(mm => mm.MaterialType)
                                                      .Where(mm => mm.MRF != null)
                                                      .ToListAsync();
                List<JobSchedule> jobSchedules = await _context.JobSchedule
                                                      .Include(j => j.Material)
                                                      .ThenInclude(m => m.MaterialType)
                                                      .Include(m => m.Material.Project)
                                                      .Include(m => m.Material.Trackers)
                                                      .Include(j => j.Subcon)
                                                      .Include(j => j.Trade).ToListAsync();

                List<MaterialQCDefect> materialQCDefects = await _context.MaterialQCDefect
                    .Include(qc => qc.QCCase)
                    .ThenInclude(c => c.MaterialMaster)
                    .ThenInclude(mm => mm.MaterialType)
                    .Include(qc => qc.QCCase.MaterialMaster.Trackers)
                    .Include(d => d.Organisation)
                    .Include(qc => qc.CreatedBy).ToListAsync();

                foreach (UserNotificationAssociation userNotification in userNotificationAssociation)
                {
                    NotificationDetails objnotificationDetails = new NotificationDetails();
                    switch (userNotification.Notification.Code)
                    {

                        case (int)Enums.NotificationCode.CreateMRF:
                            objnotificationDetails = GetMaterialDetails(userNotification.Notification, materialMasters);

                            break;
                        case (int)Enums.NotificationCode.CloseMRF:
                            objnotificationDetails = GetMaterialDetails(userNotification.Notification, materialMasters);
                            break;
                        case (int)Enums.NotificationCode.JobAssigned:
                            objnotificationDetails = GetjobDetails(userNotification.Notification, jobSchedules);
                            break;
                        case (int)Enums.NotificationCode.JobCompleted:
                            objnotificationDetails = GetjobDetails(userNotification.Notification, jobSchedules);
                            break;
                        case (int)Enums.NotificationCode.JobQCAccepted:
                            objnotificationDetails = GetjobDetails(userNotification.Notification, jobSchedules);
                            break;
                        case (int)Enums.NotificationCode.JobQCRejected:
                            objnotificationDetails = GetjobDetails(userNotification.Notification, jobSchedules);
                            break;
                        case (int)Enums.NotificationCode.JobQCFailed:
                            objnotificationDetails = GetjobDetails(userNotification.Notification, jobSchedules);
                            break;
                        case (int)Enums.NotificationCode.JobQCPassed:
                            objnotificationDetails = GetjobDetails(userNotification.Notification, jobSchedules);
                            break;
                        case (int)Enums.NotificationCode.MaterialQCFailed:
                            objnotificationDetails = GetMaterialDetails(userNotification.Notification, materialMasters);
                            break;
                        case (int)Enums.NotificationCode.MaterialQCPassed:
                            objnotificationDetails = GetMaterialDetails(userNotification.Notification, materialMasters);
                            break;
                        case (int)Enums.NotificationCode.QCFailed:
                            objnotificationDetails = GetDefectDetails(userNotification.Notification, materialQCDefects);
                            break;
                        case (int)Enums.NotificationCode.QCRectified:
                            objnotificationDetails = GetDefectDetails(userNotification.Notification, materialQCDefects);
                            break;
                        case (int)Enums.NotificationCode.ModuleReceived:
                            objnotificationDetails = GetMaterialDetails(userNotification.Notification, materialMasters);
                            break;
                        default:
                            break;
                    }
                    objnotificationDetails.IsSeen = userNotification.Notification.NotificationSeenBy.Where(n => n.UserMasterID == user_id).Count() > 0;
                    notificationDetails.Add(objnotificationDetails);
                }
                notificationDetails = notificationDetails.OrderByDescending(n => n.ProcessDate).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificationDetails;
        }

        private NotificationDetails GetDefectDetails(NotificationAudit usernotification, List<MaterialQCDefect> materialQCDefects)
        {
            NotificationDetails notification = new NotificationDetails();
            try
            {
                MaterialQCDefect qCDefects = materialQCDefects.Where(qc => qc.ID == int.Parse(usernotification.Reference)).FirstOrDefault();
                if (qCDefects != null)
                {
                    notification.ProjectID = qCDefects.QCCase.MaterialMaster.ProjectID;
                    notification.Block = qCDefects.QCCase.MaterialMaster.Block;
                    notification.Level = qCDefects.QCCase.MaterialMaster.Level;
                    notification.MarkingNo = qCDefects.QCCase.MaterialMaster.MarkingNo;
                    notification.MaterialType = qCDefects.QCCase.MaterialMaster.MaterialType.Name;
                    notification.MaterialID = qCDefects.QCCase.MaterialMasterId;
                    notification.NotificationID = usernotification.ID;
                    notification.AssaignedTo = qCDefects.Organisation.Name;
                    notification.IsMaterial = true;
                    if (usernotification.Code == (int)Enums.NotificationCode.QCFailed)
                        notification.Message = "A new defect created under case, '" + qCDefects.QCCase.CaseName + "' for the Material '" + qCDefects.QCCase.MaterialMaster.Block + '-' + qCDefects.QCCase.MaterialMaster.Level + '-' + qCDefects.QCCase.MaterialMaster.Zone + '-' + qCDefects.QCCase.MaterialMaster.MarkingNo + "'";
                    if (usernotification.Code == (int)Enums.NotificationCode.QCRectified)
                        notification.Message = "A defect created under case, '" + qCDefects.QCCase.CaseName + "' for the Material '" + qCDefects.QCCase.MaterialMaster.Block + '-' + qCDefects.QCCase.MaterialMaster.Level + '-' + qCDefects.QCCase.MaterialMaster.Zone + '-' + qCDefects.QCCase.MaterialMaster.MarkingNo + "' is Rectified";
                    if (usernotification.ProcessedDate != null)
                        notification.ProcessDate = (DateTimeOffset)usernotification.ProcessedDate;
                    notification.Tag = qCDefects.QCCase.MaterialMaster.Trackers.Select(t => t.Tag).FirstOrDefault();
                    notification.NotificationCode = usernotification.Code;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notification;
        }

        private NotificationDetails GetjobDetails(NotificationAudit usernotification, List<JobSchedule> jobSchedules)
        {
            NotificationDetails notification = new NotificationDetails();
            try
            {

                JobSchedule jobScheduleDetails = jobSchedules.Where(j => j.ID == int.Parse(usernotification.Reference)).FirstOrDefault();
                if (jobScheduleDetails != null)
                {
                    notification.ProjectID = jobScheduleDetails.Material.ProjectID;
                    notification.Block = jobScheduleDetails.Material.Block;
                    notification.Level = jobScheduleDetails.Material.Level;
                    notification.Zone = jobScheduleDetails.Material.Zone;
                    notification.MarkingNo = jobScheduleDetails.Material.MarkingNo;
                    notification.MaterialType = jobScheduleDetails.Material.MaterialType.Name;
                    notification.NotificationID = usernotification.ID;
                    notification.JobScheduleID = jobScheduleDetails.ID;
                    notification.TradeName = jobScheduleDetails.Trade.Name;
                    notification.AssaignedTo = jobScheduleDetails.Subcon.Name;
                    notification.MaterialID = jobScheduleDetails.MaterialID;
                    if (usernotification.Code == (int)Enums.NotificationCode.JobAssigned)
                        notification.Message = "A new '" + jobScheduleDetails.Trade.Name + "' job has been assigned to you.";
                    else if (usernotification.Code == (int)Enums.NotificationCode.JobCompleted)
                        notification.Message = "A new '" + jobScheduleDetails.Trade.Name + "' job has been completed by '" + jobScheduleDetails.Subcon.Name + "'.";
                    else if (usernotification.Code == (int)Enums.NotificationCode.JobQCFailed)
                        notification.Message = "A '" + jobScheduleDetails.Trade.Name + "' job for Material '" + jobScheduleDetails.Material.MarkingNo + "' has failed QC.";
                    else if (usernotification.Code == (int)Enums.NotificationCode.JobQCPassed)
                        notification.Message = "A '" + jobScheduleDetails.Trade.Name + "' job for Material '" + jobScheduleDetails.Material.MaterialType.Name + "' has passed QC.";
                    else if (usernotification.Code == (int)Enums.NotificationCode.JobQCAccepted)
                        notification.Message = "QC for '" + jobScheduleDetails.Trade.Name + "' job on Material '" + jobScheduleDetails.Material.MarkingNo + "' has been accepted.";
                    else if (usernotification.Code == (int)Enums.NotificationCode.JobQCRejected)
                        notification.Message = "QC for '" + jobScheduleDetails.Trade.Name + "' job on Material '" + jobScheduleDetails.Material.MarkingNo + "' has been rejected.";
                    notification.IsMaterial = false;
                    notification.IsJob = true;
                    if (usernotification.ProcessedDate != null)
                        notification.ProcessDate = (DateTimeOffset)usernotification.ProcessedDate;
                    notification.Tag = jobScheduleDetails.Material.Trackers.Select(t => t.Tag).FirstOrDefault();
                    notification.NotificationCode = usernotification.Code;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notification;
        }

        private NotificationDetails GetMaterialDetails(NotificationAudit usernotifications, List<MaterialMaster> materialMasters)
        {
            NotificationDetails notification = new NotificationDetails();
            MaterialMaster materialMaster = null;
            
            try
            {
                if (usernotifications.Code == (int)Enums.NotificationCode.CreateMRF || usernotifications.Code == (int)Enums.NotificationCode.CloseMRF || usernotifications.Code==(int)Enums.NotificationCode.ModuleReceived )
                {
                    materialMaster = materialMasters.Where(m => m.MRF.ID == int.Parse(usernotifications.Reference)).FirstOrDefault();
                }
                else
                {
                    MaterialStageAudit materialStageAudit = _context.MaterialStageAudit.Where(sa => sa.ID == int.Parse(usernotifications.Reference)).FirstOrDefault();
                    materialMaster = materialStageAudit.Material;
                }
                if (materialMaster != null)
                {
                    notification.ProjectID = materialMaster.ProjectID;
                    notification.NotificationID = usernotifications.ID;
                    notification.MaterialID = materialMaster.ID;
                    notification.Block = materialMaster.Block;
                    notification.Level = materialMaster.Level;
                    notification.Zone = materialMaster.Zone;
                    notification.MarkingNo = materialMaster.MarkingNo;
                    notification.MaterialType = materialMaster.MaterialType.Name;
                    MRFMaster mrf = materialMaster.MRF;
                    if (usernotifications.Code == (int)Enums.NotificationCode.CreateMRF)
                        notification.Message = "A new MRF '" + mrf.MRFNo + "' has been created with '" + mrf.Materials.Count + "' materials";
                    else if (usernotifications.Code == (int)Enums.NotificationCode.CloseMRF)
                        notification.Message = "A new MRF '" + mrf.MRFNo + "' has been closed with '" + mrf.Materials.Count + "' materials";
                    else if (usernotifications.Code == (int)Enums.NotificationCode.MaterialQCFailed)
                        notification.Message = "QC for Material '" + materialMaster.Block + '-' + materialMaster.Level + '-' + materialMaster.Zone + '-' + materialMaster.MarkingNo + "' has been failed.";
                    else if (usernotifications.Code == (int)Enums.NotificationCode.MaterialQCPassed)
                        notification.Message = "QC for Material '" + materialMaster.Block + '-' + materialMaster.Level + '-' + materialMaster.Zone + '-' + materialMaster.MarkingNo + "' has been passed.";
                    else if(usernotifications.Code == (int)Enums.NotificationCode.ModuleReceived)
                        notification.Message = "A new module '" + materialMaster.Block + '-' + materialMaster.Level + '-' + materialMaster.Zone + '-' + materialMaster.MarkingNo + "' has been received at assemblyyard.";
                    notification.CreatedBy = mrf.CreatedBy.PersonName;
                    notification.IsMaterial = true;
                    notification.IsJob = false;
                    if (usernotifications.ProcessedDate != null)
                        notification.ProcessDate = (DateTimeOffset)usernotifications.ProcessedDate;
                    notification.Tag = materialMaster.Trackers.Select(t => t.Tag).FirstOrDefault();
                    notification.NotificationCode = usernotifications.Code;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notification;
        }
      
        [HttpPost("update-seenby/{userid}")]
        public async Task UpdateSeenBy([FromRoute] int userid, [FromQuery] int Type)
        {
            List<NotificationSeenBy> listnotificationSeenn = new List<NotificationSeenBy>();
            try
            {
                List<UserNotificationAssociation> userNotificationAssociation = new List<UserNotificationAssociation>();

                userNotificationAssociation = await _context.UserNotificationAssociation
                    .Include(n => n.Notification)
                    .ThenInclude(un => un.NotificationSeenBy).Where(un => un.UserID == userid && un.Notification.NotificationSeenBy.Where(ns => ns.UserMasterID == userid).Count() == 0)
                    .ToListAsync();
                if (userNotificationAssociation.Count() > 0 && Type == 0) //Material Notifications
                {
                    userNotificationAssociation = userNotificationAssociation.Where(un => un.Notification.Code == (int)Enums.NotificationCode.CreateMRF || un.Notification.Code == (int)Enums.NotificationCode.CloseMRF || un.Notification.Code == (int)Enums.NotificationCode.MaterialQCFailed || un.Notification.Code == (int)Enums.NotificationCode.MaterialQCPassed || un.Notification.Code == (int)Enums.NotificationCode.QCFailed || un.Notification.Code == (int)Enums.NotificationCode.QCRectified).ToList();
                }
                else if (userNotificationAssociation.Count() > 0 && Type == 1)//JobNotifications
                {
                    userNotificationAssociation = userNotificationAssociation.Where(un => un.Notification.Code == (int)Enums.NotificationCode.JobAssigned || un.Notification.Code == (int)Enums.NotificationCode.JobCompleted || un.Notification.Code == (int)Enums.NotificationCode.JobQCFailed || un.Notification.Code == (int)Enums.NotificationCode.JobQCPassed || un.Notification.Code == (int)Enums.NotificationCode.JobQCAccepted || un.Notification.Code == (int)Enums.NotificationCode.JobQCRejected).ToList();
                }
                if (userNotificationAssociation.Count() > 0)
                {
                    foreach (UserNotificationAssociation notificationAssociation in userNotificationAssociation)
                    {
                        NotificationSeenBy notificationSeenBy = new NotificationSeenBy();
                        notificationSeenBy.NotificationAuditID = notificationAssociation.NotificationID;
                        notificationSeenBy.UserMasterID = userid;
                        notificationSeenBy.SeenDate = DateTimeOffset.UtcNow;
                        listnotificationSeenn.Add(notificationSeenBy);
                    }
                }
                await _context.NotificationSeenBy.AddRangeAsync(listnotificationSeenn);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

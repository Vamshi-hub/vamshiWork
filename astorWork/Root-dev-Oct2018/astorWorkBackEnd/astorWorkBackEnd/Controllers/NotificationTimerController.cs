using astorWorkBackEnd.Models;
using astorWorkDAO;
using astorWorkShared.GlobalResponse;
using astorWorkShared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkBackEnd.Controllers
{
    [Produces("application/json")]
    [Route("notification-timer")]
    [ApiController]
    public class NotificationTimerController : ControllerBase
    {
        private readonly astorWorkDbContext _context;
        public NotificationTimerController(astorWorkDbContext context)
        {
            _context = context;
        }

        // GET: notification-timer/5?type=project
        [HttpGet("{id}")]
        public APIResponse Get(int id, [FromQuery]string type)
        {
            List<NotificationTimerMaster> lstNotificationTimer = new List<NotificationTimerMaster>();
            try
            {
                if (!ModelState.IsValid)
                {
                    return new APIBadRequest();
                }
                List<NotificationDetails> notificationFromDictonary = AppConfiguration.RecuringNotification.Select(n => new NotificationDetails()
                {
                    Code = n.Key,
                    NotificationName = n.Value[0],
                    Description = n.Value[1],
                    Enabled = false,
                    ID = 0,
                    TriggerTime = "00:00",
                    SiteId = type == "project" ? 0 : id,
                    ProjectId = type == "project" ? id : 0
                }).ToList();

                if (type == "project")
                {
                    lstNotificationTimer = _context.NotificationTimerMaster.Where(n => n.Project.ID == id).ToList();
                }
                else if (type == "site")
                {
                    lstNotificationTimer = _context.NotificationTimerMaster.Where(n => n.Site.ID == id).ToList();
                }
                else
                {
                    lstNotificationTimer = _context.NotificationTimerMaster.Where(n => n.ID == id).ToList();
                }
               
                if ((lstNotificationTimer != null && lstNotificationTimer.Count != 0))
                   foreach(var notification in notificationFromDictonary)
                    {
                        var v = lstNotificationTimer.Where(n => n.Code == notification.Code).FirstOrDefault();
                        if (v != null)
                        {
                            notification.ID = v.ID;
                            notification.TriggerTime = TimeSpan.FromMinutes(v.TriggerTime).ToString(@"hh\:mm");
                            notification.Enabled = v.Enabled;
                        }
                    }
                return new APIResponse(0, notificationFromDictonary);
            }
            catch
            {
                return new APIBadRequest();
            }
        }

        // POST: notification-timer
        [HttpPost]
        public async Task<APIResponse> AddUpdateNotifications([FromBody] List<NotificationDetails> lstNotificationDetails)
        {

            if (!ModelState.IsValid)
            {
                return new APIBadRequest();
            }
            try
            {
                var lstnotifications = _context.NotificationTimerMaster.Where(n =>
            lstNotificationDetails.FirstOrDefault().ProjectId != 0 ? n.Project.ID == lstNotificationDetails.FirstOrDefault().ProjectId : n.Site.ID == lstNotificationDetails.FirstOrDefault().SiteId).ToList();
            foreach (var notification in lstNotificationDetails)
            {
                if (notification.ID == 0)
                {
                    lstnotifications.Add(new NotificationTimerMaster
                    {
                        Code = notification.Code,
                        TriggerTime = Convert.ToInt32(TimeSpan.Parse(notification.TriggerTime).TotalMinutes),
                        Enabled = notification.Enabled,
                        UpdateRequired = true,
                        Project = _context.ProjectMaster.Where(p => p.ID == notification.ProjectId).FirstOrDefault(),
                        Site = _context.SiteMaster.Where(s => s.ID == notification.SiteId).FirstOrDefault()
                    });
                }
                else
                {
                    lstnotifications.Where(n => n.ID == notification.ID).FirstOrDefault().TriggerTime = Convert.ToInt32(TimeSpan.Parse(notification.TriggerTime).TotalMinutes);
                        lstnotifications.Where(n => n.ID == notification.ID).FirstOrDefault().Enabled = notification.Enabled;
                    lstnotifications.Where(n => n.ID == notification.ID).FirstOrDefault().UpdateRequired = true;
                    
                }
            }
           
                _context.NotificationTimerMaster.UpdateRange(lstnotifications);
                await _context.SaveChangesAsync();
                return new APIResponse(0, new
                {
                    lstnotifications
                });
            }
            catch (DbUpdateException dbExc)
            {
                return new APIResponse(ErrorMessages.DbDuplicateRecord, null,
                    dbExc.InnerException == null ? dbExc.Message : dbExc.InnerException.Message);
            }
            catch (Exception exc)
            {
                return new APIResponse(ErrorMessages.UnkownError, null, exc.Message);
            }
        }
    }
}

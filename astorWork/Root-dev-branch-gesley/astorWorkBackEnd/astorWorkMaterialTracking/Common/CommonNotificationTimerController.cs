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

namespace astorWorkMaterialTracking.Common
{
    public class CommonNotificationTimerController : CommonController
    {
        public CommonNotificationTimerController(astorWorkDbContext context) : base(context)
        {
        }

        protected List<Notification> GetNotificationDetails(int id, string type)
        {
            return AppConfiguration.RecuringNotification.Select(n => new Notification()
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
        }

        protected List<NotificationTimerMaster> GetTimers(int id, string type)
        {
            if (type == "project")
                return _context.NotificationTimerMaster.Where(n => n.Project.ID == id).ToList();
            else if (type == "site")
                return _context.NotificationTimerMaster.Where(n => n.Site.ID == id).ToList();
            else
                return _context.NotificationTimerMaster.Where(n => n.ID == id).ToList();
        }

        protected List<Notification> SetNotifications(List<Notification> notifications, List<NotificationTimerMaster> timers)
        {
            foreach (Notification notification in notifications)
            {
                NotificationTimerMaster timer = timers.Where(n => n.Code == notification.Code).FirstOrDefault();
                if (timer != null)
                {
                    notification.ID = timer.ID;
                    notification.TriggerTime = TimeSpan.FromMinutes(timer.TriggerTime).ToString(@"hh\:mm");
                    notification.Enabled = timer.Enabled;
                }
            }

            return notifications;
        }

        protected NotificationTimerMaster CreateTimer(Notification notification)
        {
            return new NotificationTimerMaster
            {
                Code = notification.Code,
                TriggerTime = Convert.ToInt32(TimeSpan.Parse(notification.TriggerTime).TotalMinutes),
                Enabled = notification.Enabled,
                UpdateRequired = true,
                Project = _context.ProjectMaster.Where(p => p.ID == notification.ProjectId).FirstOrDefault(),
                Site = _context.SiteMaster.Where(s => s.ID == notification.SiteId).FirstOrDefault()
            };
        }

        protected List<NotificationTimerMaster> GetTimers(List<Notification> notifications)
        {
            return _context.NotificationTimerMaster.Where(n =>
            notifications.FirstOrDefault().ProjectId != 0 ? n.Project.ID == notifications.FirstOrDefault().ProjectId : n.Site.ID == notifications.FirstOrDefault().SiteId).ToList();
        }

        protected List<NotificationTimerMaster> AddNewTimer(List<NotificationTimerMaster> timers, Notification notification)
        {
            timers.Add(new NotificationTimerMaster
            {
                Code = notification.Code,
                TriggerTime = Convert.ToInt32(TimeSpan.Parse(notification.TriggerTime).TotalMinutes),
                Enabled = notification.Enabled,
                UpdateRequired = true,
                Project = _context.ProjectMaster.Where(p => p.ID == notification.ProjectId).FirstOrDefault(),
                Site = _context.SiteMaster.Where(s => s.ID == notification.SiteId).FirstOrDefault()
            });

            return timers;
        }

        protected List<NotificationTimerMaster> SetTimer(List<NotificationTimerMaster> timers, Notification notification)
        {
            timers.Where(n => n.ID == notification.ID).FirstOrDefault().TriggerTime = Convert.ToInt32(TimeSpan.Parse(notification.TriggerTime).TotalMinutes);
            timers.Where(n => n.ID == notification.ID).FirstOrDefault().Enabled = notification.Enabled;
            timers.Where(n => n.ID == notification.ID).FirstOrDefault().UpdateRequired = true;

            return timers;
        }
    }
}
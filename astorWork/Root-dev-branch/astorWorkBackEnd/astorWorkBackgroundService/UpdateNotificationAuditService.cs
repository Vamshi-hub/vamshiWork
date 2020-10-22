using astorWorkBackgroundService.Models;
using astorWorkDAO;
using astorWorkShared.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace astorWorkBackgroundService
{
    public class UpdateNotificationAuditService
    {
        public astorWorkDbContext _context;

        public UpdateNotificationAuditService(astorWorkDbContext astorWorkDbContext)
        {
            _context = astorWorkDbContext;
        }

        /// <summary>
        /// gets the list of material exceeded the expectedDeliveryDate
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<DeliveryMaterial> GetDelayedDeliveryMaterials(int projectOrOrganisationID, bool isProjectID)
        {
            /*
            DateTimeOffset? lastProcessedDate = null;
            if (notificationAudit != null)
                lastProcessedDate = _context.NotificationAudit.Where(n => n.ProcessedDate != null && 
                                                                          n.Code == Convert.ToInt64(Enums.NotificationCode.DelayInDelivery) && 
                                                                          n.Reference == notificationAudit.Reference &&
                                                                          n.NotificationTimer == notificationAudit.NotificationTimer)
                                                              .OrderByDescending(n => n.ProcessedDate)
                                                              .FirstOrDefault()
                                                              .ProcessedDate;
                                                              */
            IEnumerable<MaterialMaster> materialMasters = _context.MaterialMaster.Include(m => m.MRF)
                                                          .Include(m => m.Project)
                                                          .Include(v => v.Organisation)
                                                          .Include(m => m.MaterialInfoAudits)
                                                          .Include(m => m.StageAudits)
                                                          .ThenInclude(s => s.Stage)
                                                          .Where
                                                          (
                                                            m => (isProjectID ? m.Project.ID == projectOrOrganisationID : m.Organisation.ID == projectOrOrganisationID)
                                                            &&
                                                            (
                                                                (m.MaterialInfoAudits.OrderByDescending(i => i.UpdatedDate).FirstOrDefault().ExpectedDeliveryDate.AddMinutes(m.Project.TimeZoneOffset).Date < DateTime.UtcNow.AddMinutes(m.Project.TimeZoneOffset).Date)
                                                                || (m.MRF.ExpectedDeliveryDate.AddMinutes(m.Project.TimeZoneOffset).Date < DateTime.UtcNow.AddMinutes(m.Project.TimeZoneOffset).Date)
                                                            )
                                                            &&
                                                            (
                                                                m.StageAudits == null ||
                                                                m.StageAudits.Count == 0 ||
                                                                m.StageAudits.OrderByDescending(s => s.Stage.Order).FirstOrDefault().Stage.Order < GetStageOrder(Convert.ToInt32(Enums.MileStoneId.Delivered))
                                                            )
                                                            /*
                                                            &&
                                                            (
                                                                (notificationAudit == null)?true: 
                                                                (
                                                                    (lastProcessedDate == null) ? true : (m.MRF.ExpectedDeliveryDate > lastProcessedDate)
                                                                    || m.MaterialInfoAudits.OrderByDescending(i => i.UpdatedDate).FirstOrDefault().ExpectedDeliveryDate > lastProcessedDate
                                                                )
                                                            )*/
                                                          );


            return CreateDeliveryMaterialsList(materialMasters);
        }

        /// <summary>
        /// gets list of material expected to deliver today
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<DeliveryMaterial> GetExpectedDeliveryMaterials(int projectOrOrganisationID, bool isProjectID)
        {
            IEnumerable<MaterialMaster> materialMasters = _context.MaterialMaster.Include(m => m.MRF)
                                                          .Include(m => m.Project)
                                                          .Include(v => v.Organisation)
                                                          .Include(m => m.MaterialInfoAudits)
                                                          .Include(m => m.StageAudits)
                                                          .ThenInclude(s => s.Stage)
                                                          .Where
                                                          (
                                                            m => (isProjectID ? m.Project.ID == projectOrOrganisationID : m.Organisation.ID == projectOrOrganisationID)
                                                            && (
                                                                m.MRF.ExpectedDeliveryDate.Date.AddMinutes(m.Project.TimeZoneOffset).Date == DateTime.UtcNow.AddMinutes(m.Project.TimeZoneOffset).Date
                                                                ||
                                                                (
                                                                    m.MaterialInfoAudits != null && m.MaterialInfoAudits.Count != 0
                                                                    && m.MaterialInfoAudits.OrderByDescending(i => i.UpdatedDate).FirstOrDefault().ExpectedDeliveryDate.AddMinutes(m.Project.TimeZoneOffset) >= DateTime.UtcNow.AddMinutes(m.Project.TimeZoneOffset).Date
                                                                    && m.MaterialInfoAudits.Count != 0 && m.MaterialInfoAudits.OrderByDescending(i => i.UpdatedDate).FirstOrDefault().ExpectedDeliveryDate.AddMinutes(m.Project.TimeZoneOffset) < DateTime.UtcNow.AddMinutes(m.Project.TimeZoneOffset).Date.AddDays(1)
                                                                )
                                                            )
                                                            && (
                                                                m.StageAudits == null || m.StageAudits.Count <= 0 || 
                                                                m.StageAudits.OrderByDescending(s => s.Stage.Order).FirstOrDefault().Stage.Order < GetStageOrder(Convert.ToInt32(Enums.MileStoneId.Delivered))
                                                            )
                                                          );

            return CreateDeliveryMaterialsList(materialMasters).Where(m => m.ExpectedDeliveryDate.Date == DateTime.UtcNow.AddMinutes(m.TimeZoneOffset).Date).ToList();
        }

        private int GetStageOrder(int mileStoneId) {
            return _context.MaterialStageMaster.Where(s => s.MilestoneId == mileStoneId).FirstOrDefault().Order;
        }

        protected List<DeliveryMaterial> CreateDeliveryMaterialsList(IEnumerable<MaterialMaster> materialMasters) {
            List<DeliveryMaterial> deliveryMaterials = new List<DeliveryMaterial>();

            foreach (MaterialMaster materialMaster in materialMasters)
            {
                DeliveryMaterial deliveryMaterial = CreateDeliveryMaterial(materialMaster);
                deliveryMaterials.Add(deliveryMaterial);
            }

            return deliveryMaterials;
        }

        protected DeliveryMaterial CreateDeliveryMaterial(MaterialMaster materialMaster)
        {

            int TimeZoneOffset = materialMaster.Project.TimeZoneOffset;

            DeliveryMaterial delayedDeliveryMaterial = new DeliveryMaterial();
            delayedDeliveryMaterial.ID = materialMaster.ID;
            delayedDeliveryMaterial.Block = materialMaster.Block;
            delayedDeliveryMaterial.Level = materialMaster.Level;
            delayedDeliveryMaterial.Zone = materialMaster.Zone;
            delayedDeliveryMaterial.MarkingNo = materialMaster.MarkingNo;
            delayedDeliveryMaterial.MaterialType = materialMaster.MaterialType.Name;
            delayedDeliveryMaterial.ExpectedDeliveryDate = GetMaterialExpectedDeliveryDate(materialMaster).AddMinutes(Convert.ToDouble(TimeZoneOffset));
            delayedDeliveryMaterial.TimeZoneOffset = TimeZoneOffset;

            return delayedDeliveryMaterial;
        }

        protected DateTimeOffset GetMaterialExpectedDeliveryDate(MaterialMaster materialMaster)
        {
            DateTimeOffset expectedDeliveryDate;

            if (materialMaster.MaterialInfoAudits == null || materialMaster.MaterialInfoAudits.Count == 0)
                expectedDeliveryDate = materialMaster.MRF.ExpectedDeliveryDate;
            else
                expectedDeliveryDate = materialMaster.MaterialInfoAudits.OrderByDescending(i => i.UpdatedDate).FirstOrDefault().ExpectedDeliveryDate;

            return expectedDeliveryDate;
        }

        protected UserMaster GetProjectManager(int projectID)
        {
            int projectManagerRoleID = 4;
            return _context.UserMaster
                .Include(u => u.Role)
                .Include(u => u.Project)
                .Where(u => u.Role.ID == projectManagerRoleID
                    && u.Project.ID == projectID)
                .FirstOrDefault();
        }

        protected List<UserMaster> AddNotificationReceipients(int projectID, List<DeliveryMaterial> deliveryMaterials, int siteID)
        {
            List<UserMaster> receipients = new List<UserMaster>();

            if (siteID == 0) // email for project
            {
                UserMaster projectManager = GetProjectManager(projectID);

                if (projectManager != null)
                    receipients.Add(projectManager);
            }
            else // email for vendor
                receipients = GetMRFAttentions(deliveryMaterials, siteID);

            return receipients;
        }

        public List<UserMaster> GetMRFAttentions(List<DeliveryMaterial> deliveryMaterials, int siteID) {
            List<UserMaster> receipients = new List<UserMaster>();

            foreach (DeliveryMaterial delayedDeliveryMaterial in deliveryMaterials)
            {
                IEnumerable<UserMRFAssociation> userMRFAssociations = _context.UserMRFAssociation
                                                    .Include(uma => uma.MRF)
                                                    .ThenInclude(mrf => mrf.Materials)
                                                    .Include(uma => uma.User).
                                                    Include(s => s.User.Site)
                                                    .Where(uma => uma.MRF.Materials.Any(mm => mm.ID == delayedDeliveryMaterial.ID));

                if (userMRFAssociations != null)
                {
                    foreach (UserMRFAssociation userMRFAttention in userMRFAssociations.Where(u => u.User.Site.ID == siteID))
                    {
                        UserMaster mrfAttention = userMRFAttention.User;

                        if (!receipients.Contains(mrfAttention))
                            receipients.Add(mrfAttention);
                    }
                }
            }

            if (receipients == null || receipients.Count < 1)
                return null;

            return receipients;
        }

        public void UpdateDeliveryNotificationAudit(astorWorkDbContext _astorWorkDbContext, int notificationTimerId, int notificationCode)
        {
            _context = _astorWorkDbContext;

            int projectOrOrganisationID = 0;
            bool isProjectID = false;

            NotificationTimerMaster timer = _astorWorkDbContext.NotificationTimerMaster
                                            .Include(p => p.Project)
                                            .Include(n => n.Site)
                                            .ThenInclude(s => s.Organisation)
                                            .Where(n => n.ID == notificationTimerId).FirstOrDefault();

            if (timer != null && timer.Project != null)
            {
                projectOrOrganisationID = timer.Project.ID;
                isProjectID = true;
            }
            else
                projectOrOrganisationID = timer.Site.Organisation.ID;

            List<DeliveryMaterial> deliveryMaterials = new List<DeliveryMaterial>();

            if (notificationCode == Convert.ToInt32(Enums.NotificationCode.DelayInDelivery))
                deliveryMaterials = GetDelayedDeliveryMaterials(projectOrOrganisationID, isProjectID);
            else
                deliveryMaterials = GetExpectedDeliveryMaterials(projectOrOrganisationID, isProjectID);

            List<UserMaster> receipients = AddNotificationReceipients(projectOrOrganisationID, deliveryMaterials, timer.Site != null ? timer.Site.ID : 0);

            if (receipients != null && receipients.Count > 0 && deliveryMaterials.Count > 0)
                UpdateNotificationAudit(receipients, Convert.ToInt32(notificationCode), Convert.ToInt32(Enums.NotificationType.Email), projectOrOrganisationID.ToString(), timer);
        }

        /*
        public void UpdateTodayExpectedDeliveryNotificationAudit(astorWorkDbContext _astorWorkDbContext, int notificationTimerId)
        {
            _context = _astorWorkDbContext;

            int projectOrVendorID = 0;
            bool isProjectID = false;

            NotificationTimerMaster timer = _context.NotificationTimerMaster
                                            .Include(p => p.Project)
                                            .Include(n => n.Site)
                                            .ThenInclude(s => s.Vendor)
                                            .Where(n => n.ID == notificationTimerId).FirstOrDefault();

            if (timer != null && timer.Project != null)
            {
                projectOrVendorID = timer.Project.ID;
                isProjectID = true;
            }
            else
                projectOrVendorID = timer.Site.Vendor.ID;

            List<DeliveryMaterial> todayExpectedDeliveryMaterials = GetTodayDeliveryMaterials(projectOrVendorID, isProjectID);
            List<UserMaster> receipients = AddNotificationReceipients(projectOrVendorID, todayExpectedDeliveryMaterials, timer.Site != null ? timer.Site.ID : 0);
            if (receipients != null && receipients.Count > 0)
                UpdateNotificationAudit(receipients, Convert.ToInt32(Enums.NotificationCode.TodayExpectedDelivery), Convert.ToInt32(Enums.NotificationType.Email), projectOrVendorID.ToString(), timer);
        }
        */

        protected void UpdateNotificationAudit(List<UserMaster> recipients, int code, int type, string referenceID, NotificationTimerMaster timer)
        {
            NotificationAudit notification = new NotificationAudit
            {
                Code = code,
                Type = type,
                Reference = referenceID,
                CreatedDate = DateTimeOffset.Now,
                ProcessedDate = null,
                NotificationTimer = timer
            };

            List<UserNotificationAssociation> userNotificationAssociation = recipients.Select(
                receipient => new UserNotificationAssociation
                {
                    Receipient = receipient,
                    Notification = notification
                }
            ).ToList();

            _context.NotificationAudit.Add(notification);
            _context.UserNotificationAssociation.AddRange(userNotificationAssociation);

            _context.SaveChanges();
        }
    }
}

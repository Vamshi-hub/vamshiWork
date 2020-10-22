using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkShared.Utilities
{
    public class Enums
    {
        public enum NotificationType
        {
            Email = 0,
            SMS = 1,
            MobilePushNotification = 2,
            WebNotification = 3
        }

        public enum NotificationCode
        {
            CreateMRF = 0,
            BIMSync = 1,
            DelayInDelivery = 2,
            QCFailed = 3,
            QCRectified = 4,
            CloseMRF = 5,
            ResetPassword = 6,
            CreateUser = 7,
            TodayExpectedDelivery = 8
        }

        public enum AttachmentType
        {
            ProjectFile,
            MRFShopDrawing,
            MaterialDrawing
        }

        public enum RoleType
        {
            SuperAdmin = 1,
            Admin = 2,
            Management = 3,
            ProjectManager = 4,
            SiteOfficer = 5,
            BIM = 6,
            VendorProjectManager = 7,
            VendorProductionOfficer = 8
        }

        public enum StageOrder
        {
            Delivered = 3
        }
    }
}

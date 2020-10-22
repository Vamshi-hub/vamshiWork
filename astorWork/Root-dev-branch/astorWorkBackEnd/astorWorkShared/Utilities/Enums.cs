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
        /// <summary>
        /// Material QC is structural 
        /// Job QC is Archi
        /// QC pass or fail is defect notification
        /// </summary>
        public enum NotificationCode
        {
            CreateMRF = 0, //info 
            BIMSync = 1, //infos
            DelayInDelivery = 2, //warning
            QCFailed = 3, //critical
            QCRectified = 4, //info
            CloseMRF = 5, // info
            ResetPassword = 6, //NA
            CreateUser = 7, //NA
            TodayExpectedDelivery = 8, //info
            JobAssigned = 9, //info
            JobCompleted = 10, // info
            JobQCPassed = 11, //info
            JobQCFailed = 12, //critical
            JobQCAccepted = 13, // info
            JobQCRejected = 14, //critical @
            MaterialQCPassed=15, //info 
            MaterialQCFailed = 16//critical @

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
            VendorProductionOfficer = 8,
            PPVCSubcontractor = 9,
            RTO = 10,
            MainConQC=11
        }

        public enum MileStoneId
        {
            Delivered = 2
        }

        public enum OrganisationType
        {
            MainCon = 0,
            Vendor = 1,
            Subcon = 2
        }

        public enum TagType
        {
            QR_Code,
            RFID,
            Beacon
        }

        public enum JobStatus
        {
            Job_not_assigned,
            Job_not_scheduled,
            Job_not_started,
            Job_delayed,
            Job_started,
            Job_completed,
            QC_failed_by_Maincon,
            QC_rectified_by_Subcon,
            QC_passed_by_Maincon,
            QC_routed_to_RTO,
            QC_rejected_by_RTO,
            QC_accepted_by_RTO,
            All_QC_passed
        }

        public enum QCStatus
        {
            Pending_QC,
            QC_failed_by_Maincon,
            QC_rectified_by_Subcon,
            QC_passed_by_Maincon,
            QC_routed_to_RTO,
            QC_rejected_by_RTO,
            QC_accepted_by_RTO
        }

        public enum ChecklistItemStatus
        {
            Pending,
            Pass,
            Fail,
            NA
        }

        public enum ChecklistItemType
        {
            Architectural=1,
            Structural=2
        }

        public enum MobileEntryPoint
        {
            Vendor = 0,
            MainCon = 1,
            SubCon = 2,
            RTO = 3
        }
    }
}

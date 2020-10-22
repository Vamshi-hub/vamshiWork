using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkMobile.Shared.Classes
{
    public class Enums
    {
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

        public enum OrganisationType
        {
            MainCon = 0,
            Vendor = 1,
            Subcon = 2
        }
        public enum ChecklistItemStatus
        {
            Pending,
            Pass,
            Fail,
            NA
        }
        public enum PageType
        {
            Navigation,
            Modal,
            None
        }
        public enum PageActions
        {
            PushAsync,
            PopAsync,
            None
        }
        public enum MessageActions
        {
            Success,
            Error,
            Warning
        }
        public enum ColorTypes
        {
            Success,
            Error,
            Warning
        }
        public enum Pages
        {
            QC_Notifications,
            Job_Summary
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
            TodayExpectedDelivery = 8,
            JobAssigned = 9,
            JobCompleted = 10,
            JobQCPassed = 11,
            JobQCFailed = 12,
            JobQCAccepted = 13,
            JobQCRejected = 14,
            MaterialQCPassed = 15,
            MaterialQCFailed = 16

        }
        public enum NotificationType
        {
            Material_Notification,
            Job_Notification
        }
        public enum InspectionStage
        {
            NA,
            Before,
            Ongoing,
            After
        }
        public int Type { get; set; }
        public enum ChecklistType
        {
            Architectural_Maincon = 1,
            Structural = 2,
            M_E = 3,
            Stage = 4,
            Architectural_RTO = 5
        }
        public enum RoleType
        {
            SiteOfficerOrMainCon,
            RTO = 3
        }
    }
}

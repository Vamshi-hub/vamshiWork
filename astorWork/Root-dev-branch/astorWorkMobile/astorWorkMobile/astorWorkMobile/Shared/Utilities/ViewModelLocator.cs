using astorWorkMobile.JobTrack.ViewModels;
using astorWorkMobile.MaterialTrack.ViewModels;
using astorWorkMobile.Shared.Classes;

namespace astorWorkMobile.Shared.Utilities
{
    public static class ViewModelLocator
    {
        public static PageCode pageTypeCode = new PageCode();
        public static MainContentPageVM mainContentPageVM = new MainContentPageVM();

        public static VendorHomeVM vendorHomeVM = new VendorHomeVM();
        public static ScanTrackerVM scanTrackerVM = new ScanTrackerVM();
        public static SiteUpdateStageVM siteUpdateStageVM = new SiteUpdateStageVM();
        public static LoginVM loginVM = new LoginVM();
        public static MaincontentPageMenuItem menuItem = new MaincontentPageMenuItem();
        public static MTTakePhotoVM mtTakePhotoVM = new MTTakePhotoVM();
        public static SiteListDefectsVM siteListDefectsVM = new SiteListDefectsVM();
        public static QCDefectVM qcDefectVM = new QCDefectVM();
        public static SiteScanRFIDDemoVM siteScanDemoVM;
        public static BulkUpdateVM bulkUpdateVM = new BulkUpdateVM();

        public static MainconHomeVM mainconHomeVM = new MainconHomeVM();
        public static SubconHomeVM subconHomeVM = new SubconHomeVM();
        public static RTOHomeVM rtoHomeVM = new RTOHomeVM();
        public static MTRTOHomeVM mtRTOHomeVM = new MTRTOHomeVM();
        public static JobScanVM jobScanVM = new JobScanVM();
        public static JobScheduleVM jobScheduleVM = new JobScheduleVM();
        public static JobChecklistVM jobChecklistVM = new JobChecklistVM();
        public static JobChecklistItemVM jobChecklistItemVM = new JobChecklistItemVM();
        public static ChatVM chatVM = new ChatVM();
        public static ChatTakePhotoVM chatTakePhotoVM = new ChatTakePhotoVM();
        public static MaterialFrameVM materialFrameVM = new MaterialFrameVM();
        public static NotificationVM notificationVM = new NotificationVM();
    }
}

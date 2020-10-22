using astorWorkMobile.MaterialTrack.ViewModels;
using astorWorkMobile.Shared.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace astorWorkMobile.Shared.Utilities
{
    public static class ViewModelLocator
    {
        public static VendorInventoryVM vendorInventoryVM = new VendorInventoryVM();
        public static SingleScanTrackerVM singleScanTrackerVM = new SingleScanTrackerVM();
        public static VendorEnrolmentVM vendorEnrolmentVM = new VendorEnrolmentVM();
        public static VendorStartDeliveryVM vendorStartDeliveryVM = new VendorStartDeliveryVM();
        public static SiteScanRFIDVM siteScanRFIDVM = new SiteScanRFIDVM();
        public static SiteUpdateStageVM siteUpdateStageVM = new SiteUpdateStageVM();
        public static LoginVM loginVM = new LoginVM();
        public static MaincontentPageMenuItem menuItem = new MaincontentPageMenuItem();
        public static TakePhotoVM takePhotoVM = new TakePhotoVM();
        public static SiteListDefectsVM siteListDefectsVM = new SiteListDefectsVM();
        public static QCDefectVM qcDefectVM = new QCDefectVM();
        public static SiteScanRFIDDemoVM siteScanDemoVM;
    }
}

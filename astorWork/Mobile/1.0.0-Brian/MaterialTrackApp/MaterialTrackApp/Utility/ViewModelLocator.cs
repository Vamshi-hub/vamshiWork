using MaterialTrackApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialTrackApp.Utility
{
    public static class ViewModelLocator
    {
        public static EnrollPageVM enrollPageVM = new EnrollPageVM();
        public static InstallPageVM installPageVM = new InstallPageVM();
        public static ReportPageViewModel ReportPageVM = new ReportPageViewModel();
        public static BeaconInfoPageVM beaconInfoPageVM = new BeaconInfoPageVM();
        public static HomePageVM homePageVM = new HomePageVM();
        public static PendingTaskPageVM pendingTaskPageVM = new PendingTaskPageVM();
        public static ScanBeaconPageVM scanBeaconPageVM = new ScanBeaconPageVM();
        public static FilterMarkingNoVM filterMarkingNoVM = new FilterMarkingNoVM();
        public static FilterMRFNoVM filterMRFNoVM = new FilterMRFNoVM();
    }
}

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
        public static EnrollPageViewModel EnrollPageVM = new EnrollPageViewModel();
        public static ReportPageViewModel ReportPageVM = new ReportPageViewModel();
        public static BeaconInfoPageViewModel BeaconInfoPageVM = new BeaconInfoPageViewModel();
    }
}

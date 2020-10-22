using astorWorkMobile.JobTrack.Entities;
using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static astorWorkMobile.Shared.Utilities.ApiClient;

namespace astorWorkMobile.JobTrack.ViewModels
{
    public class JobScanVM : MasterVM
    {
        public StackLayout ScannedItemsGrid;
        public double Height;
        public double Width;
        public int SubConId = -1;
        public bool isSubon
        {
            get
            {
                var status = Application.Current.Properties["entry_point"].ToString() == "2" ? true : false;
                return status;
            }
        }
        public Material PPVC { get; set; }
        public List<JobScheduleDetails> Jobs { get; set; }
        public string ModuleName
        {
            get
            {
                if (Jobs.Count > 0)
                    return Jobs[0].ModuleName;
                else
                    return "No module scanned";
            }
        }
        public bool ShowJobsList
        {

            get
            {
                return Jobs != null && Jobs.Count > 0;
            }
        }
        private bool _allowScan;
        public bool AllowScan
        {
            get
            {
                return _allowScan;
            }
            set
            {
                _allowScan = value;
                OnPropertyChanged("AllowScan");
            }
        }
        private bool _cameraReady;
        public bool CameraReady
        {
            get
            {
                return _cameraReady;
            }
            set
            {
                _cameraReady = value;
                OnPropertyChanged("CameraReady");
            }
        }
        public async Task GetJobScheduleByTracker(string tag)
        {
            if (tag != null)
            {
                try
                {
                    IsLoading = true;
                    ApiResult result = null;
                    if (isSubon)
                    {
                        SubConId = int.Parse(Application.Current.Properties["organisationID"].ToString());
                        result = await ApiClient.Instance.JTGetJobScheduleBySubCon(0, SubConId, tag);
                    }
                    else
                    {
                        result = await ApiClient.Instance.JTGetJobScheduleBySubCon(new string[] { tag });
                    }
                    if (result.status == 0)
                    {
                        Jobs = result.data as List<JobScheduleDetails>;
                    }
                    else
                    {
                        Jobs = new List<JobScheduleDetails>();
                        ErrorMessage = result.message;
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
                IsLoading = false;
                OnPropertyChanged("ShowJobsList");
                OnPropertyChanged("Jobs");
            }
            AllowScan = true;
        }
        public JobScanVM() : base()
        {
            Jobs = new List<JobScheduleDetails>();
            AllowScan = true;
            _cameraReady = false;
        }
        public override void Reset()
        {
            base.Reset();
            _neverLoadBefore = false;
            AllowScan = true;
            Jobs.Clear();
        }
    }
}

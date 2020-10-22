using astorWorkMobile.JobTrack.Entities;
using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using static astorWorkMobile.Shared.Classes.Enums;
using static astorWorkMobile.Shared.Utilities.ApiClient;

namespace astorWorkMobile.JobTrack.ViewModels
{
    public class JobChecklistVM : MasterVM
    {
        public string CheckListHeader { get; set; }
        public string CheckListSubHeader { get; set; }
        public JobScheduleDetails _job { get; set; }
        public JobScheduleDetails Job
        {
            get { return _job; }
            set
            {
                _job = value;
                if (_job == null && Material != null)
                {
                    IsArchitechtural = false;
                    IsStructural = true;
                }
                else if (_job != null && Material == null)
                {
                    IsArchitechtural = true;
                    IsStructural = false;
                }
            }
        }

        public Material _material { get; set; }
        public Material Material
        {
            get { return _material; }
            set
            {
                _material = value;
                if (_material == null && Job != null)
                {
                    IsArchitechtural = true;
                    IsStructural = false;
                }
                else if (_material != null && Job == null)
                {
                    IsArchitechtural = false;
                    IsStructural = true;
                }
            }
        }

        public bool IsArchitechtural { get; set; }
        public bool IsStructural { get; set; }

        public List<Checklist> Checklists { get; set; }

        public string Remarks { get; set; }

        private bool _showChecklist { get; set; }
        public bool ShowChecklist
        {
            get => _showChecklist;
            set
            {
                _showChecklist = value;
                OnPropertyChanged("ShowChecklist");
            }
        }

        public async Task GetChecklist()
        {
            ApiResult result;
            IsLoading = true;

            if (IsArchitechtural)
            {
                CheckListHeader = Job.TradeName;
                CheckListSubHeader = Job.ModuleName;
                result = await ApiClient.Instance.JTGetJobChecklists(Job.ProjectID, Job.ID, 0);
            }
            else
            {
                CheckListHeader = ViewModelLocator.jobChecklistVM.Material.markingNo;
                CheckListSubHeader = ViewModelLocator.jobChecklistVM.Material.destination;
                result = await ApiClient.Instance.JTGetJobChecklists(0, 0, ViewModelLocator.jobChecklistVM.Material.id);
            }
            int currentInspecChkNumber = 0;

            var checklist = (result.data as List<Checklist>).OrderBy(c => c.Sequence).ToList();
            var userID = Application.Current.Properties["user_id"];
            if (Application.Current.Properties["entry_point"] != null)
            {
                switch (Application.Current.Properties["entry_point"])
                {
                    case 3:
                        checklist = checklist.Where(chk => chk.RTOID == Convert.ToInt32(Application.Current.Properties["user_id"])).ToList();
                        break;
                    case 2:
                        checklist = checklist.Where(chk => chk.StatusCode == (int)QCStatus.QC_failed_by_Maincon).ToList();
                        break;
                    case 0:
                        checklist = checklist.Where(chk => chk.StatusCode == (int)QCStatus.QC_failed_by_Maincon).ToList();
                        break;
                    default:
                        break;
                }
            }
            if (Job != null && Job.RouteToRTO)
            {
                var v = checklist.Where(c => c.StatusCode != (int)QCStatus.QC_accepted_by_RTO).OrderBy(c => c.Sequence);
                if (v != null && v.Count() > 0)
                {
                    currentInspecChkNumber = v.FirstOrDefault().Sequence;
                }
                else
                {
                    currentInspecChkNumber = checklist.Count();
                }
            }
            else
            {
                var v = checklist.Where(c => c.StatusCode != (int)QCStatus.QC_passed_by_Maincon && c.StatusCode != (int)QCStatus.QC_accepted_by_RTO).OrderBy(c => c.Sequence);
                if (v != null && v.Count() > 0)
                {
                    currentInspecChkNumber = v.FirstOrDefault().Sequence;
                }
                else
                {
                    currentInspecChkNumber = checklist.Count();
                }
            }

            foreach (var checklst in checklist)
            {
                if (checklst.Sequence <= currentInspecChkNumber)
                {
                    checklst.IsEnabled = true;
                }
            }

            Checklists = checklist;
            ShowChecklist = true;
            OnPropertyChanged("CheckListHeader");
            OnPropertyChanged("CheckListSubHeader");
            OnPropertyChanged("ShowChecklist");
            OnPropertyChanged("Checklists");
           // Checklists.Clear();
            if (Checklists != null && Checklists.Count == 0)
            {
                Device.BeginInvokeOnMainThread(() => { DisplaySnackBar("No checklists found", PageActions.PopAsync, MessageActions.Warning, null, null); });
            }
            IsLoading = false;
        }

        public JobChecklistVM() : base()
        {
        }

    }
}

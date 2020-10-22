using astorWorkMobile.JobTrack.Entities;
using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.MaterialTrack.Pages;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using static astorWorkMobile.Shared.Classes.Enums;

namespace astorWorkMobile.MaterialTrack.ViewModels
{
    public class SiteListDefectsVM : MasterVM
    {
        public int MyProperty { get; set; }
        public Material Material { get; set; }
        public PPVCJob Job { get; set; }
        public bool DisplayButton
        {
            get
            {
                if (Convert.ToInt32(Application.Current.Properties["entry_point"]) == 5)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public async void InitialiseDefectPage(QCDefect defect)
        {
            ViewModelLocator.qcDefectVM.QCOpenCase = OpenCase;
            ViewModelLocator.qcDefectVM.QCDefectDetails = (defect != null) ? defect : new QCDefect
            {
                IsOpen = true,
                ID = 0
            };
            ViewModelLocator.qcDefectVM.QCPhotos.Clear();
            if (defect == null)
            {
                GetOrganisations();
            }
            await Navigation.PushAsync(new DefectDetails());
        }
        public async Task GetOrganisations()
        {
            var result = await ApiClient.Instance.MTGetOrganisations();//.ContinueWith(orgs =>
            if (result.status == 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ViewModelLocator.qcDefectVM.ListSubcons = result.data as List<Organisation>;
                    ViewModelLocator.qcDefectVM.ListSubcons = ViewModelLocator.qcDefectVM.ListSubcons
                                                             .Where(o => o.OrganisationType == OrganisationType.Subcon
                                                                      || o.OrganisationType == OrganisationType.Vendor)
                                                             .ToList();
                });
            }
        }
        public async Task<bool> GetQCCaseDetails()
        {
            bool success = false;
            if (Material != null)
            {
                var result = await ApiClient.Instance.MTGetQCCases(Material.id);
                if (result.data != null)
                {
                    try
                    {
                        var listCases = result.data as List<QCCase>;
                        OpenCase = listCases.Find(c => c.IsOpen);

                        if (OpenCase == null)
                            ErrorMessage = "Cannot find any open QC case for this material";
                        else
                            success = true;
                    }
                    catch (Exception exc)
                    {
                        ErrorMessage = exc.Message;
                    }
                }
                else
                    ErrorMessage = result.message;
            }
            else
                ErrorMessage = "No material specified";

            return success;
        }

        private QCCase _openCase;
        public QCCase OpenCase
        {
            get
            {
                return _openCase;
            }
            set
            {
                _openCase = value;
                RefreshCase();
                OnPropertyChanged("OpenCase");
            }
        }

        public void RefreshCase()
        {
            if (_openCase != null)
                Task.Run(_openCase.RefreshDefects);
        }

        public override void Reset()
        {
            base.Reset();
            Material = null;
            OpenCase = null;
        }
    }
}

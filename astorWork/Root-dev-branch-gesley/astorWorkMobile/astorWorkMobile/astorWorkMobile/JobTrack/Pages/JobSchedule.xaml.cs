using astorWorkMobile.JobTrack.Entities;
using astorWorkMobile.JobTrack.ViewModels;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static astorWorkMobile.Shared.Classes.Enums;

namespace astorWorkMobile.JobTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JobSchedule : ContentPage
    {
        string Status;
        bool IsScanBtnsVisible;
        int pageSize = 15;
        int materialSize;
        public ObservableCollection<string> Items { get; set; }

        public JobSchedule(string status)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");
            Status = status;
        }
        public JobSchedule()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            ViewModelLocator.jobScheduleVM.JobSchedules = new ObservableCollection<JobScheduleDetails>();
            ViewModelLocator.jobScheduleVM.IsLoading = true;
            Title = Status != "6" ? ((JobStatus)Convert.ToInt32(Status)).ToString().Replace('_', ' ') + " Jobs" : "Jobs in QC";
            if (Convert.ToInt32(Status) <= 5)
            {
                Title = ((JobStatus)Convert.ToInt32(Status)).ToString().Replace('_', ' ');
            }
            if (Status == "5")
            {
                Title = "QC Pending Jobs";
            }
            else if (Status == "4")
            {
                Title = "Ongoing Jobs";
            }
            else if (Status == "3")
            {
                Title = "Delayed Jobs";
            }
            else if (Status == "0")
            {
                Title = "Jobs Not Assigned";
            }
            List<JobScheduleDetails> _jobs = new List<JobScheduleDetails>();
            ApiClient.Instance.MTGetOrganisations().ContinueWith(async (t) =>
            {
                if (t.Result.status == 0)
                {
                    await ApiClient.Instance.JTGetJobSchedule(0, 0, pageSize, null, null, null, null, int.Parse(Status)).ContinueWith(p =>
                            {
                                if (p.Result.status == 0)
                                {
                                    _jobs = p.Result.data as List<JobScheduleDetails>;
                                    if (_jobs != null)
                                    {
                                        ViewModelLocator.jobScheduleVM.ListMaterialJobSchedule =
                                        _jobs.OrderBy(j => j.StatusCode).Take(pageSize).ToList();
                                    }
                                }
                            });
                    ViewModelLocator.jobScheduleVM.ListSubcons = t.Result.data as List<Organisation>;
                    ViewModelLocator.jobScheduleVM.ModuleName = string.Empty;
                    ViewModelLocator.jobScheduleVM.ListSubcons = ViewModelLocator.jobScheduleVM.ListSubcons.Where(o => o.OrganisationType == OrganisationType.Subcon).ToList();
                    ViewModelLocator.jobScheduleVM.IsLoading = false;
                }
                else
                {
                    ViewModelLocator.jobScheduleVM.ErrorMessage = t.Result.message;
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var vm = BindingContext as JobScheduleVM;
            vm.QCButtonClicked();
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            ViewModelLocator.jobScheduleVM.DisplaySnackBar("Job Assigned Successfully", Enums.PageActions.None, Enums.MessageActions.Success, null, null);

        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DummyMainPage());
        }

        private void ListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var item = e.Item as JobScheduleDetails;
            var index = ViewModelLocator.jobScheduleVM.JobSchedules.IndexOf(item);
            if (ViewModelLocator.jobScheduleVM.JobSchedules.Count == index + 1)
            {
                ViewModelLocator.jobScheduleVM.IsRunning = true;
                ViewModelLocator.jobScheduleVM.OnPropertyChanged("IsRunning");
                Task.Run(() =>
                {
                    ApiClient.Instance.JTGetJobSchedule(0, ViewModelLocator.jobScheduleVM.JobSchedules.Count, pageSize, ViewModelLocator.jobScheduleVM.SelectedBlock, ViewModelLocator.jobScheduleVM.SelectedLevel, ViewModelLocator.jobScheduleVM.SelectedMarkingNo, null, int.Parse(Status)).ContinueWith(p =>
                    {
                        if (p.Result.status == 0)
                        {
                            var _jobs = p.Result.data as List<JobScheduleDetails>;
                            if (_jobs != null && _jobs.Count != 0)
                            {
                                var list = _jobs.OrderBy(j => j.StatusCode).Take(pageSize).ToList();
                                ViewModelLocator.jobScheduleVM.ListMaterialJobSchedule.AddRange(list);
                                //   ViewModelLocator.jobScheduleVM.ListMaterialJobSchedule = ViewModelLocator.jobScheduleVM.ListMaterialJobSchedule;
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    ViewModelLocator.jobScheduleVM.IsRunning = false;
                                    //ViewModelLocator.jobScheduleVM.OnPropertyChanged("JobSchedules");
                                    ViewModelLocator.jobScheduleVM.OnPropertyChanged("IsRunning");
                                    ViewModelLocator.jobScheduleVM.JobSchedules = new ObservableCollection<JobScheduleDetails>(ViewModelLocator.jobScheduleVM.ListMaterialJobSchedule);
                                    ViewModelLocator.jobScheduleVM.ListBlocks = ViewModelLocator.jobScheduleVM.ListMaterialJobSchedule.OrderBy(js => js.Block).Select(js => js.Block).Distinct().ToList();
                                    ViewModelLocator.jobScheduleVM.OnPropertyChanged("ListBlocks");
                                    ViewModelLocator.jobScheduleVM.OnPropertyChanged("JobSchedules");
                                });
                            }
                            else
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    ViewModelLocator.jobScheduleVM.IsRunning = false;
                                    // ViewModelLocator.jobScheduleVM.OnPropertyChanged("JobSchedules");
                                    ViewModelLocator.jobScheduleVM.OnPropertyChanged("IsRunning");
                                });
                            }
                        }
                        else
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                ViewModelLocator.jobScheduleVM.IsRunning = false;
                                // ViewModelLocator.jobScheduleVM.OnPropertyChanged("JobSchedules");
                                ViewModelLocator.jobScheduleVM.OnPropertyChanged("IsRunning");
                            });
                        }
                    });
                    //Device.BeginInvokeOnMainThread(() =>
                    //{
                    //    ViewModelLocator.jobScheduleVM.IsRunning = false;
                    //    ViewModelLocator.jobScheduleVM.OnPropertyChanged("JobSchedules");
                    //    ViewModelLocator.jobScheduleVM.OnPropertyChanged("IsRunning");
                    //});
                });

            }
        }
    }
}

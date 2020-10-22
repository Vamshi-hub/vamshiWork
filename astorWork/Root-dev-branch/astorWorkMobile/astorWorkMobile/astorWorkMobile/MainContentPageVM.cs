using astorWorkMobile.JobTrack.Pages;
using astorWorkMobile.MaterialTrack.Pages;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace astorWorkMobile
{
    public class MasterPageItem
    {
        public string Icon { get; set; }
        public string Title { get; set; }
        public string ModuleName { get; set; }
        public bool ShowItem
        {
            get
            {
                object enabledModules = string.Empty;
                Application.Current.Properties.TryGetValue("enabled_modules", out enabledModules);
                enabledModules = enabledModules == null ? "" : enabledModules;

                return (string.IsNullOrEmpty(ModuleName) || (enabledModules as string).Contains(ModuleName));
            }
        }
        public ICommand TargetCommand { get; set; }
    }

    public class MainContentPageVM : MasterVM
    {
        public MasterDetailPage MasterDetailPage { get; set; }
        // public Page DetailPage { get; set; }
        public int MobileEntryPoint
        {
            get
            {
                if (Application.Current.Properties.ContainsKey("entry_point"))
                {
                    return (int)Application.Current.Properties["entry_point"];
                }
                else
                {
                    return -1;
                }
            }
        }

        public string WelcomeMessage
        {
            get
            {
                var userName = Application.Current.Properties["user_name"] as string;

                return $"Hi, {userName}";
            }
        }

        public ICommand MTButtonCommand { get; set; }

        private void MTButtonClicked()
        {
            ViewModelLocator.jobChecklistVM.Job = new JobTrack.Entities.JobScheduleDetails();
            ViewModelLocator.vendorHomeVM.IsMCStructuralInsp = false;
            ViewModelLocator.jobChecklistVM.IsArchitechtural = false;
            ViewModelLocator.jobChecklistVM.IsStructural = true;
            switch (MobileEntryPoint)
            {
                case 0:
                    ViewModelLocator.vendorHomeVM.ProjectID = -1;
                    MasterDetailPage.Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(VendorHome)));
                    MasterDetailPage.IsPresented = false;
                    // Navigation.PushAsync(new VendorHome());
                    //   OnPropertyChanged("Navigated");
                    break;
                case 1:
                    MasterDetailPage.Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(MaterialTrack.Pages.MainconHome)));
                    MasterDetailPage.IsPresented = false;
                    //  Navigation.PushAsync(new MaterialTrack.Pages.MainconHome());
                    //  OnPropertyChanged("Navigated");
                    break;
                case 2:
                    MasterDetailPage.Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(MaterialTrack.Pages.MainconHome)));
                    MasterDetailPage.IsPresented = false;
                    //  Navigation.PushAsync(new MaterialTrack.Pages.MainconHome());
                    //  OnPropertyChanged("Navigated");
                    break;
                case 3:
                    MasterDetailPage.Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(MTRTOHome)));
                    MasterDetailPage.IsPresented = false;
                    //  Navigation.PushAsync(new MTRTOHome());
                    OnPropertyChanged("Navigated");
                    break;
                case 4:
                    MasterDetailPage.Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(MaterialTrack.Pages.MainconHome)));
                    MasterDetailPage.IsPresented = false;
                    // Navigation.PushAsync(new MaterialTrack.Pages.MainconHome());
                    //  OnPropertyChanged("Navigated");
                    break;
                case 5:
                    MasterDetailPage.Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(MaterialTrack.Pages.MainconHome)));
                    MasterDetailPage.IsPresented = false;
                    // Navigation.PushAsync(new MaterialTrack.Pages.MainconHome());
                    //  OnPropertyChanged("Navigated");
                    break;
                default:
                    //  MainPage.DisplayAlert("Alert", "Sorry you don't have access to this feature", "OK");
                    //   OnPropertyChanged("Navigated");
                    MasterDetailPage.IsPresented = false;
                    DisplaySnackBar("Sorry you don't have access to this feature", Enums.PageActions.None, Enums.MessageActions.Warning, null, null);
                    break;
            }

        }

        public ICommand JTButtonCommand { get; set; }

        private void JTButtonClicked()
        {
            ViewModelLocator.vendorHomeVM.IsMCStructuralInsp = false;
            ViewModelLocator.jobChecklistVM.IsArchitechtural = true;
            ViewModelLocator.jobChecklistVM.IsStructural = false;
            switch (MobileEntryPoint)
            {
                case 1:

                    MasterDetailPage.Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(JobTrack.Pages.MainconHome)));
                    MasterDetailPage.IsPresented = false;
                    // Navigation.PushAsync(new JobTrack.Pages.MainconHome());
                    // OnPropertyChanged("Navigated");
                    break;
                case 2:
                    MasterDetailPage.Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(SubconHome)));
                    MasterDetailPage.IsPresented = false;
                    //  Navigation.PushAsync(new SubconHome());
                    //  OnPropertyChanged("Navigated");
                    break;
                case 3:
                    //ViewModelLocator.jobChecklistVM.SignatureImageSource = null;
                    //ViewModelLocator.jobChecklistVM._signatureImageSource = null;
                    MasterDetailPage.Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(RTOHome)));
                    MasterDetailPage.IsPresented = false;
                    // Navigation.PushAsync(new RTOHome());
                    // OnPropertyChanged("Navigated");
                    break;
                case 4:
                    //ViewModelLocator.jobChecklistVM.SignatureImageSource = null;
                    //ViewModelLocator.jobChecklistVM._signatureImageSource = null;
                    MasterDetailPage.Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(JobTrack.Pages.MainconHome)));
                    MasterDetailPage.IsPresented = false;
                    // Navigation.PushAsync(new JobTrack.Pages.MainconHome());
                    // OnPropertyChanged("Navigated");
                    break;
                case 5:
                    //ViewModelLocator.jobChecklistVM.SignatureImageSource = null;
                    //ViewModelLocator.jobChecklistVM._signatureImageSource = null;
                    MasterDetailPage.Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(JobTrack.Pages.MainconHome)));
                    MasterDetailPage.IsPresented = false;
                    // Navigation.PushAsync(new JobTrack.Pages.MainconHome());
                    // OnPropertyChanged("Navigated");
                    break;
                default:
                    // OnPropertyChanged("Navigated");
                    MasterDetailPage.IsPresented = false;
                    DisplaySnackBar("Sorry you don't have access to this feature", Enums.PageActions.None, Enums.MessageActions.Warning, null, null);
                    break;
            }
        }

        public ICommand LogoutButtonCommand { get; set; }

        private void LogoutButtonClicked()
        {
            OnPropertyChanged("Navigated");
            ViewModelLocator.loginVM.Reset();
            ViewModelLocator.vendorHomeVM.Reset();
            Application.Current.Properties.Clear();
            Application.Current.SavePropertiesAsync();
            ApiClient.Instance.ResetClient();
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }

        public List<MasterPageItem> MasterPageItems { get; set; }

        public MainContentPageVM()
        {
            MTButtonCommand = new Command(MTButtonClicked);
            JTButtonCommand = new Command(JTButtonClicked);
            LogoutButtonCommand = new Command(LogoutButtonClicked);

            MasterPageItems = new List<MasterPageItem>
            {
                new MasterPageItem
                {
                    Icon = "ic_delivery_accent.png",
                    Title = "Material Tracking",
                    ModuleName = "material-tracking",
                    TargetCommand = MTButtonCommand
                },
                new MasterPageItem
                {
                    Icon = "ic_job_accent.png",
                    Title = "Job Tracking",
                    ModuleName = "job-tracking",
                    TargetCommand = JTButtonCommand
                },
                new MasterPageItem
                {
                    Icon = "ic_logout_accent.png",
                    Title = "Log Out",
                    TargetCommand = LogoutButtonCommand
                }
            };
            OnPropertyChanged("MasterPageItems");
        }
    }
}

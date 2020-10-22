using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DummyMainPage : TabbedPage
    {
        public DummyMainPage()
        {
            InitializeComponent();
            this.Children[0].Title = "Job";// + " (" + 0 + ")";
            this.Children[1].Title = "Material";// + " (" + 0 + ")";
            switch (ViewModelLocator.mainContentPageVM.MobileEntryPoint)
            {
                case 0:
                    this.Children.Remove(this.Children[0]);
                    break;
            }
        }

        private async void JObNotificationPage_Appearing(object sender, EventArgs e)
        {
            await Task.Run(ViewModelLocator.notificationVM.GetNotifications).ContinueWith(async (t) =>
             {
                 if (t.Result)
                 {
                     //if (ViewModelLocator.notificationVM.JObNotifications != null && ViewModelLocator.notificationVM.JObNotifications.Count > 0)
                     //{
                     //    int count = ViewModelLocator.notificationVM.JObNotifications.Where(p => p.IsSeen == false).Count();
                     //    Task.Run(() => { Device.BeginInvokeOnMainThread(() => { this.Children[0].Title = "Job" + " (" + count + ")"; }); });
                     //}
                     //if (ViewModelLocator.notificationVM.MaterailNotifications != null && ViewModelLocator.notificationVM.MaterailNotifications.Count > 0)
                     //{
                     //    int count = ViewModelLocator.notificationVM.MaterailNotifications.Where(p => p.IsSeen == false).Count();
                     //    Task.Run(() => { Device.BeginInvokeOnMainThread(() => { this.Children[1].Title = "Material" + " (" + count + ")"; }); });
                     //}

                     if (ViewModelLocator.notificationVM.JObNotifications != null && ViewModelLocator.notificationVM.JObNotifications.Count > 0)
                     {
                         if (ViewModelLocator.notificationVM.JObNotifications.Where(p => p.IsSeen == false).Count() > 0)
                         {
                             await Task.Run(() =>
                              {
                                  ViewModelLocator.notificationVM.UpdateSeenBy(Enums.NotificationType.Job_Notification).ContinueWith((s) =>
                                  {
                                      //if (s.Result)
                                      //{
                                      //    if (ViewModelLocator.notificationVM.JObNotifications != null && ViewModelLocator.notificationVM.JObNotifications.Count > 0)
                                      //    {
                                      //        int count = ViewModelLocator.notificationVM.JObNotifications.Where(p => p.IsSeen == false).Count();
                                      //        Task.Run(() =>
                                      //        {
                                      //            Device.BeginInvokeOnMainThread(() => { this.Children[0].Title = "Job" + " (" + count + ")"; });
                                      //        });
                                      //    }
                                      //    if (ViewModelLocator.notificationVM.MaterailNotifications != null && ViewModelLocator.notificationVM.MaterailNotifications.Count > 0)
                                      //    {
                                      //        int count = ViewModelLocator.notificationVM.MaterailNotifications.Where(p => p.IsSeen == false).Count();
                                      //        Task.Run(() =>
                                      //        {
                                      //            Device.BeginInvokeOnMainThread(() => { this.Children[1].Title = "Material" + " (" + count + ")"; });
                                      //        });
                                      //    }
                                      //}
                                  });
                              });
                         }
                     }

                 }
             });
            ViewModelLocator.notificationVM.OnPropertyChanged("JObNotifications");
            ViewModelLocator.notificationVM.OnPropertyChanged("MaterailNotifications");
        }

        private async void MaterialNotificationPage_Appearing(object sender, EventArgs e)
        {
            await Task.Run(ViewModelLocator.notificationVM.GetNotifications).ContinueWith(async (t) =>
             {
                 if (t.Result)
                 {
                     //if (ViewModelLocator.notificationVM.JObNotifications != null && ViewModelLocator.notificationVM.JObNotifications.Count > 0)
                     //{
                     //    int count = ViewModelLocator.notificationVM.JObNotifications.Where(p => p.IsSeen == false).Count();
                     // //   Task.Run(() => { Device.BeginInvokeOnMainThread(() => { this.Children[0].Title = "Job" + " (" + count + ")"; }); });
                     //}
                     //if (ViewModelLocator.notificationVM.MaterailNotifications != null && ViewModelLocator.notificationVM.MaterailNotifications.Count > 0)
                     //{
                     //    int count = ViewModelLocator.notificationVM.MaterailNotifications.Where(p => p.IsSeen == false).Count();
                     //   // Task.Run(() => { Device.BeginInvokeOnMainThread(() => { this.Children[1].Title = "Material" + " (" + count + ")"; }); });
                     //}
                     if (ViewModelLocator.notificationVM.MaterailNotifications != null && ViewModelLocator.notificationVM.MaterailNotifications.Count > 0)
                     {
                         if (ViewModelLocator.notificationVM.MaterailNotifications.Where(p => p.IsSeen == false).Count() > 0)
                         {
                             await Task.Run(() =>
                              {
                                  ViewModelLocator.notificationVM.UpdateSeenBy(Enums.NotificationType.Material_Notification).ContinueWith((p) =>
                                  {
                                      //if (p.Result)
                                      //{
                                      //    if (ViewModelLocator.notificationVM.MaterailNotifications != null && ViewModelLocator.notificationVM.MaterailNotifications.Count > 0)
                                      //    {
                                      //        int count = ViewModelLocator.notificationVM.MaterailNotifications.Where(s => s.IsSeen == false).Count();
                                      //     //   Device.BeginInvokeOnMainThread(() => { this.Children[1].Title = "Material" + " (" + count + ")"; });
                                      //    }
                                      //    if (ViewModelLocator.notificationVM.JObNotifications != null && ViewModelLocator.notificationVM.JObNotifications.Count > 0)
                                      //    {
                                      //        int count = ViewModelLocator.notificationVM.JObNotifications.Where(s => s.IsSeen == false).Count();
                                      //        Device.BeginInvokeOnMainThread(() => { this.Children[0].Title = "Job" + " (" + count + ")"; });
                                      //    }

                                      //}
                                  });
                              });
                         }
                     }
                 }
             });
            ViewModelLocator.notificationVM.OnPropertyChanged("MaterailNotifications");
            ViewModelLocator.notificationVM.OnPropertyChanged("JObNotifications");
        }
    }
}
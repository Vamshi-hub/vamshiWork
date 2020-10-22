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
        //public bool MatNotify { get; set; } = true;
        //static ObservableCollection<MyNotifyTypes> StaticNotifyTypes = new ObservableCollection<MyNotifyTypes>();
        //ObservableCollection<MyNotifyTypes> NotifyTypes = new ObservableCollection<MyNotifyTypes>();
        //MyNotifyTypes notifies = new MyNotifyTypes();
        public DummyMainPage()
        {
            InitializeComponent();
            this.Children[0].Title = "Job" + " (" + 0 + ")";
            this.Children[1].Title = "Material" + " (" + 0 + ")";
        }
        //private void Tab1_Clicked(object sender, EventArgs e)
        //{
        //    // manipulate the list and bind  myCarousel.ItemsSource here for Job Notifications(Type=1)
        //    JObTab.BackgroundColor = Color.Red;
        //    MaterialTab.BackgroundColor = Color.Transparent;
        //    myCarousel.Position = 0;
        //}

        //private void Tab2_Clicked(object sender, EventArgs e)
        //{
        //    // manipulate the list and bind  myCarousel.ItemsSource here for Material Notifications(Type=2)
        //    JObTab.BackgroundColor = Color.Transparent;
        //    MaterialTab.BackgroundColor = Color.Red;
        //    myCarousel.Position = 1;
        //}

        private void JObNotificationPage_Appearing(object sender, EventArgs e)
        {
            //JObTab.BackgroundColor = Color.Red;
            //MaterialTab.BackgroundColor = Color.Transparent;
            // myCarousel.Position = 0;
            Task.Run(ViewModelLocator.notificationVM.GetNotifications).ContinueWith((t) =>
            {
                if (t.Result)
                {
                    if (ViewModelLocator.notificationVM.JObNotifications != null && ViewModelLocator.notificationVM.JObNotifications.Count > 0)
                    {
                        int count = ViewModelLocator.notificationVM.JObNotifications.Where(p => p.IsSeen == false).Count();
                        this.Children[0].Title = "Job" + " (" + count + ")";
                    }
                    if (ViewModelLocator.notificationVM.MaterailNotifications != null && ViewModelLocator.notificationVM.MaterailNotifications.Count > 0)
                    {
                        int count = ViewModelLocator.notificationVM.MaterailNotifications.Where(p => p.IsSeen == false).Count();
                        this.Children[1].Title = "Material" + " (" + count + ")";
                    }
                    if (ViewModelLocator.notificationVM.JObNotifications != null && ViewModelLocator.notificationVM.JObNotifications.Count > 0)
                    {
                        if (ViewModelLocator.notificationVM.JObNotifications.Where(p => p.IsSeen == false).Count() > 0)
                        {
                            Task.Run(() =>
                            {
                                ViewModelLocator.notificationVM.UpdateSeenBy(Enums.NotificationType.Job_Notification).ContinueWith((s) =>
                                {
                                    if (s.Result)
                                    {
                                        if (ViewModelLocator.notificationVM.JObNotifications != null && ViewModelLocator.notificationVM.JObNotifications.Count > 0)
                                        {
                                            int count = ViewModelLocator.notificationVM.JObNotifications.Where(p => p.IsSeen == false).Count();
                                            this.Children[0].Title = "Job" + " (" + count + ")";
                                        }
                                        if (ViewModelLocator.notificationVM.MaterailNotifications != null && ViewModelLocator.notificationVM.MaterailNotifications.Count > 0)
                                        {
                                            int count = ViewModelLocator.notificationVM.MaterailNotifications.Where(p => p.IsSeen == false).Count();
                                            this.Children[1].Title = "Material" + " (" + count + ")";
                                        }
                                    }
                                });
                            });
                        }
                    }
                }
            });
            if (ViewModelLocator.notificationVM.JObNotifications != null && ViewModelLocator.notificationVM.JObNotifications.Count > 0)
            {
                int count = ViewModelLocator.notificationVM.JObNotifications.Where(p => p.IsSeen == false).Count();
                this.Children[0].Title = "Job" + " (" + count + ")";
            }
            if (ViewModelLocator.notificationVM.MaterailNotifications != null && ViewModelLocator.notificationVM.MaterailNotifications.Count > 0)
            {
                int count = ViewModelLocator.notificationVM.MaterailNotifications.Where(p => p.IsSeen == false).Count();
                this.Children[1].Title = "Material" + " (" + count + ")";
            }
        }

        private void MaterialNotificationPage_Appearing(object sender, EventArgs e)
        {
            if (ViewModelLocator.notificationVM.MaterailNotifications != null && ViewModelLocator.notificationVM.MaterailNotifications.Count > 0)
            {
                if (ViewModelLocator.notificationVM.MaterailNotifications.Where(p => p.IsSeen == false).Count() > 0)
                {
                    Task.Run(() =>
                    {
                        ViewModelLocator.notificationVM.UpdateSeenBy(Enums.NotificationType.Material_Notification).ContinueWith((t) =>
                        {
                            if (t.Result)
                            {
                                if (ViewModelLocator.notificationVM.MaterailNotifications != null && ViewModelLocator.notificationVM.MaterailNotifications.Count > 0)
                                {
                                    int count = ViewModelLocator.notificationVM.MaterailNotifications.Where(p => p.IsSeen == false).Count();
                                    this.Children[1].Title = "Material" + " (" + count + ")";
                                }
                                if (ViewModelLocator.notificationVM.JObNotifications != null && ViewModelLocator.notificationVM.JObNotifications.Count > 0)
                                {
                                    int count = ViewModelLocator.notificationVM.JObNotifications.Where(p => p.IsSeen == false).Count();
                                    this.Children[0].Title = "Job" + " (" + count + ")";
                                }

                            }
                        });
                    });
                }
            }
            if (ViewModelLocator.notificationVM.JObNotifications != null && ViewModelLocator.notificationVM.JObNotifications.Count > 0)
            {
                int count = ViewModelLocator.notificationVM.JObNotifications.Where(p => p.IsSeen == false).Count();
                this.Children[0].Title = "Job" + " (" + count + ")";
            }
            if (ViewModelLocator.notificationVM.MaterailNotifications != null && ViewModelLocator.notificationVM.MaterailNotifications.Count > 0)
            {
                int count = ViewModelLocator.notificationVM.MaterailNotifications.Where(p => p.IsSeen == false).Count();
                this.Children[1].Title = "Material" + " (" + count + ")";
            }
            //JObTab.BackgroundColor = Color.Transparent;
            //MaterialTab.BackgroundColor = Color.Red;
            //myCarousel.Position = 1;
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DummyMainPage());
        }
    }
}
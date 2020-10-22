using FormsPlugin.Iconize;
using MaterialTrackApp.Entities;
using MaterialTrackApp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTrackApp.PartialView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnrollTaskView : ContentView
    {
        public EnrollTaskView()
        {
            InitializeComponent();
        }

        private void EntryMarkingNo_Focused(object sender, FocusEventArgs e)
        {
            var entry = sender as Entry;
            //entry.Unfocus();
            var task = entry.BindingContext as EnrollTaskEntity;

            ViewModelLocator.filterMarkingNoVM.Project = string.Empty;

            var selectPage = new FilterMarkingNoPage(task);
            App.mainPage.Navigation.PushPageAsync(selectPage);
        }

        private void EntryMRFNo_Focused(object sender, FocusEventArgs e)
        {
            var entry = sender as Entry;
            //entry.Unfocus();
            var task = entry.BindingContext as EnrollTaskEntity;

            if (string.IsNullOrEmpty(task.MarkingNo))
            {
                App.mainPage.DisplayAlert("Warning", "You must select a marking no. first", "OK");
            }
            else
            {
                ViewModelLocator.filterMRFNoVM.Project = task.Project;
                ViewModelLocator.filterMRFNoVM.MarkingNo = task.MarkingNo;

                var selectPage = new FilterMRFNoPage(task);
                App.mainPage.Navigation.PushPageAsync(selectPage);
            }
        }

        private void ClearMRFNo_Tapped(object sender, EventArgs e)
        {
            var img = sender as IconImage;
            var task = img.BindingContext as EnrollTaskEntity;
            task.MRFNo = null;
        }

        private void ClearMarkingNo_Tapped(object sender, EventArgs e)
        {
            var img = sender as IconImage;
            var task = img.BindingContext as EnrollTaskEntity;
            task.MarkingNo = null;
        }
    }
}
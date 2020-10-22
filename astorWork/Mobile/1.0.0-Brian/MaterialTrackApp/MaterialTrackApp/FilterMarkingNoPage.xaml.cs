using MaterialTrackApp.DB;
using MaterialTrackApp.Entities;
using MaterialTrackApp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTrackApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilterMarkingNoPage : ContentPage
    {
        private EnrollTaskEntity _task;
        public FilterMarkingNoPage(EnrollTaskEntity task)
        {
            InitializeComponent();
            _task = task;
        }

        private async void listTasks_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if(e.SelectedItem != null)
            {
                _task.Project = ViewModelLocator.filterMarkingNoVM.Project;
                _task.MarkingNo = e.SelectedItem as string;
                await Navigation.PopAsync();
            }
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            if(_task != null && picker != null && picker.SelectedIndex >= 0)
            {
                _task.Project = ViewModelLocator.filterMarkingNoVM.Project;
                _task.MarkingNo = ViewModelLocator.filterMarkingNoVM.MarkingNo;
                Navigation.PopAsync();
            }
        }
    }
}
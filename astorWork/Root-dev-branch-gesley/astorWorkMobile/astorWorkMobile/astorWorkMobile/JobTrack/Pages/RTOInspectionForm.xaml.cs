using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace astorWorkMobile.JobTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RTOInspectionForm : ContentPage
    {
        public RTOInspectionForm()
        {
            var aaaa = new RTOCheckList() { Name = "This is a test checklist which has to be inspected by the RTO person." };
            
            //ViewModelLocator.rtoInspectionVM.ListInspectionForm.Add(new RTOInspection() { DrawingNo = "DRAW12" });
            //ViewModelLocator.rtoInspectionVM.ListInspectionForm.Add(new RTOInspection() { DrawingNo = "DRAW12" });
            //ViewModelLocator.rtoInspectionVM.ListInspectionForm.Add(new RTOInspection() { DrawingNo = "DRAW12" });
            //ViewModelLocator.rtoInspectionVM.ListInspectionForm.Add(new RTOInspection() { DrawingNo = "DRAW12" });
            //ViewModelLocator.rtoInspectionVM.ListInspectionForm[0].CheckLists.Add(aaaa);
            //ViewModelLocator.rtoInspectionVM.ListInspectionForm[1].CheckLists.Add(aaaa);
            //ViewModelLocator.rtoInspectionVM.ListInspectionForm[2].CheckLists.Add(aaaa);
            InitializeComponent();
            // ViewModelLocator.rtoInspectionVM.CheckLists = new List<RTOCheckList>();
            // ViewModelLocator.rtoInspectionVM.OnPropertyChanged("CheckLists");
            ViewModelLocator.rtoInspectionVM.IsLoading = true;
            ApiClient.Instance.INGetInspectionForm(ViewModelLocator.rtoInspectionVM.JobScheduleID).ContinueWith(t =>
            {
                if (t.Result.status == 0)
                {
                    ViewModelLocator.rtoInspectionVM.IsLoading = false;
                    ViewModelLocator.rtoInspectionVM.ListInspectionForm = t.Result.data as List<RTOInspection>;
                }
                else
                {
                    ViewModelLocator.rtoInspectionVM.ErrorMessage = t.Result.message;
                }
            });
            Task.Run(ViewModelLocator.rtoInspectionVM.GetRTOs);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
        private void picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ccc = ((Picker)sender).SelectedItem.ToString();
            //  ViewModelLocator.rtoInspectionVM.RTOInspectionForm.ModuleName;
        }
    }
}
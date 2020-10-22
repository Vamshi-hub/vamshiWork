using astorWorkMobile.JobTrack.Entities;
using astorWorkMobile.JobTrack.Pages;
using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.MaterialTrack.Pages;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace astorWorkMobile.Shared.Classes
{
    public class NotificationDetails : MasterVM
    {
        public int JobScheduleID { get; set; }
        public int MaterialID { get; set; }
        public string Tag { get; set; }
        public string Block { get; set; }
        public string Level { get; set; }
        public string Zone { get; set; }
        public string MarkingNo { get; set; }
        public int ProjectID { get; set; }
        public Project Project { get; set; }
        public bool IsSeen { get; set; }
        public int NotificationID { get; set; }
        public int NotificationCode { get; set; }
        public string Message { get; set; }
        public string TradeName { get; set; }
        public string AssaignedTo { get; set; }
        public string MaterialType { get; set; }
        public string CreatedBy { get; set; }
        public bool IsJob { get; set; }
        public bool IsMaterial { get; set; }
        public DateTimeOffset ProcessDate { get; set; }
        public string ProcessedDate => DateTime.Now.ToString();
        public string ModuleName => $"{Block}-L{Level}-{Zone}-{MarkingNo}";
        public FontAttributes _fontAttribute { get; set; }
        public FontAttributes FontAttribute
        {
            get
            {
                if (!IsSeen)
                {
                    _fontAttribute = FontAttributes.Bold;
                }
                else
                {
                    _fontAttribute = FontAttributes.None;
                }
                return _fontAttribute;
            }
        }
        public Color _textColor { get; set; } = Color.Black;
        public Color TextColor
        {
            get
            {
                if (!IsSeen)
                {
                    _textColor = Color.FromHex("#1769bb");
                }
                else
                {
                    _textColor = Color.Black;
                }
                return _textColor;
            }
        }
        public List<NotificationDetails> lstNotification { get; set; }
        public ICommand NotifyClickedCommand { get; set; }
        public async void NotificationClicked(NotificationDetails notificationDetails)
        {
            ViewModelLocator.notificationVM.IsLoading = true;
            if (notificationDetails.NotificationCode >= (int)Enums.NotificationCode.JobQCPassed && notificationDetails.NotificationCode <= (int)Enums.NotificationCode.JobQCRejected)
            {
                ViewModelLocator.jobChecklistVM.IsArchitechtural = true;
                ViewModelLocator.jobChecklistVM.IsStructural = false;
                if (!string.IsNullOrEmpty(notificationDetails.Tag))
                {
                    await ViewModelLocator.jobScanVM.GetJobScheduleByTracker(notificationDetails.Tag);
                }
                else
                {

                }
                var job = ViewModelLocator.jobScanVM.Jobs.Where(p => p.MaterialID == notificationDetails.MaterialID && p.TradeName == notificationDetails.TradeName).FirstOrDefault();
                ViewModelLocator.jobChecklistVM.Job = job;
                ViewModelLocator.notificationVM.IsLoading = false;
                await Navigation.PushAsync(new JobChecklist());
            }
            else if (notificationDetails.NotificationCode == (int)Enums.NotificationCode.MaterialQCFailed || notificationDetails.NotificationCode == (int)Enums.NotificationCode.MaterialQCPassed)
            {
                ViewModelLocator.jobChecklistVM.IsArchitechtural = false;
                ViewModelLocator.jobChecklistVM.IsStructural = true;
                await ViewModelLocator.scanTrackerVM.GetTrackerAssociations(new string[] { notificationDetails.Tag });
                ViewModelLocator.jobChecklistVM.Material = ViewModelLocator.scanTrackerVM.ListMaterialItems.FirstOrDefault().Material;
                ViewModelLocator.notificationVM.IsLoading = false;
                await Navigation.PushAsync(new JobChecklist());
            }
            else if (notificationDetails.NotificationCode == (int)Enums.NotificationCode.QCFailed || notificationDetails.NotificationCode == (int)Enums.NotificationCode.QCRectified)
            {
                ViewModelLocator.jobChecklistVM.IsArchitechtural = false;
                ViewModelLocator.jobChecklistVM.IsStructural = true;
                await ViewModelLocator.scanTrackerVM.GetTrackerAssociations(new string[] { notificationDetails.Tag });
                ViewModelLocator.siteListDefectsVM.Material = ViewModelLocator.scanTrackerVM.ListMaterialItems.FirstOrDefault().Material;
                ViewModelLocator.qcDefectVM.Material = ViewModelLocator.siteListDefectsVM.Material;
                ViewModelLocator.notificationVM.IsLoading = false;
                await Navigation.PushAsync(new ListDefects());
            }
            else
            {
                ViewModelLocator.notificationVM.IsLoading = false;
            }
        }
        public NotificationDetails()
        {
            NotifyClickedCommand = new Command<NotificationDetails>(NotificationClicked);
        }
    }
}

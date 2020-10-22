using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static astorWorkMobile.Shared.Classes.Enums;

namespace astorWorkMobile.Shared.Classes
{
    public class NotificationVM : MasterVM
    {
        //public bool MatNotify { get; set; } = true;
        //static ObservableCollection<MyNotifyTypes> StaticNotifyTypes = new ObservableCollection<MyNotifyTypes>();
        public List<NotificationDetails> lstNotification { get; set; }
        public List<NotificationDetails> JObNotifications
        {
            get
            {
                return lstNotification != null ? lstNotification.Where(p => !string.IsNullOrEmpty(p.Zone) && !string.IsNullOrEmpty(p.Level) && !string.IsNullOrEmpty(p.Block) && p.IsJob).ToList() : new List<NotificationDetails>();
                // return lstNotification != null ? lstNotification.Where(n => n.IsJob).ToList() : new List<NotificationDetails>();
            }
        }
        public List<NotificationDetails> MaterailNotifications
        {
            get
            {
                return lstNotification != null ? lstNotification.Where(p => !string.IsNullOrEmpty(p.Zone) && !string.IsNullOrEmpty(p.Level) && !string.IsNullOrEmpty(p.Block) && p.IsMaterial).ToList() : new List<NotificationDetails>();
                //return lstNotification != null ? lstNotification.Where(n => !n.IsJob).ToList() : new List<NotificationDetails>();
            }
        }
        public NotificationVM()
        {
        }
        public async Task<bool> UpdateSeenBy(NotificationType notificationType)
        {
            bool status = false;
            int UserID = 0;
            if (Application.Current.Properties.ContainsKey("user_id"))
                UserID = Convert.ToInt32(Application.Current.Properties["user_id"]);
            try
            {
                var result = await ApiClient.Instance.NTUpdateSeenBy(UserID, (int)notificationType);
                //   await GetNotifications();
                if (result.status == 0)
                {
                    status = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            IsLoading = false;
            return status;
        }
        public async Task<bool> GetNotifications()
        {
            IsLoading = true;
            bool status = false;
            lstNotification = new List<NotificationDetails>();
            int UserID = 0;
            if (Application.Current.Properties.ContainsKey("user_id"))
                UserID = Convert.ToInt32(Application.Current.Properties["user_id"]);
            try
            {
                var result = await ApiClient.Instance.NTGetNotfications(UserID);
                if (result.status == 0)
                {
                    lstNotification = result.data as List<NotificationDetails>;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        OnPropertyChanged("lstNotification");
                        OnPropertyChanged("JObNotifications");
                        OnPropertyChanged("MaterailNotifications");
                    });

                    status = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            IsLoading = false;
            return status;
        }
    }
}

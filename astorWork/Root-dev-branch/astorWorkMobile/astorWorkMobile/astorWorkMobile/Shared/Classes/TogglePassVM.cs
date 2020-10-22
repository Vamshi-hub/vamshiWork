using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace astorWorkMobile.Shared.Classes
{
    public class TogglePassVM: INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _status;
        public bool Status { get
            {
                return _status;
            }
            set
            {
                _status = value;
                if (_status)
                {
                    StatusIcon = "ic_check.png";
                    StatusIconColor = "Green";
                    StatusLabel = "Pass";
                }
                else
                {
                    StatusIcon = "ic_clear.png";
                    StatusIconColor = "Red";
                    StatusLabel = "Not Pass";
                }

                OnPropertyChanged("Status");
                OnPropertyChanged("StatusIcon");
                OnPropertyChanged("StatusIconColor");
                OnPropertyChanged("StatusLabel");
            }
        }
        public string StatusIcon { get; set; }
        public string StatusIconColor { get; set; }
        public string StatusLabel { get; set; }

        public TogglePassVM()
        {
            Status = false;
        }
    }
}

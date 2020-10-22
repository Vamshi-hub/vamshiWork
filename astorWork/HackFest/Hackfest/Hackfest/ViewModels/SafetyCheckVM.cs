using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hackfest.ViewModels
{
    public class SafetyCheckVM : INotifyPropertyChanged
    {
        private string _statusIcon;
        public string StatusIcon
        {
            get { return _statusIcon; }
            set
            {
                _statusIcon = value;
                RaisePropertyChanged("StatusIcon");
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                _statusMessage = value;
                RaisePropertyChanged("StatusMessage");
            }
        }

        private string _statusColor;
        public string StatusColor
        {
            get { return _statusColor; }
            set
            {
                _statusColor = value;
                RaisePropertyChanged("StatusColor");
            }
        }

        private string _personName;
        public string PersonName
        {
            get { return _personName; }
            set
            {
                _personName = value;
                RaisePropertyChanged("PersonName");
            }
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SafetyCheckVM()
        {
            _statusIcon = "BlockHelper";
            _statusMessage = "Idle";
            _statusColor = "LightGray";
            _personName = "NA";
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astorWork_Navisworks.ViewModels
{
    public class SyncResultPageVM: INotifyPropertyChanged
    {
        private string _urlVideo;
        public string UrlVideo
        {
            get { return _urlVideo; }
            set
            {
                _urlVideo = value;
                RaisePropertyChanged("UrlVideo");
            }
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SyncResultPageVM()
        {
        }
    }
}

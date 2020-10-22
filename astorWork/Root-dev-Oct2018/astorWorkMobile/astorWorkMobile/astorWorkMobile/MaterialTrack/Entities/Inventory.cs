using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace astorWorkMobile.MaterialTrack.Entities
{
    public class Inventory : INotifyPropertyChanged
    {
        public int id { get; set; }
        public string markingNo { get; set; }
        public int sn { get; set; }
        public DateTime castingDate { get; set; }
        public string trackerLabel { get; set; }

        public string ExpansionIcon
        {
            get
            {
                if (_expanded)
                    return "ic_keyboard_arrow_up.png";
                else
                    return "ic_keyboard_arrow_down.png";
            }
        }

        private bool _expanded;
        public bool Expanded
        {
            get
            {
                return _expanded;
            }
            set
            {
                _expanded = value;
                OnPropertyChanged("Expanded");
                OnPropertyChanged("ExpansionIcon");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Inventory()
        {
            Expanded = false;
        }
    }
}

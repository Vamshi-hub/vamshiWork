using MaterialTrackApp.DB;
using MaterialTrackApp.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialTrackApp.ViewModel
{
    public class FilterMRFNoVM : INotifyPropertyChanged
    {
        public ObservableCollection<string> Projects { get; set; }

        public string Project { get; set; }

        private string _markingNo;
        public string MarkingNo
        {
            get
            {
                return _markingNo;
            }
            set
            {
                _markingNo = value;

                ListMRFNo = new ObservableCollection<string>(Materials
                    .Where(mm => mm.Project == Project && mm.MarkingNo == _markingNo && !string.IsNullOrEmpty(mm.MRFNo))
                    .OrderBy(mm => mm.MRFNo)
                    .Select(mm => mm.MRFNo)
                    .Distinct());

                MRFNo = string.Empty;

                OnPropertyChanged("ListMRFNo");
                OnPropertyChanged("MRFNo");
            }
        }

        public ObservableCollection<string> ListMRFNo { get; set; }
        public string MRFNo { get; set; }

        public IEnumerable<MaterialEntity> Materials { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

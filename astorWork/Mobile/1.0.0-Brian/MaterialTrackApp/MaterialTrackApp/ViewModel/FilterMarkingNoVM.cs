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
    public class FilterMarkingNoVM : INotifyPropertyChanged
    {
        public ObservableCollection<string> Projects { get; set; }

        private string _project;
        public string Project
        {
            get
            {
                return _project;
            }
            set
            {
                _project = value;
                if (value != null)
                {
                    ComponentType = string.Empty;
                    ComponentTypes = new ObservableCollection<string>(Materials.Where(mm => mm.Project.Equals(_project) && !string.IsNullOrEmpty(mm.MaterialType))
                        .OrderBy(mm => mm.MaterialType)
                        .Select(mm => mm.MaterialType)
                        .Distinct());

                    OnPropertyChanged("Project");
                    OnPropertyChanged("ComponentType");
                    OnPropertyChanged("ComponentTypes");
                }
            }
        }

        public ObservableCollection<string> ComponentTypes { get; set; }

        private string _componentType;
        public string ComponentType
        {
            get
            {
                return _componentType;
            }
            set
            {
                _componentType = value;

                ListMarkingNo = new ObservableCollection<string>(Materials
                    .Where(mm => mm.Project == _project && mm.MaterialType == _componentType)
                    .OrderBy(mm => mm.MarkingNo)
                    .Select(mm => mm.MarkingNo)
                    .Distinct());

                MarkingNo = string.Empty;

                OnPropertyChanged("ListMarkingNo");
                OnPropertyChanged("MarkingNo");
            }
        }

        public ObservableCollection<string> ListMarkingNo { get; set; }
        public string MarkingNo { get; set; }

        public IEnumerable<MaterialEntity> Materials { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

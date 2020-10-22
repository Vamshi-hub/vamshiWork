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
    public class FilterValueVM : INotifyPropertyChanged
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
                if(value != null)
                {
                    _componentType = string.Empty;
                    ComponentTypes = new ObservableCollection<string>(Materials.Where(mm => mm.Project.Equals(_project))
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
        public string ComponentType {
            get { 
                return _componentType;
            }
            set {
                _componentType = value;
                if (string.IsNullOrEmpty(_componentType))
                    IsValueSelectionVisible = false;
                else
                {
                    IsValueSelectionVisible = true;
                    ListMarkingNo = new ObservableCollection<string>(Materials
                        .Where(mm => mm.Project.Equals(_project) && mm.MaterialType.Equals(_componentType))
                        .OrderBy(mm => mm.MarkingNo)
                        .Select(mm => mm.MarkingNo)
                        .Distinct());

                    MarkingNo = null;

                    OnPropertyChanged("ListMarkingNo");
                    OnPropertyChanged("MarkingNo");
                }
                OnPropertyChanged("IsValueSelectionVisible");
            }
        }
        
        public IEnumerable<MaterialEntity> Materials { get; set; }

        public ObservableCollection<string> ListMarkingNo { get; set; }
        public ObservableCollection<string> ListMRFNo { get; set; }

        private string _markingNo;
        public string MarkingNo {
            get
            {
                return _markingNo;
            }
            set
            {
                _markingNo = value;

                if (!string.IsNullOrEmpty(_markingNo))
                {
                    MRFNo = null;

                    ListMRFNo = new ObservableCollection<string>(Materials
                        .Where(mm => mm.Project.Equals(_project) && mm.MarkingNo.Equals(value))
                        .OrderBy(mm => mm.MRFNo)
                        .Select(mm => mm.MRFNo)
                        .Distinct());

                    OnPropertyChanged("ListMRFNo");
                    OnPropertyChanged("MRFNo");
                }
            }
        }
        public string MRFNo { get; set; }

        public void ClearSelection()
        {
            Project = null;
            ComponentType = null;
            MarkingNo = null;
            MRFNo = null;

            OnPropertyChanged("Project");
            OnPropertyChanged("ComponentType");
            OnPropertyChanged("MarkingNo");
            OnPropertyChanged("MRFNo");
        }

        public Boolean IsValueSelectionVisible { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

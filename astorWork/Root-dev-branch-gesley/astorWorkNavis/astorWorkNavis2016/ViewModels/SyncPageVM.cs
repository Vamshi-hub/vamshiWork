using astorWorkNavis2016.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkNavis2016.ViewModels
{
    public class SyncPageVM: INotifyPropertyChanged
    {
        public List<string> StageNames { get; set; }
        private ObservableCollection<MaterialEntity> _listMaterialsMatched;
        public ObservableCollection<MaterialEntity> ListMaterialsMatched
        {
            get { return _listMaterialsMatched; }
            set
            {
                _listMaterialsMatched = value;
                RaisePropertyChanged("ListMaterialsMatched");
            }
        }

        private ObservableCollection<MaterialViewEntity> _materialsMatched;
        public ObservableCollection<MaterialViewEntity> MaterialsMatched
        {
            get { return _materialsMatched; }
            set
            {
                _materialsMatched = value;
                RaisePropertyChanged("MaterialsMatched");
            }
        }

        private ObservableCollection<MaterialEntity> _listMaterialsUnmatched;
        public ObservableCollection<MaterialEntity> ListMaterialsUnmatched
        {
            get { return _listMaterialsUnmatched; }
            set
            {
                _listMaterialsUnmatched = value;
                RaisePropertyChanged("ListMaterialsUnmatched");
            }
        }

        private ObservableCollection<MaterialViewEntity> _materialsUnmatched;
        public ObservableCollection<MaterialViewEntity> MaterialsUnmatched
        {
            get { return _materialsUnmatched; }
            set
            {
                _materialsUnmatched = value;
                RaisePropertyChanged("MaterialsUnmatched");
            }
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SyncPageVM()
        {
            ListMaterialsMatched = new ObservableCollection<MaterialEntity>();
            ListMaterialsUnmatched = new ObservableCollection<MaterialEntity>();
            MaterialsMatched = new ObservableCollection<MaterialViewEntity>();
            MaterialsUnmatched = new ObservableCollection<MaterialViewEntity>();
        }
    }
}

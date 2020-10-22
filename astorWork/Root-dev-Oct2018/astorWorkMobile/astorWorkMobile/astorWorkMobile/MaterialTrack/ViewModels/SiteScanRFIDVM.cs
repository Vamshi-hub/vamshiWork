using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace astorWorkMobile.MaterialTrack.ViewModels
{
    public class SiteScanRFIDVM : MasterVM
    {
        private ObservableCollection<Tracker> _listTracker;
        public ObservableCollection<Tracker> ListTracker { get
            {
                return _listTracker;
            }
            set
            {
                _listTracker = value;
                OnPropertyChanged("ListTracker");
            }
        }

        public void RemoveTrackerByMaterial(Material material)
        {
            var tracker = _listTracker.Where(t => t.material != null && t.material.Equals(material)).FirstOrDefault();
            if (tracker != null)
            {
                _listTracker.Remove(tracker);
                OnPropertyChanged("ListTracker");
            }
        }

        public void AddTrackers(List<Tracker> trackers)
        {
            if (trackers != null)
            {
                _listTracker = new ObservableCollection<Tracker>(trackers);
                OnPropertyChanged("ListTracker");
            }
        }

        private int _countTags;
        public int CountTags
        {
            get
            {
                return _countTags;
            }
            set
            {
                _countTags = value;
                OnPropertyChanged("CountTags");
            }
        }

        private int _countUpdated;
        public int CountUpdated
        {
            get
            {
                return _countUpdated;
            }
            set
            {
                _countUpdated = value;
                OnPropertyChanged("CountUpdated");
            }
        }

        private double _scanIconRotation;
        public Double ScanIconRotation
        {
            get
            {
                return _scanIconRotation;
            }
            set
            {
                _scanIconRotation = value;
                OnPropertyChanged("ScanIconRotation");
            }
        }

        public string ToggleScanButtonLabel
        {
            get
            {
                if (_allowScan)
                    return "Scan";
                else
                    return "Stop";
            }
        }

        private bool _allowScan;
        public bool AllowScan { get
            {
                return _allowScan;
            }
            set
            {
                _allowScan = value;
                OnPropertyChanged("AllowScan");
            }
        }

        public SiteScanRFIDVM() : base()
        {
            AllowScan = true;
            CountTags = 0;
            CountUpdated = 0;
            ScanIconRotation = 0;
            ListTracker = new ObservableCollection<Tracker>();
        }

        public override void Reset()
        {
            base.Reset();
            AllowScan = true;
            CountTags = 0;
            CountUpdated = 0;
            ScanIconRotation = 0;
            ListTracker = new ObservableCollection<Tracker>();
        }
    }
}

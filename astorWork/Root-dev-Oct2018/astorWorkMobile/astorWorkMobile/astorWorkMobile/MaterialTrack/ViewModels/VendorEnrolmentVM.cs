using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace astorWorkMobile.MaterialTrack.ViewModels
{
    public class VendorEnrolmentVM : MasterVM
    {
        public ICommand SearchMarkingNoCommand { get; private set; }

        public Project Project { get; set; }

        private string _selectedMaterialType;
        public string SelectedMaterialType
        {
            get
            {
                return _selectedMaterialType;
            }
            set
            {
                _selectedMaterialType = value;
                ListMarkingNo = null;
                Task.Run(GetPreInventoryInfo);
            }
        }

        private async Task GetPreInventoryInfo()
        {
            IsLoading = true;
            int vendorId = int.Parse(App.Current.Properties["vendor_id"].ToString());
            var result = await ApiClient.Instance.MTGetPreInventoryInfo(Project.id, vendorId, _selectedMaterialType);
            if (result.status != 0)
                ErrorMessage = result.message;
            else
            {
                try
                {
                    var jsonData = result.data as JToken;
                    MaxSN = jsonData["maxSN"].ToObject<int>();
                    SN = MaxSN + 1;
                    MarkingNos = jsonData["markingNos"].ToObject<List<string>>();
                    if (MarkingNos == null)
                        ErrorMessage = "No marking no. found for this material type";
                    else
                    {
                        CastingDate = DateTime.Now.DayOfWeek.Equals(DayOfWeek.Monday) ?
                            DateTime.Today.AddDays(-2) : DateTime.Today.AddDays(-1);

                        OnPropertyChanged("SN");
                        OnPropertyChanged("CastingDate");
                    }
                }
                catch (Exception exc)
                {
                    ErrorMessage = exc.Message;
                }
            }

            IsLoading = false;
        }

        public int MaxSN { get; set; }

        private List<string> _markingNos;
        public List<string> MarkingNos
        {
            get
            {
                return _markingNos;
            }
            set
            {
                _markingNos = value;
                
                OnPropertyChanged("MarkingNos");
            }
        }

        private ObservableCollection<string> _listMarkingNo;
        public ObservableCollection<string> ListMarkingNo
        {
            get
            {
                return _listMarkingNo;
            }
            set
            {
                _listMarkingNo = value;

                OnPropertyChanged("ListMarkingNo");
                OnPropertyChanged("ListMarkingNoHeight");
            }
        }

        public int ListMarkingNoHeight
        {
            get
            {
                if (_listMarkingNo == null)
                    return 0;
                else
                    return 40 * _listMarkingNo.Count;
            }
        }

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
                if (!string.IsNullOrEmpty(_markingNo) && _markingNo != _selectedMarkingNo)
                {
                    SearchMarkingNo(_markingNo);
                }

                OnPropertyChanged("MarkingNo");
                OnPropertyChanged("EnrolButtonEnabled");
            }
        }

        private string _selectedMarkingNo;
        /*
        public string SelectedMarkingNo
        {
            get { return null; }
            set
            {
                _markingNo = value;
                _selectedMarkingNo = value;
                OnPropertyChanged("MarkingNo");
            }
        }
        */
        public void SetMarkingNo(string newMarkingNo)
        {
            _selectedMarkingNo = newMarkingNo;
            MarkingNo = newMarkingNo;
            ListMarkingNo = null;
        }

        private DateTime _castingDate;
        public DateTime CastingDate
        {
            get
            {
                return _castingDate;
            }
            set
            {
                _castingDate = value;
                OnPropertyChanged("EnrolButtonEnabled");
            }
        }

        private int _sn;
        public int SN
        {
            get
            {
                return _sn;
            }
            set
            {
                _sn = value;
                OnPropertyChanged("EnrolButtonEnabled");
            }
        }

        public bool EnrolButtonEnabled
        {
            get
            {
                return
                    !string.IsNullOrEmpty(SelectedMaterialType) &&
                    !string.IsNullOrEmpty(MarkingNo) &&
                    (SN > 0);
            }
        }

        public VendorEnrolmentVM() : base()
        {
            SearchMarkingNoCommand = new Command<string>(SearchMarkingNo);
        }

        void SearchMarkingNo(string searchText)
        {
            if (string.IsNullOrEmpty(searchText) || MarkingNos == null)
            {
                ListMarkingNo = null;
            }
            else
            {
                var filteredList = MarkingNos.Where(mn => mn.ToLower().Contains(searchText.ToLower()));
                ListMarkingNo = new ObservableCollection<string>(filteredList);
            }
        }



        public override void Reset()
        {
            _selectedMaterialType = null;
            MarkingNo = null;
            base.Reset();
        }
    }
}

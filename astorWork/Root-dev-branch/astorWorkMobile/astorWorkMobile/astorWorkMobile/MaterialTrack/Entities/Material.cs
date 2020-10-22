using astorWorkMobile.Shared.Utilities;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace astorWorkMobile.MaterialTrack.Entities
{
    public class Material : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public int id { get; set; }
        public int projectID { get; set; }
        public string markingNo { get; set; }
        private bool _visibleLength { get; set; }
        public bool VisibleLength
        {
            get
            {
                if (Length == null || Length == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        private bool _visibleArea { get; set; }
        public bool VisibleArea
        {
            get
            {
                if (Area == null || Area == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        private bool _visibleDrawingNo { get; set; }
        public bool VisibleDrawingNo
        {
            get
            {
                if (string.IsNullOrEmpty(DrawingNo))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public float? Area { get; set; }
        public float? Length { get; set; }
        public string DrawingNo { get; set; }
        public bool IsChecklist { get; set; }
        public string block { get; set; }
        public string level { get; set; }
        public string zone { get; set; }
        public string materialType { get; set; }
        public int stageId { get; set; }
        public Stage currentStage { get; set; }
        public int countQCCase { get; set; }
        public string mrfNo { get; set; }
        public int sn { get; set; }
        public int organisationID { get; set; }
        public DateTime castingDate { get; set; }
        public DateTimeOffset _expectedDeliveryDate { get; set; }
        public DateTimeOffset ExpectedDeliveryDate {
            get
            {
                return _expectedDeliveryDate;
            }
            set
            {
                _expectedDeliveryDate = value.LocalDateTime;
            }
        }
        public bool IsExpectedDeliveryDate { get
            {
                var mobile_entry = Convert.ToInt32(Application.Current.Properties["entry_point"]);
                return mobile_entry == 0 ? true : false;
            }
        }
        public DateTime orderDate { get; set; }
        public string QCStatus { get; set; }
        public int _QCStatusCode { get; set; } = -1;
        public int QCStatusCode
        {
            get => _QCStatusCode;
            set
            {
                _QCStatusCode = value;
                //if (_QCStatusCode == -1)
                //{
                //    QCStatus = "No QC";
                //}
                 if (_QCStatusCode == 0)
                {
                    QCStatus = "QC Pending";
                }
                if (QCStatus.Contains('_'))
                {
                    QCStatus = QCStatus.Replace('_', ' ');
                    
                }
                if (QCStatus.Contains("by Maincon"))
                {
                        QCStatus = QCStatus.Replace("by Maincon", "");
                }
            }
        }
        public string routeTo { get; set; }
        public string destination => string.Format("{0}-L{1}-Z{2}", block, level, zone);
        public Location CurrentLocation { get; set; }
        public int? ForgeElementId { get; set; }
        public string ForgeModelURN { get; set; }
        public bool CanIgnoreQC { get; set; }
        private bool _isCheckBoxVisible { get; set; } = false;
        public bool IsCheckBoxVisible
        {
            get => _isCheckBoxVisible;
            set
            {
                _isCheckBoxVisible = value;
                if (_isCheckBoxVisible)
                {
                    IsChecked = false;
                }
                OnPropertyChanged("IsCheckBoxVisible");
            }
        }
        private bool _isChecked { get; set; }
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                if (_isChecked)
                    CheckBoxIcon = "ic_checkbox_checked.png";
                else
                    CheckBoxIcon = "ic_checkbox_empty.png";
                OnPropertyChanged("IsChecked");
                if (ViewModelLocator.vendorHomeVM.OrderedMaterialItems != null || ViewModelLocator.vendorHomeVM.OrderedMaterialItems.Count() > 0)
                {
                    if (ViewModelLocator.vendorHomeVM.OrderedMaterialItems.Where(om => om.Material != null && om.Material.IsChecked).Count() > 0)
                        ViewModelLocator.vendorHomeVM.IsItemChecked = true;
                    else
                        ViewModelLocator.vendorHomeVM.IsItemChecked = false;
                    if (ViewModelLocator.vendorHomeVM.OrderedMaterialItems.Count() == ViewModelLocator.vendorHomeVM.OrderedMaterialItems.Where(om => om.Material != null && om.Material.IsChecked).Count())
                        ViewModelLocator.vendorHomeVM.IsAll = true;
                    else
                        ViewModelLocator.vendorHomeVM.IsAll = false;
                    //else if (!ViewModelLocator.vendorHomeVM.IsAll && ViewModelLocator.vendorHomeVM.OrderedMaterialItems.Count() != ViewModelLocator.vendorHomeVM.OrderedMaterialItems.Where(om => om.Material != null && om.Material.IsChecked).Count())
                    //{
                    //    ViewModelLocator.vendorHomeVM.IsAll = false;
                    //}
                }
                OnPropertyChanged("IsChecked");
            }
        }
        public string _checkBoxIcon { get; set; }
        public string CheckBoxIcon
        {
            get => _checkBoxIcon;
            set
            {
                _checkBoxIcon = value;
                OnPropertyChanged("CheckBoxIcon");
            }
        }
        private bool _isToggled { get; set; }
        public bool IsToggled
        {
            get => _isToggled;
            set
            {
                _isToggled = value;
                OnPropertyChanged("IsToggled");
                if (_isToggled != value)
                {
                    _isToggled = value;
                    OnPropertyChanged("IsToggled");
                    if (ViewModelLocator.vendorHomeVM.OrderedMaterialItems != null || ViewModelLocator.vendorHomeVM.OrderedMaterialItems.Count() > 0)
                    {
                        if (ViewModelLocator.vendorHomeVM.OrderedMaterialItems.Where(om => om.Material != null && om.Material.IsToggled).Count() > 0)
                            ViewModelLocator.vendorHomeVM.IsItemChecked = true;
                        else
                            ViewModelLocator.vendorHomeVM.IsItemChecked = false;
                        if (ViewModelLocator.vendorHomeVM.OrderedMaterialItems.Count() == ViewModelLocator.vendorHomeVM.OrderedMaterialItems.Where(om => om.Material != null && om.Material.IsToggled).Count())
                            ViewModelLocator.vendorHomeVM.IsAll = true;
                    }
                }
            }
        }
        private bool _isVisible { get; set; }
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged("IsVisible");
                    OnPropertyChanged("ExpansionIcon");
                }
            }
        }
        public string ExpansionIcon
        {
            get
            {
                if (_isVisible)
                    return "ic_keyboard_arrow_up.png";
                else
                    return "ic_keyboard_arrow_down.png";
            }
        }
        public bool IsQCOpen { get; set; }
        //public string Module { get; set; }
            //get
            //{
            //    if (zone != "0")
            //    {
            //        if (markingNo == materialType)
            //            return $"{block}_L{level}_Z{zone}_{markingNo}";
            //        else
            //            return $"{block}_L{level}_Z{zone}_{markingNo} ({materialType})";
            //    }
            //    else
            //    {
            //        if (markingNo == materialType)
            //            return $"{block}_L{level}_{markingNo}";
            //        else
            //            return $"{block}_L{level}_{markingNo} ({materialType})";
            //    }
            //}
            //    }
    }
}

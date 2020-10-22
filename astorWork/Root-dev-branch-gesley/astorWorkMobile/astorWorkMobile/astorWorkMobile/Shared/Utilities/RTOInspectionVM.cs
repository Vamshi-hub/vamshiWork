using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using static astorWorkMobile.Shared.Classes.Enums;

namespace astorWorkMobile.Shared.Utilities
{
    public class RTOInspectionVM : MasterVM
    {
        public int Role { get; set; }
        public int JobScheduleID { get; set; }
        public string TradeOrActivity { get; set; }
        
        

        public RTOInspection RTOInspectionForm = new RTOInspection();
        public List<RTOInspection> _listInspectionForm = new List<RTOInspection>();
        public List<RTOInspection> ListInspectionForm
        {
            get { return _listInspectionForm; }
            set
            {
                _listInspectionForm = value;
                OnPropertyChanged("ListInspectionForm");
            }
        }

        private User _selectedRTO { get; set; }
        public User SelectedRTO
        {
            get { return _selectedRTO; }
            set
            {
                _selectedRTO = value;
                //if (value != null)
                //{
                //    DisplayRTOSignPad = true;
                //    OnPropertyChanged("DisplayRTOSignPad");
                //}
                OnPropertyChanged("SelectedRTO");
            }
        }
        private List<User> _listRTO { get; set; }
        public List<User> ListRTO
        {
            get { return _listRTO; }
            set
            {
                _listRTO = value;
                if (_listRTO.Count == 1)
                {
                    SelectedRTO = _listRTO[0];
                    Device.BeginInvokeOnMainThread(() => { OnPropertyChanged("SelectedRTO"); });
                }
                //else if (_selectedRTO != null)
                //{
                //    SelectedRTO = _selectedRTO;
                //}
                Device.BeginInvokeOnMainThread(() => { OnPropertyChanged("ListRTO"); });
            }
        }
        public async void GetRTOs()
        {
            try
            {
                var result = await ApiClient.Instance.JTGetRTOList();
                if (result.status == 0)
                {
                    ListRTO = result.data as List<User>;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                throw;
            }
            Device.BeginInvokeOnMainThread(() => { OnPropertyChanged("CheckLists"); OnPropertyChanged("ListInspectionForm"); });
        }
        public RTOInspectionVM()
        {
            
        }
    }
    public class RTOCheckList
    {
        public string Name { get; set; }
    }
    public class RTOInspection : MasterVM
    {
        public bool IsExpanded { get; set; }
        public string ExpansionIcon
        {
            get
            {
                if (IsExpanded)
                {
                    return "ic_keyboard_arrow_up.png";
                }
                else
                {
                    return "ic_keyboard_arrow_down.png";
                }
            }
        }
        public int JobScheduleID { get; set; }
        public string TradeOrActivity { get; set; }
        public string ReferenceNo { get; set; }
        public string Discipline { get; set; }
        public string ModuleName { get; set; }
        public string DrawingNo { get; set; }
        public TimeSpan? Time { get; set; }
        public DateTime? Date { get; set; }

        public List<Signatures> Signatures = new List<Signatures>();
        public List<RoleType> SignatureRoles { get; set; }
        public ICommand ExpandCommand { get; set; }

        public List<string> _checklistItems { get; set; } = new List<string>();
        public List<string> ChecklistItems
        {
            get { return _checklistItems; }
            set
            {
                _checklistItems = value;
                OnPropertyChanged("ChecklistItems");
            }
        }

        private void ExpandClicked()
        {
            IsExpanded = !IsExpanded;
            OnPropertyChanged("IsExpanded");
            OnPropertyChanged("ExpansionIcon");
            if (ViewModelLocator.jobScanVM.ScannedItemsGrid != null)
            {
                ViewModelLocator.jobScanVM.ScannedItemsGrid.LayoutTo(new Rectangle(0, 0, ViewModelLocator.jobScanVM.Width, ViewModelLocator.jobScanVM.Height), 300, Easing.Linear);
            }
        }

        public ICommand SignatureCommand { get; set; }
        async void InvokeSignaturePad(string commandType)
        {
            try
            {
                if (Convert.ToInt32(commandType) == (int)RoleType.SiteOfficerOrMainCon)
                {
                    ViewModelLocator.rtoInspectionVM.Role = (int)RoleType.SiteOfficerOrMainCon;
                    var signaturePad = new SignatureView(this.GetType());
                    signaturePad.BindingContext = this;
                    await Navigation.PushModalAsync(signaturePad);
                }
                else if (Convert.ToInt32(commandType) == (int)RoleType.RTO)
                {
                    ViewModelLocator.rtoInspectionVM.Role = (int)RoleType.RTO;
                    var signaturePad = new SignatureView(this.GetType());
                    signaturePad.BindingContext = this;
                    await Navigation.PushModalAsync(signaturePad);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private User _selectedRTO { get; set; }
        public User SelectedRTO
        {
            get 
            {
                return _selectedRTO;
                //
            }
            set
            {
                _selectedRTO = value;
                if (value != null)
                {
                    DisplayRTOSignPad = true;
                    OnPropertyChanged("DisplayRTOSignPad");
                }
                OnPropertyChanged("SelectedRTO");
            }
        }
        //private List<User> _listRTO { get; set; }
        public List<User> ListRTO
        {
            get { return ViewModelLocator.rtoInspectionVM.ListRTO; }
        }

        private ImageSource _rtoSignature { get; set; }
        public ImageSource RTOSignature
        {
            get { return _rtoSignature; }
            set
            {
                _rtoSignature = value;
                OnPropertyChanged("RTOSignature");
            }
        }
        private string RTOSignatureBase64 { get; set; }
        public bool DisplayRTOSignPad { get; set; } = false;
        public bool ShowSignatureButton { get; set; } = true;

        private string _imageBase64 { get; set; }
        public string ImageBase64
        {
            get { return _imageBase64; }
            set
            {
                try
                {
                    _imageBase64 = value;
                    var imageSource = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(_imageBase64)));
                    if (ViewModelLocator.rtoInspectionVM.Role == (int)RoleType.SiteOfficerOrMainCon)
                    {
                        MainConSignature = imageSource;
                        MainConSignatureBase64 = _imageBase64;
                        if (Signatures != null && Signatures.Where(p => p.UserID == Convert.ToInt32(Application.Current.Properties["user_id"])).Count() == 1)
                        {
                            Signatures.RemoveAt(Signatures.IndexOf(Signatures.Where(p => p.UserID == Convert.ToInt32(Application.Current.Properties["user_id"])).FirstOrDefault()));
                            Signatures.Add(new Signatures() { ImageBase64 = _imageBase64, UserID = Convert.ToInt32(Application.Current.Properties["user_id"]) });
                        }
                        else
                        {
                            Signatures.Add(new Signatures() { ImageBase64 = _imageBase64, UserID = Convert.ToInt32(Application.Current.Properties["user_id"]) });
                        }
                    }
                    else if (ViewModelLocator.rtoInspectionVM.Role == (int)RoleType.RTO)
                    {
                        RTOSignature = imageSource;
                        RTOSignatureBase64 = _imageBase64;
                        if (Signatures != null && Signatures.Where(p => p.UserID == SelectedRTO.UserID).Count() == 1)
                        {
                            Signatures.RemoveAt(Signatures.IndexOf(Signatures.Where(p => p.UserID == SelectedRTO.UserID).FirstOrDefault()));
                            Signatures.Add(new Signatures() { ImageBase64 = _imageBase64, UserID = SelectedRTO.UserID });
                        }
                        else
                        {
                            Signatures.Add(new Signatures() { ImageBase64 = _imageBase64, UserID = SelectedRTO.UserID });
                        }
                    }
                }
                catch (Exception ex)
                {

                    ErrorMessage = ex.Message;
                }
            }
        }
        private ImageSource _mainConSignature { get; set; }
        public ImageSource MainConSignature
        {
            get { return _mainConSignature; }
            set
            {
                _mainConSignature = value;
                OnPropertyChanged("MainConSignature");
            }
        }
        private string MainConSignatureBase64 { get; set; }
        public RTOInspection()
        {
            ExpandCommand = new Command(ExpandClicked);
            SignatureCommand = new Command<string>(InvokeSignaturePad);
        }


    }
    public class Signatures
    {
        public string URL { get; set; }
        public string ImageBase64 { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
    }
}

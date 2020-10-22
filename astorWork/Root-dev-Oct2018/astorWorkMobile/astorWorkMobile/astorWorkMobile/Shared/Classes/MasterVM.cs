using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace astorWorkMobile.Shared.Classes
{
    public class MasterVM : INotifyPropertyChanged
    {
        private bool _loading;
        public bool IsLoading
        {
            get
            {
                return _loading;
            }
            set
            {
                _loading = value;

                if (_loading)
                {
                    _neverLoadBefore = false;
                    ErrorMessage = string.Empty;
                }

                OnPropertyChanged("IsLoading");
                OnPropertyChanged("ShowLoadingView");
                OnPropertyChanged("ShowContent");
            }
        }
        public bool ShowLoadingView
        {
            get
            {
                return _loading || !string.IsNullOrEmpty(_errorMessage);
            }
        }

        public bool ShowContent
        {
            get
            {
                return !_neverLoadBefore && !ShowLoadingView;
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
                if (!string.IsNullOrEmpty(_errorMessage))
                {
                    IsLoading = false;
                    Device.BeginInvokeOnMainThread((Action)(() =>
                    {
                        App.Current.MainPage.DisplayAlert("Error", _errorMessage, "OK");
                        ErrorMessage = string.Empty;
                    }));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool _neverLoadBefore = true;
        public virtual void Reset()
        {
            IsLoading = false;
            ErrorMessage = string.Empty;
            _neverLoadBefore = true;
        }

        public MasterVM()
        {
            IsLoading = false;
            ErrorMessage = string.Empty;
        }
    }
}

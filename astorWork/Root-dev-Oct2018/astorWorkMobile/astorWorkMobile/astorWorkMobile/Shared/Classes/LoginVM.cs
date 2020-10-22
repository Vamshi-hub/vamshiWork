using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace astorWorkMobile.Shared.Classes
{
    public class LoginVM : MasterVM
    {
        private string _tenantName;
        public string TenantName
        {
            get
            {
                return _tenantName;
            }
            set
            {
                _tenantName = value;
                OnPropertyChanged("TenantName");
                OnPropertyChanged("LoginButtonEnabled");
            }
        }

        private string _userName;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                OnPropertyChanged("UserName");
                OnPropertyChanged("LoginButtonEnabled");
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
                OnPropertyChanged("LoginButtonEnabled");
            }
        }

        public bool LoginButtonEnabled
        {
            get
            {
                return
                    !string.IsNullOrEmpty(_tenantName) &&
                    !string.IsNullOrEmpty(_userName) &&
                    !string.IsNullOrEmpty(_password);
            }
        }

        public override void Reset()
        {
            TenantName = string.Empty;
            UserName = string.Empty;
            Password = string.Empty;
        }
    }
}

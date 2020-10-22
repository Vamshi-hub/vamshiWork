using astorWorkMobile.Shared.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace astorWorkMobile
{

    public class MaincontentPageMenuItem : MasterVM
    {
       
        public int Id { get; set; }
        public string _title { get; set; }
        public string Title { get; set; }
       

        public Type TargetType { get; set; }
        public string WelcomeMessage
        {
            get
            {
                var userName = Application.Current.Properties["user_name"] as string;

                return $"Hi, {userName}";
            }
        }
    }
}
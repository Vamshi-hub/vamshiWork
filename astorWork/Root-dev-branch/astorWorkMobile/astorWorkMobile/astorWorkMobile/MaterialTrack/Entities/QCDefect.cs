using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using static astorWorkMobile.Shared.Utilities.ApiClient;

namespace astorWorkMobile.MaterialTrack.Entities
{
    public class QCDefect : MasterVM
    {
        public int ID { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
        public bool _isOpen { get; set; }
        public Color DefectStatusColor { get; set; }
        public string DefectQCStatusText { get; set; }
        public bool IsOpen { get; set; }

        public bool IsRectified { get; set; }
        public bool IsClosed { get; set; }
        public bool IsDummyDefects { get; set; }
        /*
         * 0 - Open
         * 1 - Rectified
         * 2 - Closed
         */
        private int _status;
        public int StatusCode
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                OnPropertyChanged("StatusText");
                OnPropertyChanged("StatusColor");
            }
        }
        public string StatusText
        {
            get
            {
                var text = string.Empty;
                switch (StatusCode)
                {
                    case 1:
                        text = "Open";
                        break;
                    case 2:
                        text = "Rectified";
                        break;
                    case 3:
                        text = "Closed";
                        break;
                }
                return text;
            }
        }
        public Color StatusColor
        {
            get
            {
                var color = Color.Black;
                switch (StatusCode)
                {
                    case 1:
                        color = Color.Red;
                        break;
                    case 2:
                        color = Color.Green;
                        break;
                    case 3:
                        color = Color.Blue;
                        break;
                }
                return color;
            }
        }
        public int CountPhotos { get; set; }
        public string SubconName { get; set; }
        public int SelectedSubconID { get; set; }
    }
}

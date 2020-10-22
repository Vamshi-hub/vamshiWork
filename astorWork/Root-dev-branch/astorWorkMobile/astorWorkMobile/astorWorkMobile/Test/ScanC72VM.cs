using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace astorWorkMobile.Test
{
    public class ScanC72VM : INotifyPropertyChanged
    {
        public string Status { get; set; }
        public IEnumerable<string> Tags { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public ScanC72VM()
        {
            Tags = new List<string>();
            Status = "Unkown";
        }
    }
}

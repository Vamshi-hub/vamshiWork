using astorWorkMobile.MaterialTrack.Pages;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.JobTrack.Pages.Cells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IncomingViewCell : ViewCell
    {
       
        Random rnd = new Random();
        public IncomingViewCell()
        {
            InitializeComponent();
            //Color color = userColors[UserName.Text];
            //UserName.TextColor = color;
        }
    }
}
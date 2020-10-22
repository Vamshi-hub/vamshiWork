using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using astorWorkMobile.iOS.Renderers;
using astorWorkMobile.Shared.Views;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ListView), typeof(CustomListViewRenderer))]
namespace astorWorkMobile.iOS.Renderers
{
    class CustomListViewRenderer : ListViewRenderer
    {
        public CustomListViewRenderer()
        {
            MaterialTrack.ViewModels.MaterialFrameVM.ViewCellSizeChangedEvent += UpdateTableView;
        }

        private void UpdateTableView()
        {
            var tv = Control as UITableView;
            if (tv == null) return;
            tv.BeginUpdates();
            tv.EndUpdates();
        }
    }
}
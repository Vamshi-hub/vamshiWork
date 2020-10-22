using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.MaterialTrack.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ViewQCPhoto : CarouselPage
    {
		public ViewQCPhoto ()
		{
			InitializeComponent ();
        }

        private void OnRemoveClicked(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if(btn != null && btn.BindingContext != null)
            {
                var qcPhoto = btn.BindingContext as QCPhoto;
                ViewModelLocator.siteUpdateStageVM.QCPhotos.Remove(qcPhoto);
                ViewModelLocator.siteUpdateStageVM.CountQCPhotos--;
            }
        }
    }
}
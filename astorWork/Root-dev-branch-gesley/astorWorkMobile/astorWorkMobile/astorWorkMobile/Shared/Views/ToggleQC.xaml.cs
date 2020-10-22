using astorWorkMobile.MaterialTrack.ViewModels;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.Shared.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ToggleQC : ContentView
    {
        public static readonly BindableProperty CheckedProperty = BindableProperty.Create(
                                                         propertyName: "Checked",
                                                         returnType: typeof(bool),
                                                         declaringType: typeof(ToggleQC),
                                                         defaultValue: false,
                                                         defaultBindingMode: BindingMode.TwoWay,
                                                         propertyChanged: checkedPropertyChanged);

        private static void checkedPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            
            var control = (ToggleQC)bindable;
            if ((bool)newValue)
            {
                control.lblToggleChecked.Text = "Pass";
            }
            else
            {
                control.lblToggleChecked.Text = "Not Pass";
            }
            
        }

        void OnCheckBoxTapped(Object sender, EventArgs e)
        {            
            var ctx = btnToggleChecked.BindingContext;
            if (ctx != null && ctx.GetType() == typeof(TogglePassVM))
            {
                ((TogglePassVM)ctx).Status = !((TogglePassVM)ctx).Status;
            }            
        }

        public ToggleQC ()
		{
			InitializeComponent ();
		}
	}
}
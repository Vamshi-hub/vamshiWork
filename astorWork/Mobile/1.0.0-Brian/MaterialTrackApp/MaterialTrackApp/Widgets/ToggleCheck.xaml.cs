using FormsPlugin.Iconize;
using MaterialTrackApp.DB;
using MaterialTrackApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTrackApp.Widgets
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ToggleCheck : ContentView
    {
        public static readonly BindableProperty CheckedProperty = BindableProperty.Create(
                                                         propertyName: "Checked",
                                                         returnType: typeof(bool),
                                                         declaringType: typeof(ToggleCheck),
                                                         defaultValue: false,
                                                         defaultBindingMode: BindingMode.TwoWay,
                                                         propertyChanged: checkedPropertyChanged);

        private static void checkedPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (ToggleCheck)bindable;
            if ((bool)newValue)
            {
                control.btnToggleChecked.Icon = "md-check-box";
                control.btnToggleChecked.IconColor = Color.Green;
            }
            else
            {
                control.btnToggleChecked.Icon = "md-check-box-outline-blank";
                control.btnToggleChecked.IconColor = Color.Gray;
            }
        }


        public ToggleCheck()
        {
            InitializeComponent();
        }

        private void btnToggleChecked_Clicked(object sender, EventArgs e)
        {
            IconImage btn = sender as IconImage;
            if (btn != null)
            {
                var beacon = btn.BindingContext as BeaconEntity;
                if (beacon != null)
                    beacon.IsChecked = !beacon.IsChecked;

                var installTask = btn.BindingContext as InstallTaskEntity;
                if (installTask != null)
                    installTask.Selected = !installTask.Selected;
            }

        }
    }
}
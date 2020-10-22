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
    public partial class TogglePassed : ContentView
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
            var control = (TogglePassed)bindable;
            if ((bool)newValue)
            {
                control.btnToggleChecked.Icon = "md-check-box";
                control.btnToggleChecked.IconColor = Color.Green;
            }
            else
            {
                control.btnToggleChecked.Icon = "md-indeterminate-check-box";
                control.btnToggleChecked.IconColor = Color.Red;
            }
        }


        public TogglePassed()
        {
            InitializeComponent();
        }

        private void btnToggleChecked_Clicked(object sender, EventArgs e)
        {
            IconImage btn = sender as IconImage;
            if (btn != null)
            {
                var enrollTask = btn.BindingContext as EnrollTaskEntity;
                if (enrollTask != null)
                    enrollTask.PassQC = !enrollTask.PassQC;

                var installTask = btn.BindingContext as InstallTaskEntity;
                if (installTask != null)
                    installTask.PassQC = !installTask.PassQC;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.Test
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ScanC72 : ContentView
	{
        private ScanC72VM vm = new ScanC72VM();

        public ScanC72 ()
		{
			InitializeComponent ();
            BindingContext = vm;
        }
	}
}
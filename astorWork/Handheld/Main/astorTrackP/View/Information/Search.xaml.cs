using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Plugin.Toasts;
using System.Linq;

namespace astorCable
{
	public partial class Search : ContentPage
	{
        SearchViewModel _viewModel;
		public Search()
		{
			InitializeComponent ();
			_viewModel = new SearchViewModel(this.Navigation);
			BindingContext = _viewModel;

			NavigationPage.SetTitleIcon (this, "cablecutting.png");
		}

        void ScanRFID()
        {
            lock (_viewModel)
            {
                if (_viewModel.isLoading) return;
                _viewModel.isLoading = true;
            }

            if (_viewModel.isScanning) App.rfidReader.StopTagRead();
            _viewModel.isScanning = !_viewModel.isScanning;
            if (_viewModel.isScanning) this.ScanTag();
            _viewModel.isLoading = false;
        }

        void ScanTag()
        {
            string strTagID;
            while (true)
            {
                if (_viewModel.isScanning)
                {
                    strTagID = App.rfidReader.ReadSingleTagAsync().Result;

                    if (string.IsNullOrEmpty(strTagID))
                        continue;

                    if (strTagID.Length > 9)
                        strTagID = strTagID.Substring(strTagID.Length - 9, 9);

                    _viewModel.SearchDocument = strTagID;

                    _viewModel.DoSearchCommand();

                    _viewModel.isScanning = !_viewModel.isScanning;
                    _viewModel.isLoading = false;
                    App.rfidReader.StopTagRead();
                }
                else
                {
                    if (_viewModel.isScanning)
                        App.rfidReader.StopTagRead();
                    break;
                }
            }
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<App>(this, "OnKeyDown");

            base.OnDisappearing();

        }

        protected override void OnAppearing()
        {
            MessagingCenter.Subscribe<App>(this, "OnKeyDown", (s) =>
            {
                ScanRFID();
            });

            if (App.rfidReader != null)
            {
                if (!App.rfidReader.ReaderInitialized())
                    App.rfidReader.InitializeReaderAsync().Wait();
            }

            _viewModel.isLoading = true;
            base.OnAppearing();
            _viewModel.isScanning = false;
            _viewModel.isLoading = false;
        }

    }
}


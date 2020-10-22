using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Plugin.Toasts;
using System.Linq;


namespace astorCable
{
	public partial class InformationOption : ContentPage
	{


        InformationOptionViewModel _viewModel;
		public InformationOption()
		{
			InitializeComponent ();
			NavigationPage.SetTitleIcon (this, "receive.png");

			_viewModel = new InformationOptionViewModel(this.Navigation);
			BindingContext = _viewModel;

            ObservableCollection<DocumentType> docList = new ObservableCollection<DocumentType>();
            docList.Add(new DocumentType { Name = "Cable Tags" });
            docList.Add(new DocumentType { Name = "Junction Box" });
            uxDocumentType.ItemsSource = docList;

        }

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

//			App.DocumentTypeDB.SaveItem(new DocumentType{ Name = "Purchase Order" });
//			App.DocumentTypeDB.SaveItem(new DocumentType{ Name = "Return Material Order" });
//			App.DocumentTypeDB.SaveItem(new DocumentType{ Name = "Transfer Order" });
//			App.DocumentTypeDB.SaveItem(new DocumentType{ Name = "Adhoc" });
//
//			uxDocumentType.ItemsSource = App.DocumentDB.GetItems();

		}
	}
}


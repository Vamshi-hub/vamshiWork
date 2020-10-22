﻿using FormsPlugin.Iconize;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MaterialTrackApp.MainPageMaster;

namespace MaterialTrackApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        private Dictionary<int, IconNavigationPage> _listDetailPages;
        public MainPage()
        {
            InitializeComponent();
            _listDetailPages = new Dictionary<int, IconNavigationPage>();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
            
            // Set default detail page
            var defaultPage = new IconNavigationPage(new HomePage());
            _listDetailPages.Add(0, defaultPage);
            Detail = defaultPage;
            
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MainPageMenuItem;
            if (item == null)
                return;

            if (item.Id == 5)
            {
                Application.Current.Properties.Clear();
                await Application.Current.SavePropertiesAsync();

                Application.Current.MainPage = App.indexPage;                
            }
            else
            {
                try
                {
                    await Detail.Navigation.PopToRootAsync();

                    IconNavigationPage newPage = null;
                    if (_listDetailPages.ContainsKey(item.Id))
                        newPage = _listDetailPages[item.Id];

                    if (newPage == null)
                    {
                        var page = (Page)Activator.CreateInstance(item.TargetType);
                        page.Title = item.Title;
                        newPage = new IconNavigationPage(page);
                        _listDetailPages.Add(item.Id, newPage);
                    }

                    Detail = newPage;
                    IsPresented = false;
                    MasterPage.ListView.SelectedItem = null;
                }
                catch(Exception exc)
                {
                    Debug.WriteLine("Navigate detail page failed");
                    Debug.WriteLine(exc.Message);
                }
            }
        }

        public void NavigateDetail(int itemIndex)
        {
            var menuItems = ((MainPageMasterViewModel)MasterPage.BindingContext).MenuItems;
            if (0 <= itemIndex && itemIndex < menuItems.Count)
                MasterPage.ListView.SelectedItem = menuItems[itemIndex];
        }
    }
}
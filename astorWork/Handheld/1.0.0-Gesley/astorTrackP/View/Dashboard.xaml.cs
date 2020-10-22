using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorTrackP
{
    public partial class Dashboard : ContentPage
    {
        //DashboardViewModel _vm;
        public Dashboard()
        {
            InitializeComponent();
            //NavigationPage.SetHasNavigationBar(this, false);

            //_vm = new DashboardViewModel(this.Navigation);
            //BindingContext = _vm;
            NavigationPage.SetTitleIcon(this, "dashboard.png");
            DisplayChart();
        }

        private void DisplayChart()
        {
            var data = App.MaterialMasterDb.GetMaterialMasterDashboard().FirstOrDefault();
            uxLayout.Children.Add(new Grid
            {
                Children =
                    {
                        new Grid
                        {
                            BackgroundColor  = Color.White,
                        },
                        new StackLayout
                        {
                            Spacing = 0,
                            Children =
                            {
                                new Label
                                {
                                    XAlign = TextAlignment.Center,
                                    VerticalOptions = LayoutOptions.Center,
                                    HorizontalOptions = LayoutOptions.Center,
                                    Text = "Overall Project Progress",
                                    TextColor = Color.Black,
                                    FontSize = 18,
                                },
                                new Grid
                                {
                                    Children =
                                    {
                                        new CrossPieChartView
                                        {
                                            Progress = Convert.ToSingle(data.Progress),
                                            ProgressColor = Color.Red,
                                            ProgressBackgroundColor = Color.FromHex("#EEEEEEEE"),
                                            StrokeThickness = Device.OnPlatform(10, 20, 80),
                                            Radius = Device.OnPlatform(100, 180, 160),
                                            BackgroundColor = Color.White
                                        },
                                        new Label
                                        {
                                            Text =  data.Progress.ToString(),
                                            Font = Font.BoldSystemFontOfSize(NamedSize.Large),
                                            FontSize = 60,
                                            VerticalOptions = LayoutOptions.Center,
                                            HorizontalOptions = LayoutOptions.Center,
                                            TextColor = Color.Black
                                        }
                                    }
                                },
                                new Label
                                {
                                    XAlign = TextAlignment.Center,
                                    VerticalOptions = LayoutOptions.Center,
                                    HorizontalOptions = LayoutOptions.Center,
                                    Text = "Daily Progress",
                                    TextColor = Color.Black,
                                    FontSize = 18,
                                },
                                new StackLayout
                                {
                                    Orientation = StackOrientation.Horizontal,
                                    HorizontalOptions = LayoutOptions.FillAndExpand,
                                    VerticalOptions = LayoutOptions.FillAndExpand,
                                    HeightRequest = 150,
                                    Spacing = 0,
                                    Children =
                                    {
                                       

                                        //Produced
                                        new StackLayout
                                        {
                                            Orientation = StackOrientation.Vertical,
                                            HorizontalOptions = LayoutOptions.FillAndExpand,
                                            VerticalOptions = LayoutOptions.FillAndExpand,
                                            Spacing = 0,
                                            Children = {
                                                            new Grid
                                                            {
                                                                BackgroundColor= Color.Purple,
                                                                ColumnSpacing = 0,
                                                                RowSpacing = 0,
                                                                Children =
                                                                {
                                                                    new CrossPieChartView
                                                                    {
                                                                        Progress = data.Produced,
                                                                        ProgressColor = Color.Yellow,
                                                                        ProgressBackgroundColor =Color.FromHex("#EEEEEEEE"),
                                                                        StrokeThickness = Device.OnPlatform(10, 10, 20),
                                                                        Radius = Device.OnPlatform(100, 120, 36),
                                                                        BackgroundColor = Color.White
                                                                    },
                                                                    new Label
                                                                    {
                                                                        Text = data.Produced.ToString(),
                                                                        Font = Font.BoldSystemFontOfSize(NamedSize.Large),
                                                                        FontSize = 30,
                                                                        VerticalOptions = LayoutOptions.Center,
                                                                        HorizontalOptions = LayoutOptions.Center,
                                                                        TextColor = Color.Black
                                                                    }
                                                                }
                                                            },
                                                            new Label
                                                            {
                                                                XAlign = TextAlignment.Center,
                                                                VerticalOptions = LayoutOptions.Center,
                                                                HorizontalOptions = LayoutOptions.Center,
                                                                Text = "Produced",
                                                                TextColor = Color.Black,
                                                                FontSize = 15,
                                                            }
                                                          }
                                        },

                                        //Delivered
                                        new StackLayout
                                        {
                                            Orientation = StackOrientation.Vertical,
                                             HorizontalOptions = LayoutOptions.FillAndExpand,
                                            VerticalOptions = LayoutOptions.FillAndExpand,
                                            Spacing = 0,
                                            Children = {
                                                            new Grid
                                                            {
                                                                Children =
                                                                {
                                                                    new CrossPieChartView
                                                                    {
                                                                        Progress = data.Delivered,
                                                                        ProgressColor = Color.Green,
                                                                        ProgressBackgroundColor =Color.FromHex("#EEEEEEEE"),
                                                                        StrokeThickness = Device.OnPlatform(10, 10, 20),
                                                                        Radius = Device.OnPlatform(100, 120, 36),
                                                                        BackgroundColor = Color.White
                                                                    },
                                                                    new Label
                                                                    {
                                                                        Text = data.Delivered.ToString(),
                                                                        Font = Font.BoldSystemFontOfSize(NamedSize.Large),
                                                                        FontSize = 30,
                                                                        VerticalOptions = LayoutOptions.Center,
                                                                        HorizontalOptions = LayoutOptions.Center,
                                                                        TextColor = Color.Black
                                                                    }
                                                                }
                                                            },
                                                            new Label
                                                            {
                                                                XAlign = TextAlignment.Center,
                                                                VerticalOptions = LayoutOptions.Center,
                                                                HorizontalOptions = LayoutOptions.Center,
                                                                Text = "Delivered",
                                                                TextColor = Color.Black,
                                                                FontSize = 15,
                                                            }
                                            }
                                        },

                                        //Installed
                                         new StackLayout
                                        {
                                            Orientation = StackOrientation.Vertical,
                                            HorizontalOptions = LayoutOptions.FillAndExpand,
                                            VerticalOptions = LayoutOptions.FillAndExpand,
                                            Spacing = 0,
                                            Children = {
                                                            new Grid
                                                            {
                                                                VerticalOptions = LayoutOptions.Start,
                                                                Children =
                                                                {
                                                                    new CrossPieChartView
                                                                    {
                                                                        Progress = data.Installed,
                                                                        ProgressColor = Color.Blue,
                                                                        ProgressBackgroundColor =Color.FromHex("#EEEEEEEE"),
                                                                        StrokeThickness = Device.OnPlatform(10, 10, 20),
                                                                        Radius = Device.OnPlatform(100, 120, 36),
                                                                        BackgroundColor = Color.White
                                                                    },
                                                                    new Label
                                                                    {
                                                                        Text = data.Installed.ToString(),
                                                                        Font = Font.BoldSystemFontOfSize(NamedSize.Large),
                                                                        FontSize = 30,
                                                                        VerticalOptions = LayoutOptions.Center,
                                                                        HorizontalOptions = LayoutOptions.Center,
                                                                        TextColor = Color.Black
                                                                    }
                                                                }
                                                            },
                                                            new Label
                                                            {
                                                                XAlign = TextAlignment.Center,
                                                                VerticalOptions = LayoutOptions.Center,
                                                                HorizontalOptions = LayoutOptions.Center,
                                                                Text = "Installed",
                                                                TextColor = Color.Black,
                                                                FontSize = 15,
                                                            }
                                                          }
                                        },


                                    }
                                }
                            }
                        }
                    }
            });
        }
    }
}
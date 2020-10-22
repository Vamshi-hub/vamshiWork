using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml.Media;
using CrossPieCharts.FormsPlugin.Abstractions;
using Xamarin.Forms.Platform.WinRT;
using CrossPieCharts.WindowsPhone81;

[assembly: ExportRenderer(typeof(CrossPieChartView), typeof(CrossPieChartRenderer))]

namespace CrossPieCharts.WindowsPhone81
{
    /// <summary>
    /// Pie Chart control for Windows 10.
    /// </summary>
    public class CrossPieChartRenderer : ViewRenderer<CrossPieChartView, PieChart>
    {

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Control == null || Element == null)
            {
                return;
            }

            if (e.PropertyName == CrossPieChartView.ProgressProperty.PropertyName)
            {
                Control.Percentage = Element.Progress;
            }
            else if (e.PropertyName == CrossPieChartView.RadiusProperty.PropertyName)
            {
                Control.Radius = Element.Radius;
            }
            else if (e.PropertyName == CrossPieChartView.StrokeThicknessProperty.PropertyName)
            {
                Control.StrokeThickness = Element.StrokeThickness;
            }
            else if (e.PropertyName == CrossPieChartView.ProgressColorProperty.PropertyName)
            {
                Control.BackgroundColor = Convert(Element.ProgressColor);
            }
            else if (e.PropertyName == CrossPieChartView.ProgressBackgroundColorProperty.PropertyName)
            {
                Control.BackgroundColor = Convert(Element.ProgressBackgroundColor);
            }
            else if (e.PropertyName == CrossPieChartView.BackgroundColorProperty.PropertyName)
            {
                Control.BackgroundColor = Convert(Element.BackgroundColor);
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<CrossPieChartView> e)
        {
            base.OnElementChanged(e);

            if ((Element != null) && (e.OldElement == null))
            {

                var pieChart = new PieChart
                {

                    StrokeThickness = Element.StrokeThickness,
                    Percentage = Element.Progress,
                    Radius = Element.Radius,
                    //Width = Element.Radius * 2,
                    //Height = Element.Radius * 2,
                    SegmentColor = new SolidColorBrush(Color.FromArgb(
                                                  (byte)(Element.ProgressColor.A * 255),
                                                  (byte)(Element.ProgressColor.R * 255),
                                                  (byte)(Element.ProgressColor.G * 255),
                                                  (byte)(Element.ProgressColor.B * 255))),
                    Segment360Color = new SolidColorBrush(Color.FromArgb(
                                                  (byte)(Element.ProgressBackgroundColor.A * 255),
                                                  (byte)(Element.ProgressBackgroundColor.R * 255),
                                                  (byte)(Element.ProgressBackgroundColor.G * 255),
                                                  (byte)(Element.ProgressBackgroundColor.B * 255))),
                    BackgroundColor = new SolidColorBrush(Color.FromArgb(
                                                  (byte)(Element.BackgroundColor.A * 255),
                                                  (byte)(Element.BackgroundColor.R * 255),
                                                  (byte)(Element.BackgroundColor.G * 255),
                                                  (byte)(Element.BackgroundColor.B * 255))),
                };

                //Height = 300;
                //Width = 300;

                SetNativeControl(pieChart);
            }
        }

        private SolidColorBrush Convert(Xamarin.Forms.Color color)
        {
            return new SolidColorBrush(Color.FromArgb(
                (byte)(color.A * 255),
                (byte)(color.R * 255),
                (byte)(color.G * 255),
                (byte)(color.B * 255)));
        }
    }
}

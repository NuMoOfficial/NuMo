using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Xamarin.Forms;
using OxyPlot.Axes;

namespace NuMo_Tabbed.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NutrientVisual : ContentPage
    {
        PlotView nutrView = new PlotView
        {
            Model = new PlotModel { Title = "My Nutrients" },
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill,
        };
        public NutrientVisual()
        {
            InitializeComponent();

            int[] data = new int[] { 1, 2, 5, 10, 3, 6, 0 };
            String name = "Keto Index Over Time";
            var s1 = new LineSeries()
            {
                Color = OxyColors.PeachPuff,
                MarkerType = MarkerType.Diamond,
                MarkerSize = 4,
                StrokeThickness = 10.0,
                MarkerStroke = OxyColors.PowderBlue,
                MarkerFill = OxyColors.PaleTurquoise,
                MarkerStrokeThickness = 2.0
            };
            for (int i = 0; i < 7; i++)
            {
                s1.Points.Add(new DataPoint(i, data[i]));
            }

            nutrView.Model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "GKI", AxislineStyle = LineStyle.Solid });
            nutrView.Model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Days ago", AxislineStyle = LineStyle.Solid, StartPosition = 1, EndPosition = 0 });
            nutrView.Model.Series.Add(s1);
            nutrView.Model.Title = name;
            Content = nutrView;
        }

    }
}
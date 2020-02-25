using NuMo_Tabbed.ItemViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NuMo_Tabbed.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class KetoPage : ContentPage
    {
        DataAccessor db = DataAccessor.getDataAccessor();
        DateTime date;
        Label indexText = null;

        public KetoPage(DateTime date)
        {
            InitializeComponent();
            this.date = date;

            this.Title = "Glucose Ketone Index";

            var tapped = new TapGestureRecognizer();
            tapped.Tapped += (s, e) =>
            {
                Launcher.OpenAsync(new Uri("https://perfectketo.com/track-your-glucose-ketone-index/"));
            };

            hyperlink.GestureRecognizers.Add(tapped);

            String value = db.getKeto(date);

            if (value != null && value != "")
            {
                double index = Double.Parse(value);
                indexText = new Label { Text = "Your Glucose Ketone Index is " + index.ToString("F"), FontSize = 21, TextColor = Color.FromHex("#00A4A7") };

                ketoStack.Children.Add(indexText);
            }
        }

        public void calculateGKI(object sender, EventArgs args)
        {
            try
            {
                if (indexText != null)
                {
                    ketoStack.Children.Remove(indexText);
                }

                double index = (double.Parse(glucoseEntry.Text) / 18) / double.Parse(ketoEntry.Text);
                indexText = new Label { Text = "Your Glucose Ketone Index is " + index.ToString("F"), FontSize = 21, TextColor = Color.FromHex("#00A4A7") };

                ketoStack.Children.Add(indexText);

                db.saveKeto(date, index);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                DisplayAlert("Invalid Input", "Please enter a number", "OK");
            }
        }
        //open nutrient as function over time line graph
        async void AddLineNutr(object sender, EventArgs args)
        {
            double[] ketoList = new double[7];

            for (int i = 0; i < 7; i++)
            {
                var baseList = db.getKetoHistory(DateTime.Today.AddDays(-i));

                foreach (var item in baseList)
                {
                    ketoList[i] = double.Parse(item.imageString);
                    Console.WriteLine(ketoList[i]);
                }
            }

            await Navigation.PushAsync(new NutrientVisual(ketoList));
        }
    }
}
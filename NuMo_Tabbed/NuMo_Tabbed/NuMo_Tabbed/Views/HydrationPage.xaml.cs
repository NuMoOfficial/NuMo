using Plugin.LocalNotifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NuMo_Tabbed.Views
{

    public partial class HydrationPage : ContentPage
    {
        int numberOfDrinks = 0;
        Image waterLevel = new Image();
        DateTime date = DateTime.Now;
        DataAccessor db = DataAccessor.getDataAccessor();
        //initial setup
        public HydrationPage()
        {
            InitializeComponent();
            this.Title = "The Hydration Station";
            this.Drinks.Text = numberOfDrinks + " glasses of water";

            CrossLocalNotifications.Current.Show("Stay Hydrated!", "Don't forget to drink water today!", 101, DateTime.Now.AddDays(1)); //send a notification to remind to stay hydrated every 24 hours
            String dbDrinks = db.getHydralog(date.ToString("MM/dd/yyyy"));
            if (!dbDrinks.Equals("")) //if todays date is in the db already
            {
                DisplayOldHydration(dbDrinks); //display the stored info for today
            }
            else
            {
                SetNewWaterImage(); //initialize water level at 0 and display
                MainImage.Source = waterLevel.Source;
            }
        }
        //action events for when water is added
        private void AddWater_OnClicked(object sender, EventArgs e)
        {
            // If the add button is clicked
            numberOfDrinks++;
            this.Drinks.Text = numberOfDrinks + " glasses of water";
            SetNewWaterImage(); //refresh water level
            MainImage.Source = waterLevel.Source; //display water level

        }

        //action events for when water loss button is activated
        private void MinusWater_OnClicked(object sender, EventArgs e)
        {
            // If the loss button is picked
            if (numberOfDrinks > 0) numberOfDrinks--;
            this.Drinks.Text = numberOfDrinks + " glasses of water";
            SetNewWaterImage(); //set new appropriate image to display hydration
            MainImage.Source = waterLevel.Source;


        }
        //Date changed
        void dateClicked(object sender, DateChangedEventArgs e)
        {
            String key = date.ToString("MM/dd/yyyy");
            int val = numberOfDrinks;
            db.saveHydralog(key, val + ""); //store the hydration log
            date = e.NewDate;
            String dbDrinks = db.getHydralog(date.ToString("MM/dd/yyyy"));
            //check if new date is in db
            if (!dbDrinks.Equals(""))
            {
                DisplayOldHydration(dbDrinks);
            }
            else
            {
                numberOfDrinks = 0;
                this.Drinks.Text = numberOfDrinks + " glasses of water";
                SetNewWaterImage();
                MainImage.Source = waterLevel.Source;
            }

            this.OnAppearing();
        }

        //sets a new hydration image depending on the number of drinks
        private void SetNewWaterImage()
        {
            String key = date.ToString("MM/dd/yyyy");
            int val = numberOfDrinks;
            db.saveHydralog(key, val + ""); //update the data entry
            switch (numberOfDrinks)
            {
                case 0:
                    waterLevel.Source = "NoWater.png";
                    break;
                case 1:
                    waterLevel.Source = "LowWater.png";
                    break;
                case 2:
                    waterLevel.Source = "LowMedWater.png";
                    break;
                case 3:
                    waterLevel.Source = "LowHighWater.png";
                    break;
                case 4:
                    waterLevel.Source = "MedWater.png";
                    break;
                case 5:
                    waterLevel.Source = "MedLowWater.png";
                    break;
                case 6:
                    waterLevel.Source = "MedMedWater.png";
                    break;
                case 7:
                    waterLevel.Source = "MedHighWater.png";
                    break;
                default:
                    waterLevel.Source = "HighWater.png";
                    break;

            }

        }
        //sets the hydration to the level specified in db for selected date
        private void DisplayOldHydration(String dateKey)
        {
            int newDrinks = Convert.ToInt32(dateKey);
            numberOfDrinks = newDrinks;
            this.Drinks.Text = numberOfDrinks + " glasses of water";
            SetNewWaterImage();
            MainImage.Source = waterLevel.Source;
        }

    }
}
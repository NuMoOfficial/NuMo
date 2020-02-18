using NuMo_Tabbed.DatabaseItems;
using NuMo_Tabbed.ItemViews;
using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NuMo_Tabbed.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        DateTime date;
        List<IMyDayViewItem> ViewItemList;

        //Initially we only want to see info for 1 day.
        int daysToLoad = 1;

        public HomePage()
        {
            InitializeComponent();

            //subscribe to events to refresh the page and update a food item
            MessagingCenter.Subscribe<MyDayFoodItem>(this, "RefreshMyDay", (sender) =>
            {
                this.OnAppearing();
            });
            MessagingCenter.Subscribe<MyDayFoodItem>(this, "UpdateMyDayFoodItem", (sender) =>
            {
                UpdateMyDayFoodItem(sender);
            });


            ViewItemList = new List<IMyDayViewItem>();
            var timeArr = new String[] { "Today", "This Week", "This Month" };
            timePicker.ItemsSource = timeArr;
            timePicker.SelectedItem = timeArr[0];

            //ToolbarItem plus = new ToolbarItem();
            //plus.Icon = "ic_add_black_24dp.png";
            //plus.Clicked += AddButton;
            //ToolbarItems.Add(plus);

            //setting the values of the calendar
            //timeLengthChoice.SelectedIndex = 0;
            date = datePicker.Date;
        }

        //Set the profile picture, default to logo if one does not exist.
        void UpdateMyDayPicture()
        {
            var db = DataAccessor.getDataAccessor();
            var oldItems = db.getReminders("ProfileImage");
            MyDayReminderItem myDayReminderItem = null;
            if (oldItems.Count > 0)
                myDayReminderItem = oldItems.First<MyDayReminderItem>();
            if (myDayReminderItem != null)
            {
                //pic.Source = myDayReminderItem.ReminderImage.Source;
            }
            else
            {
                //pic.Source = "ic_logo_24dp.png";
            }
        }

        //Update a food item's information
        async void UpdateMyDayFoodItem(MyDayFoodItem item)
        {
            AddItemUpdate update = new AddItemUpdate(item);
            await Navigation.PushAsync(update.nutrFacts);

        }

        //Plus button in top right to add a food item to today's history
        async void AddButton(object sender, EventArgs args)
        {
            date = datePicker.Date;
            await Navigation.PushAsync(new AddItemToFoodHistory(date));
        }


        async void AddKeto(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new KetoPage(datePicker.Date));
        }

        //Display the food items associated with today, and back in time to the number of selected days.
        void OnItemsClicked()
        {

            Image pic2 = new Image();
            if (Application.Current.Properties.ContainsKey("Profile Pic"))
            {
                pic2 = Application.Current.Properties["Profile Pic"] as Image;
                //pic.Source = pic2.Source;
            }

            listView.BeginRefresh();
            listView.ItemsSource = null;
            var db = DataAccessor.getDataAccessor();
            ViewItemList.Clear();
            var ReminderList = new List<MyDayReminderItem>();
            for (int i = 0; i < daysToLoad; i++)
            {
                ReminderList = db.getReminders(date.AddDays(-i).ToString());
                foreach (var item in ReminderList)
                {
                    ViewItemList.Add(item);
                }
            }
            for (int i = 0; i < daysToLoad; i++)
            {
                var baseList = db.getFoodHistory(date.AddDays(-i).ToString());

                foreach (var item in baseList)
                {
                    ViewItemList.Add(item);
                }
            }
            listView.ItemsSource = ViewItemList;
            listView.EndRefresh();
        }

        //Ensure only Items OR nutrients is selected, never both.
        void viewToggle(object sender, EventArgs args)
        {
            var toggle = (Switch)sender;

            if (toggle.IsToggled)
            {
                OnNutrientsClicked();
            }
            else
            {
                OnItemsClicked();
            }
        }

        //Display nutrient info for the day/history range selected.
        void OnNutrientsClicked()
        {
            listView.BeginRefresh();
            listView.ItemsSource = null;

            var nutrientList = getNutrients();
            ViewItemList.Clear();
            foreach (var item in nutrientList)
            {
                ViewItemList.Add(item);
            }
            listView.ItemsSource = ViewItemList;
            listView.EndRefresh();
        }

        //Get all the nutrients associated with the current selection of days.
        private List<Nutrient> getNutrients()
        {
            var db = DataAccessor.getDataAccessor();
            var baseList = new List<FoodHistoryItem>();
            for (int i = 0; i < daysToLoad; i++)
            {
                baseList.AddRange(db.getFoodHistoryList(date.AddDays(-i).ToString()));
            }
            var nutrientList = db.getNutrientsFromHistoryList(baseList);

            foreach (var item in nutrientList)
            {
                if (item.DisplayName != "Omega6/3 Ratio")
                    item.quantity /= daysToLoad;
            }
            return nutrientList;
        }

        //Open a page to create a new Reminder item for the selected day.
        async void OnReminderClicked(object sender, EventArgs args)
        {
            //Go to camera
            await Navigation.PushAsync(new CameraStuff());
        }

        //Allow user to update the current date
        void dateClicked(object sender, DateChangedEventArgs e)
        {
            date = e.NewDate;
            this.OnAppearing();
        }

        //Allow user to update their choice of time range
        void OnTimeLengthChoiceChanged(object sender, EventArgs e)
        {
            //this.Title = timeLengthChoice.Items.ElementAt(timeLengthChoice.SelectedIndex);
            if ((String)timePicker.SelectedItem == "Today")
            {
                daysToLoad = 1;
            }
            else if ((String)timePicker.SelectedItem == "This Week")
            {
                daysToLoad = 7;
            }
            else if ((String)timePicker.SelectedItem == "This Month")
            {
                daysToLoad = 30;
            }
            this.OnAppearing();
        }

        //Make sure the most current information is being used.
        protected override void OnAppearing()
        {
            UpdateMyDayPicture();
            base.OnAppearing();
            nutrientToggle.IsToggled = false;
            OnItemsClicked();
        }
    }
}
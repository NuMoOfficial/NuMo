using NuMo_Tabbed.DatabaseItems;
using System;
using System.Diagnostics;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading;

namespace NuMo_Tabbed.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddFoodPage : ContentPage
    {
        public NumoNameSearch selectedResult;

        public Entry searchbar;

        public NutrFacts nutrFacts;

        DateTime date = DateTime.Now;


        public AddFoodPage()
        {
            InitializeComponent();


            searchbar = new Entry
            {
                Placeholder = "Search item",
            };


            //Get search results for every key entry into the search bar and display them
            searchbar.TextChanged += (sender, e) =>
            {
                String searchItem = e.NewTextValue;
                var db = DataAccessor.getDataAccessor();
                var searchResults = db.searchName(searchItem);
                searchList.ItemsSource = searchResults;
            };

            mainStack.Children.Insert(0, searchbar);
        }

        public void changeText(string text)
        {
            searchbar.Text = text;
            searchbar.Focus();
        }

        //update search results.
        public void searchForMatches(object sender, TextChangedEventArgs e)
        {
            var searchItem = e.NewTextValue;
            var db = DataAccessor.getDataAccessor();
            var searchResults = db.searchName(searchItem);
            searchList.ItemsSource = searchResults;
        }

        //Opens a new window when a search item is clicked.
        public async void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
            {
                return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
            }

            //navigate to NutrFacts page, passing in the event arguments
            nutrFacts = new NutrFacts(this, (NumoNameSearch)e.Item);
            await Navigation.PushAsync(nutrFacts);

            selectedResult = (NumoNameSearch)e.Item;
        }

        //Clear all fields to make it obvious the button press had an impact.
        public virtual void saveButtonClicked(object sender, EventArgs args)
        {
            if (nutrFacts != null)
            {
                var nutrQuantifier = nutrFacts.getQuantifier();
                var nutrQuantity = nutrFacts.Quantity;
                //save info to database here
                if (selectedResult != null && nutrQuantity != null && !nutrQuantity.Equals("0") && nutrQuantifier != null && date != null)
                {
                    var db = DataAccessor.getDataAccessor();

                    //Increment the times this item has been selected so it will get priority in the future
                    db.incrementTimesSearched(selectedResult.food_no);

                    FoodHistoryItem item = new FoodHistoryItem();
                    //need to add date, quantity, quantifiers, and food_no to this item
                    item.food_no = selectedResult.food_no;
                    this.date = DateTime.Today;
                    item.Date = date.ToString();
                    item.Quantity = Convert.ToDouble(nutrQuantity);
                    item.Quantifier = nutrQuantifier;


                    //Add to our database
                    db.addFoodHistory(item);
                }
            }
        }
    }
}
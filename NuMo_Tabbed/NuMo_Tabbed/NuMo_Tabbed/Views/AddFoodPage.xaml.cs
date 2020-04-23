using NuMo_Tabbed.DatabaseItems;
using System;
using System.Diagnostics;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
                    item.Date = date.ToString();
                    item.Quantity = Convert.ToDouble(nutrQuantity);
                    item.Quantifier = nutrQuantifier;

                    //Add to our database
                    db.addFoodHistory(item);

                    updateUndoButton();
                }
            }
        }

        protected void updateUndoButton()
        {
            // Update the undo button
            ToolbarItem undoButton = this.FindByName<ToolbarItem>("undoButton");

            if (undoButton.Text.Equals(""))
            {
                undoButton.Text = "Undo";
            }
            else if (undoButton.Text.Equals("Undo"))
            {
                undoButton.Text = "";
            }

        }

        async void undoButtonClicked(object sender, EventArgs args)
        {
            String action = await DisplayActionSheet("Do you want to remove\nthe last food added?", "Cancel", "Undo", "");

            if (action.Equals("Undo"))
            {
                // Get Memento from Caretaker
                Caretaker ct = Caretaker.getCaretaker();
                Memento m = ct.getMemento();
                await DisplayAlert("Got Memento from Caretaker", "", "OK");
                bool success = m.getLastState();

                if (success)
                {
                    await DisplayAlert("Rollback Successful! Yay!", "", "OK");
                    // Remove undo button
                    updateUndoButton();

                    await DisplayAlert("Undo Complete", "", "OK");
                }
                else
                {
                    await DisplayAlert("Rollback unsuccessful :(", "", "OK");
                }
            }
            
        }
    }
}
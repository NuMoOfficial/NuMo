using NuMo_Tabbed.DatabaseItems;
using NuMo_Tabbed.ItemViews;
using NuMo_Tabbed.Services;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NuMo_Tabbed.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateRecipePage : ContentPage
    {
        List<FoodHistoryItem> recipeList = new List<FoodHistoryItem>();

        public CreateRecipePage()
        {
            InitializeComponent();
        }

        async void SaveRecipe(object sender, EventArgs e)
        {
            //check if inputs are valid
            if (recipeName.Text == null || recipeName.Text == "")
            {
                await DisplayAlert("Please input RECIPE NAME", "", "OK");
            }
            else if (quantity.Text == null || quantity.Text == "" || quantity.Text == "0")
            {
                await DisplayAlert("Please input QUANTITY", "example: 1.5 or 3", "OK");
            }
            else if (quantifier.Text == null || quantifier.Text == "")
            {
                await DisplayAlert("Please input QUANTIFIER NAME", "", "OK");
            }
            else if (recipeList.Count == 0)
            {
                await DisplayAlert("Please add an INGREDIENT", "", "OK");
            }
            else //perform save
            {

                //access database
                var db = DataAccessor.getDataAccessor();
                var nutrientList = db.getNutrientsFromHistoryList(recipeList);
                double totalQuantity = 0;

                double servingQuantity = Convert.ToDouble(quantity.Text);

                foreach (var item in recipeList)
                {
                    totalQuantity += UnitConverter.getMultiplier(item.Quantifier, item.food_no) * item.Quantity;
                }
                foreach (var item in nutrientList)
                {
                    item.quantity /= (totalQuantity);
                    item.quantity *= 100;//database is in 100g standard.
                }
                var servingMultiplier = (totalQuantity / servingQuantity) / 100;
                db.createFoodItem(nutrientList, recipeName.Text, servingMultiplier, quantifier.Text.ToString());

                //clear variables
                recipeList.Clear();
                recipeName.Text = "";
                ingredientList.ItemsSource = "";
                quantity.Text = "";
                quantifier.Text = "";

                // Update the undo button
                ToolbarItem undoButton = this.FindByName<ToolbarItem>("undoButton");
                undoButton.Text = "Undo";

                await DisplayAlert("Recipe Saved", "", "OK");
            }
        }

        async void AddIngredient(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddFoodToRecipe(recipeList));
        }

        //refresh everytime we return from the addItemPage
        protected override void OnAppearing()
        {
            base.OnAppearing();
            ingredientList.BeginRefresh();
            ingredientList.ItemsSource = null;

            List<MyDayFoodItem> foodItems = new List<MyDayFoodItem>();
            foreach (var item in recipeList)
            {
                var foodItem = new MyDayFoodItem();
                foodItem.DisplayName = item.DisplayName;
                foodItem.quantity = "(" + item.Quantity + ")" + " " + item.Quantifier;
                foodItems.Add(foodItem);
            }
            ingredientList.ItemsSource = foodItems;
            ingredientList.EndRefresh();
        }

        async void undoButtonClicked(object sender, EventArgs args)
        {
            String action = await DisplayActionSheet("Do you want to remove\nthe last recipe added?", "Cancel", "Undo", "");

            if (action.Equals("Undo"))
            {
                // Get Memento from Caretaker
                Caretaker ct = Caretaker.getCaretaker();
                Memento m = ct.getMemento();
                bool success = m.getLastState();

                if (success)
                {
                    // Remove undo button
                    ToolbarItem undoButton = this.FindByName<ToolbarItem>("undoButton");
                    undoButton.Text = "";

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
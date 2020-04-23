using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NuMo_Tabbed.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateFoodPage : ContentPage
    {
        //index in inputValues corresponds to value at that index in mappings[]
        public static string[] inputValues = {
            "Protein(g)", "Carbs By Diff(g)", "Total Sugars(g)",
            "Total Dietary Fiber(g)", "Calcium(mg)", "Iron(mg)", "Ferrous Sulfate",
            "Magnesium(mg)", "Phosphorus(mg)", "Potassium(mg)",
            "Sodium(mg)", "Zinc(mg)", "Copper(mg)",
            "Magnanese(mg)", "Selenium(µg)", "Selenomethionine",
            "Vitamin A RAE(µg)", "Beta Carotene", "Vitamin E(mg)", "alpha (α) tocopherol", "Vitamin C(mg)", "Ascorbic Acid",
            "Vitamin B1", "Thiamin(mg)", "Vitamin B2", "Riboflavin(mg)", "Vitamin B3", "Niacin(mg)", "Niacinimide",
            "Pantothenic Acid(mg)", "Vitamin B6(mg)", "Folate(µg)",
            "Vitamin B12(µg)", "Vitamin K(µg)",  "Omega 6(g)", "LA", "AA",
            "Omega 3(g)", "DHA", "EPA", "ALA" };
        //the nut_id in the database.
        public static int[] mappings = {
            203, 205, 269,
            291, 301, 303, 303,
             304, 305, 306,
            307, 309, 312,
            315, 317, 317,
            320, 320, 323, 323, 401, 401,
            404, 404, 405, 405, 406, 406, 406,
            410, 415, 417,
            418,430, 618, 618, 618,
            619, 619, 619, 619 };

        public CreateFoodPage()
        {
            InitializeComponent();
            foreach (var item in inputValues)
            {
                var entryCell = new EntryCell()
                {
                    Label = item,
                    Keyboard = Keyboard.Numeric,
                    //WidthRequest = 65,
                    Text = "0",
                    //Margin = new Thickness(0, 0, 50, 0),
                    //HorizontalOptions = LayoutOptions.EndAndExpand
                };
                nutrientSection.Add(entryCell);
            }

        }

        //Create food item from user data and save to database
        async void saveButtonClicked(object sender, EventArgs args)
        {
            //check if inputs are valid
            if (CreateItemName.Text == null || CreateItemName.Text == "")
            {
                await DisplayAlert("Please input NAME", "", "OK");
            }
            else if (quantity.Text == null || quantity.Text == "" || quantity.Text == "0")
            {
                await DisplayAlert("Please input QUANTITY", "example: 1.5 or 3", "OK");
            }
            else if (quantifier.Text == null || quantifier.Text == "")
            {
                await DisplayAlert("Please input SERVING", "", "OK");
            }
            else if (gramsAmount.Text == null || gramsAmount.Text == "")
            {
                await DisplayAlert("Please input GRAMS", "", "OK");
            }
            else //perform save
            {
                var db = DataAccessor.getDataAccessor();

                var nutrientList = new List<Nutrient>();
                //multiplier deals with getting food to the 100g rate by the time it's in the db.
                var multiplier = 1 / Convert.ToDouble(gramsAmount.Text);

                foreach (var item in nutrientSection)
                {

                    var entryCellItem = (EntryCell)item;
                    if (Convert.ToDouble(entryCellItem.Text) > 0)
                    {
                        var oldNutrient = nutrientList.FindLast(i => i.dbNo == mappings[Array.IndexOf(inputValues, entryCellItem.Label)]);
                        if (oldNutrient == null)
                        {
                            var nutrient = new Nutrient();
                            nutrient.dbNo = mappings[Array.IndexOf(inputValues, entryCellItem.Label)];
                            nutrient.quantity = Convert.ToDouble(entryCellItem.Text) * multiplier;
                            nutrientList.Add(nutrient);
                        }
                        else
                        {
                            oldNutrient.quantity += Convert.ToDouble(entryCellItem.Text) * multiplier;
                        }
                    }
                }
                //check if there were any nutrients input
                if (nutrientList.Count == 0)
                {
                    await DisplayAlert("Please input NUTRIENTS", "", "OK");
                }
                else
                {
                    //servingMultiplier for the custom unit
                    var servingMultiplier = Convert.ToDouble(quantity.Text) / (multiplier);

                    //put into database
                    db.createFoodItem(nutrientList, CreateItemName.Text, servingMultiplier, quantifier.Text.ToString());
                    //wipe inputs
                    foreach (var item in nutrientSection)
                    {
                        var entryCellItem = (EntryCell)item;
                        entryCellItem.Text = "0";
                    }
                    quantity.Text = "";
                    quantifier.Text = "";
                    gramsAmount.Text = "";
                    CreateItemName.Text = "";

                    // Update the undo button
                    ToolbarItem undoButton = this.FindByName<ToolbarItem>("undoButton");
                    undoButton.Text = "Undo";

                    //alert user it was saved
                    await DisplayAlert("Item Saved", "", "OK");
                }
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
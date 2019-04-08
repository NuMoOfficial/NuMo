using NuMo_Tabbed.DatabaseItems;
using NuMo_Tabbed.Views;
using System;
using System.Collections.Generic;

// Add item to recipe page. This page is simply an add on to the add recipe page, where the recipe can intake more ingredients

namespace NuMo_Tabbed.Services
{
    public class AddFoodToRecipe : AddFoodPage
    {
        List<FoodHistoryItem> recipeList;

        public AddFoodToRecipe(List<FoodHistoryItem> temp) : base()
        {
            recipeList = temp;
        }

        //Append created item to the recipeList.
        public override void saveButtonClicked(object sender, EventArgs args)
        {
            var result = new FoodHistoryItem();
            result.food_no = selectedResult.food_no;

            //nutrFacts is from parenting class
            result.Quantity = Convert.ToDouble(nutrFacts.Quantity);
            result.Quantifier = nutrFacts.getQuantifier();
            recipeList.Add(result);
            Navigation.RemovePage(this);
            base.OnBackButtonPressed();
        }
    }
}

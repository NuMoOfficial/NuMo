using NuMo_Tabbed.DatabaseItems;
using NuMo_Tabbed.ItemViews;
using NuMo_Tabbed.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuMo_Tabbed
{
    //Update a foodhistory item from MyDayPage(date cannot be changed)
    class AddItemUpdate : AddFoodPage
    {
        MyDayFoodItem myDayItem;
        NumoNameSearch search;

        public AddItemUpdate(MyDayFoodItem item)
        {
            myDayItem = item;

            //get food item from database
            var db = DataAccessor.getDataAccessor();
            FoodHistoryItem foodHistoryItem = db.getFoodHistoryItem(item.id);

            //store food info in NumoNameSearch var
            search = new NumoNameSearch();
            search.food_no = foodHistoryItem.food_no;
            search.name = foodHistoryItem.DisplayName;

            //create new instance to display food info
            nutrFacts = new NutrFacts(this, search);

            //update the values being displayed
            nutrFacts.DescriptView = foodHistoryItem.DisplayName;
            nutrFacts.Quantity = foodHistoryItem.Quantity.ToString();
            nutrFacts.UnitPickerText = foodHistoryItem.Quantifier;
            nutrFacts.selectedResult = search;
            nutrFacts.updateUnitPickerWithCustomOptions();


        }

        async public void SaveButtonClicked(object sender, EventArgs e)
        {
            var nutrQuantifier = nutrFacts.getQuantifier();
            var nutrQuantity = nutrFacts.Quantity;

            if (search != null && nutrQuantity != null && !nutrQuantity.Equals("0") && nutrQuantifier != null)
            {
                var db = DataAccessor.getDataAccessor();

                FoodHistoryItem item = new FoodHistoryItem();
                //need to add date, quantity, quantifiers, and food_no to this item
                item.food_no = search.food_no;
                item.Quantity = Convert.ToDouble(nutrQuantity);
                item.Quantifier = nutrQuantifier;
                item.History_Id = myDayItem.id;

                //Add to our database
                //bool success = db.updateFoodHistory(item, myDayItem.id, saveMemento: false);
                bool success = db.updateFoodHistory(item, saveMemento: false);
                if (success)
                {
                    await DisplayAlert("Update successful", "", "OK");
                }
                else
                {
                    await DisplayAlert("Update unsuccessful", "", "OK");
                }
            }
            MyDayFoodItem.sendRefresh();
        } 
    }
}

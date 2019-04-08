using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuMo_Tabbed
{
    //The FoodItem class contains logic for organizing a list of individual nutrients. There should be no repeats in the list.
    class FoodItem
    {
        public List<Nutrient> nutrients;

        public FoodItem()
        {
        }

        //Create a consolodated FoodItem from a list of them.
        public FoodItem(List<FoodItem> foodItems)
        {
            nutrients = new List<Nutrient>();
            foreach (var foodItem in foodItems)
            {
                foreach (var nutrient in foodItem.nutrients)
                {
                    var findResult = nutrients.Find(i => i.DisplayName == nutrient.DisplayName);
                    if (findResult != null)
                    {
                        findResult.quantity += nutrient.quantity;
                    }
                    else
                    {
                        nutrients.Add(nutrient);
                    }
                }
            }
            //freshly calculate the omega6/3 ratio
            calculate63ratio(nutrients);
        }

        //Create a FoodItem from a series of food data, along with the appropriate multiplie to get to 100g units.
        public FoodItem(List<NutData> values, double multiplier)
        {
            nutrients = new List<Nutrient>();
            foreach (var value in values)
            {
                Nutrient nutrient = new Nutrient();
                nutrient.quantity = value.nutr_value * multiplier / 100;
                nutrient.dbNo = value.nutr_no;
                switch (value.nutr_no)
                {
                    case 208:
                        nutrient.DisplayName = "Calories";
                        nutrient.quantifier = "kcal";
                        break;
                    case 203:
                        nutrient.DisplayName = "Protein(g)";
                        nutrient.quantifier = "g";
                        break;
                    case 205:
                        nutrient.DisplayName = "Carbohydrates(g)";
                        nutrient.quantifier = "g";
                        break;
                    case 269:
                        nutrient.DisplayName = "Total Sugars(g)";
                        nutrient.quantifier = "g";
                        break;
                    case 291:
                        nutrient.DisplayName = "Total Dietary Fiber(g)";
                        nutrient.quantifier = "g";
                        break;
                    case 301:
                        nutrient.DisplayName = "Calcium(mg)";
                        nutrient.quantifier = "mg";
                        break;
                    case 303:
                        nutrient.DisplayName = "Iron(mg)";
                        nutrient.quantifier = "mg";
                        break;
                    case 304:
                        nutrient.DisplayName = "Magnesium(mg)";
                        nutrient.quantifier = "mg";
                        break;
                    case 305:
                        nutrient.DisplayName = "Phosphorus(mg)";
                        nutrient.quantifier = "mg";
                        break;
                    case 306:
                        nutrient.DisplayName = "Potassium(mg)";
                        nutrient.quantifier = "mg";
                        break;
                    case 307:
                        nutrient.DisplayName = "Sodium(mg)";
                        nutrient.quantifier = "g";
                        break;
                    case 309:
                        nutrient.DisplayName = "Zinc(mg)";
                        nutrient.quantifier = "mg";
                        break;
                    case 312:
                        nutrient.DisplayName = "Copper(mg)";
                        nutrient.quantifier = "mg";
                        break;
                    case 315:
                        nutrient.DisplayName = "Magnanese(mg)";
                        nutrient.quantifier = "mg";
                        break;
                    case 317:
                        nutrient.DisplayName = "Selenium(µg)";
                        nutrient.quantifier = "µg";
                        break;
                    case 320:
                        nutrient.DisplayName = "Vitamin A RAE(µg)";
                        nutrient.quantifier = "µg";
                        break;
                    case 323:
                        nutrient.DisplayName = "Vitamin E(mg)";
                        nutrient.quantifier = "mg";
                        break;
                    case 401:
                        nutrient.DisplayName = "Vitamin C(mg)";
                        nutrient.quantifier = "mg";
                        break;
                    case 404:
                        nutrient.DisplayName = "Thiamin(mg)";
                        nutrient.quantifier = "mg";
                        break;
                    case 405:
                        nutrient.DisplayName = "Riboflavin(mg)";
                        nutrient.quantifier = "mg";
                        break;
                    case 406:
                        nutrient.DisplayName = "Niacin(mg)";
                        nutrient.quantifier = "mg";
                        break;
                    case 410:
                        nutrient.DisplayName = "Pantothenic Acid(mg)";
                        nutrient.quantifier = "mg";
                        break;
                    case 415:
                        nutrient.DisplayName = "Vitamin B6(mg)";
                        nutrient.quantifier = "mg";
                        break;
                    case 417:
                        nutrient.DisplayName = "Folate(µg)";
                        nutrient.quantifier = "µg";
                        break;
                    case 418:
                        nutrient.DisplayName = "Vitamin B12(µg)";
                        nutrient.quantifier = "µg";
                        break;
                    case 430:
                        nutrient.DisplayName = "Vitamin K(µg)";
                        nutrient.quantifier = "µg";
                        break;
                    case 618:
                        nutrient.DisplayName = "Omega 6 1(g)";
                        nutrient.quantifier = "g";
                        break;
                    case 670:
                        nutrient.DisplayName = "Omega 6 2(g)";
                        nutrient.quantifier = "g";
                        break;
                    case 672:
                        nutrient.DisplayName = "Omega 6 3(g)";
                        nutrient.quantifier = "g";
                        break;
                    case 685:
                        nutrient.DisplayName = "Omega 6 4(g)";
                        nutrient.quantifier = "g";
                        nutrients.Add(nutrient);
                        nutrient = new Nutrient();
                        nutrient.DisplayName = "Omega 3 7(g)";
                        nutrient.quantifier = "g";
                        nutrient.quantity = value.nutr_value / 100;
                        break;
                    case 620:
                        nutrient.DisplayName = "Omega 6 5(g)";
                        nutrient.quantifier = "g";
                        break;
                    case 853:
                        nutrient.DisplayName = "Omega 6 6(g)";
                        nutrient.quantifier = "g";
                        break;
                    case 619:
                        nutrient.DisplayName = "Omega 3 1(g)";
                        nutrient.quantifier = "g";
                        break;
                    case 858:
                        nutrient.DisplayName = "Omega 3 2(g)";
                        nutrient.quantifier = "g";
                        break;
                    case 852:
                        nutrient.DisplayName = "Omega 3 3(g)";
                        nutrient.quantifier = "g";
                        break;
                    case 621:
                        nutrient.DisplayName = "Omega 3 4(g)";
                        nutrient.quantifier = "g";
                        break;
                    case 629:
                        nutrient.DisplayName = "Omega 3 5(g)";
                        nutrient.quantifier = "g";
                        break;
                    case 631:
                        nutrient.DisplayName = "Omega 3 6(g)";
                        nutrient.quantifier = "g";
                        break;

                    default:
                        break;
                }
                if (nutrient.DisplayName != null)
                {
                    nutrients.Add(nutrient);
                }
            }
            //Special logic for totaling up omega 6 nutrient and omega 3 nutrient
            Nutrient omega6 = new Nutrient();
            omega6.DisplayName = "Omega 6 total(g)";
            omega6.quantifier = "g";
            omega6.quantity = 0;
            var omega6elements = nutrients.FindAll(i => i.DisplayName.Equals("Omega 6 1(g)") || i.DisplayName.Equals("Omega 6 2(g)") || i.DisplayName.Equals("Omega 6 3(g)") || i.DisplayName.Equals("Omega 6 4(g)") || i.DisplayName.Equals("Omega 6 5(g)") || i.DisplayName.Equals("Omega 6 6(g)"));
            foreach (var element in omega6elements)
            {
                omega6.quantity += element.quantity;
            }
            //This entry is subtracted from our total due to direction from Ed Dratz
            var omega6special = nutrients.Find(i => i.DisplayName.Equals("Omega 6 2(g)"));
            if (omega6special != null)
            {
                omega6.quantity -= 2 * omega6special.quantity;
            }
            if (omega6.quantity > 0)
                nutrients.Add(omega6);
            Nutrient omega3 = new Nutrient();
            omega3.DisplayName = "Omega 3 total(g)";
            omega3.quantifier = "g";
            omega3.quantity = 0;
            var omega3elements = nutrients.FindAll(i => i.DisplayName.Equals("Omega 3 1(g)") || i.DisplayName.Equals("Omega 3 2(g)") || i.DisplayName.Equals("Omega 3 3(g)") || i.DisplayName.Equals("Omega 3 4(g)") || i.DisplayName.Equals("Omega 3 5(g)") || i.DisplayName.Equals("Omega 3 6(g)") || i.DisplayName.Equals("Omega 3 7(g)"));
            foreach (var element in omega3elements)
            {
                omega3.quantity += element.quantity;
            }
            //This entry is subtracted from our total due to direction from Ed Dratz
            var omega3special = nutrients.Find(i => i.DisplayName.Equals("Omega 3 7(g)"));
            if (omega3special != null)
            {
                omega3.quantity -= 2 * omega3special.quantity;
            }
            if (omega3.quantity > 0)
                nutrients.Add(omega3);
            calculate63ratio(nutrients);

            Nutrient netCarbs = new Nutrient();
            netCarbs.DisplayName = "Net Carbohydrates(g)";
            netCarbs.quantifier = "g";
            var totalCarbs = nutrients.Find(i => i.DisplayName.Equals("Carbs By Diff(g)"));
            if (totalCarbs != null)
            {
                netCarbs.quantity = totalCarbs.quantity;
                var dietaryFiber = nutrients.Find(i => i.DisplayName.Equals("Total Dietary Fiber(g)"));
                if (dietaryFiber != null)
                {
                    netCarbs.quantity -= dietaryFiber.quantity;
                }
                nutrients.Add(netCarbs);
            }
        }

        //Calculates total O6/total O3 if applicable
        public static void calculate63ratio(List<Nutrient> nutrients)
        {
            var omega3 = nutrients.Find(i => i.DisplayName.Equals("Omega 3 total(g)"));
            var omega6 = nutrients.Find(i => i.DisplayName.Equals("Omega 6 total(g)"));
            if ((omega3 != null) && omega3.quantity > 0)
            {
                Nutrient omega63ratio = new NuMo_Tabbed.Nutrient();
                omega63ratio.quantity = 0;
                omega63ratio.DisplayName = "Omega6/3 Ratio";
                omega63ratio.quantifier = "special";
                if (omega6 != null)
                    omega63ratio.quantity = omega6.quantity / omega3.quantity;
                nutrients.RemoveAll(i => i.DisplayName.Equals("Omega6/3 Ratio"));
                nutrients.Add(omega63ratio);
            }
        }

        //removes excessive omega entries, we assume users only want totals and ratio.
        public void stripExtraOmegs()
        {
            nutrients.RemoveAll(i => i.DisplayName.Contains("Omega ") && !(i.DisplayName.Equals("Omega 3 total(g)") || i.DisplayName.Equals("Omega 6 total(g)")));
        }

    }
}

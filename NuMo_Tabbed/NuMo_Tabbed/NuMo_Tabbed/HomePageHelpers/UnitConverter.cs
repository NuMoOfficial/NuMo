using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuMo_Tabbed
{
    static class UnitConverter
    {
        //standard units
        public static string[] standardUnits = { "Grams", "KiloGrams", "Milligrams", "Pounds", "Ounces" };
        //standard weights, position must correspond to standardUnits
        public static double[] standardWeights = { 1.0, 1000.0, .001, 453.592, 28.3495 };

        //gets the multiplier to convert to grams
        public static double getMultiplier(string quantifier, int food_no)
        {
            var quantityIndex = -1;
            double multiplier = 1.0;
            for (int i = 0; i < standardUnits.Length; i++)
            {
                if (quantifier.Equals(standardUnits[i]))
                {
                    quantityIndex = i;
                    break;
                }
            }
            if (quantityIndex >= 0)
            {
                multiplier = standardWeights[quantityIndex];
            }
            else
            {
                //custom units code here
                var db = DataAccessor.getDataAccessor();
                var convertItems = db.getCustomQuantifiers(food_no);
                foreach (var item in convertItems)
                {
                    if (item.
                        name.
                        Equals
                        (quantifier))
                    {
                        multiplier = item.gramsMultiplier;
                        break;
                    }
                }
            }
            return multiplier;
        }
    }
}

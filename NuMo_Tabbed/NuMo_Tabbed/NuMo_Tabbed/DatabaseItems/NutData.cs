using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuMo_Tabbed
{
    //This class holds information of a single nutrient item with relation to a food item.
    class NutData
    {
        public int food_no { get; set; }
        public int nutr_no { get; set; }
        public double nutr_value { get; set; }
    }
}

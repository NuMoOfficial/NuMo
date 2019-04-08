using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuMo_Tabbed
{
    //This class is for keeping track of user food item search results.
    public class NumoNameSearch
    {
        public int food_no { get; set; }
        public string name { get; set; }
        public List<ConvertItem> convertions { get; set; }

        public NumoNameSearch()
        {
            convertions = new List<ConvertItem>();
        }

        //Override for easy display of search result.
        public override String ToString()
        {
            return name;
        }

        public class ConvertItem
        {
            public string name { get; set; }
            public double gramsMultiplier { get; set; }
            public ConvertItem() { }
            public ConvertItem(string name, double multiplier)
            {
                this.name = name;
                this.gramsMultiplier = multiplier;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuMo_Tabbed
{
    //This class holds data on a single nutrient and can be displayed on the MyDay page.
    public class Nutrient : IMyDayViewItem
    {
        public String DisplayName { get; set; }
        public String quantifier { get; set; }
        public double quantity { get; set; }
        public int dbNo { get; set; }
    }
}

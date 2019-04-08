using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuMo_Tabbed.DatabaseItems
{
    //FoodHistoryItem is used to keep track of food quantities for specific days. 
    public class FoodHistoryItem
    {
        public String Date { get; set; }
        public double Quantity { get; set; }
        public String Quantifier { get; set; }
        public int food_no { get; set; }
        public int History_Id { get; set; }
        public String DisplayName
        {
            get
            {
                var db = DataAccessor.getDataAccessor();
                return db.getNameFromID(food_no);
            }
        }
    }
}

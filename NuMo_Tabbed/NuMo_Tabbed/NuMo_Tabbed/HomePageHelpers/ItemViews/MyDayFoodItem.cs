using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace NuMo_Tabbed.ItemViews
{
    //Food item entries displayed on the MyDayPage
    class MyDayFoodItem : IMyDayViewItem
    {
        public String DisplayName { get; set; }
        public String quantity { get; set; }
        public int id { get; set; }
        public ICommand OnEditEvent { get; set; }
        public ICommand OnDeleteEvent { get; set; }

        public MyDayFoodItem()
        {
            OnEditEvent = new Command(OnEdit);
            OnDeleteEvent = new Command(OnDelete);
        }

        //If a user edits an item, we need to reopen the item in an addItemPage, this command is sent to MyDayPage.
        void OnEdit()
        {
            MessagingCenter.Send(this, "UpdateMyDayFoodItem");
        }

        //On deletion we delete the entry from the database and then refresh the mydayPage
        void OnDelete()
        {
            var db = DataAccessor.getDataAccessor();
            bool success = db.deleteFoodHistoryItem(id);
            //Refresh the mydaypage
            sendRefresh();
        }

        public static void sendRefresh()
        {
            MessagingCenter.Send(new MyDayFoodItem(), "RefreshMyDay");
        }
    }
}

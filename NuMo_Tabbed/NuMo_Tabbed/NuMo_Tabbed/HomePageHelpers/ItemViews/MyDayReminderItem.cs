using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace NuMo_Tabbed.ItemViews
{
    //Reminder item for MyDayPage, displays user saved picture to easily remind them of what they ate.
    class MyDayReminderItem : IMyDayViewItem
    {
        public String Date { get; set; }
        public Image ReminderImage { get; set; }
        public String imageString { get; set; }
        public int id { get; set; }
        public ICommand OnDeleteEvent { get; set; }

        public MyDayReminderItem()
        {

            OnDeleteEvent = new Command(OnDelete);
        }

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

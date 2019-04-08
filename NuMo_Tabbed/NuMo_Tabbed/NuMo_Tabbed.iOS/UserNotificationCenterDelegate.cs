using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using UserNotifications;

namespace NuMo_Tabbed.iOS
{
    //This class is intended to fix an issue with notifications that came with iOS 10
    public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate

    {

        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)

        {

            // Tell system to display the notification anyway or use

            // `None` to say we have handled the display locally.

            completionHandler(UNNotificationPresentationOptions.Alert);

        }

    }

}

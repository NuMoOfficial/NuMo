using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace NuMo_Tabbed.ViewModels
{
    public class UserProfileViewModel : BaseViewModel
    {
        public UserProfileViewModel()
        {
            Title = "User Profile";
        }

        public ICommand OpenWebCommand { get; }
    }
}
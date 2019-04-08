using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace NuMo_Tabbed.ViewModels
{
    public class CreateFoodViewModel : BaseViewModel
    {
        public CreateFoodViewModel()
        {
            Title = "Create Food";
        }

        public ICommand OpenWebCommand { get; }
    }
}
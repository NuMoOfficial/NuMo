using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace NuMo_Tabbed.ViewModels
{
    public class CreateRecipeViewModel : BaseViewModel
    {
        public CreateRecipeViewModel()
        {
            Title = "Create Recipe";
        }

        public ICommand OpenWebCommand { get; }
    }
}
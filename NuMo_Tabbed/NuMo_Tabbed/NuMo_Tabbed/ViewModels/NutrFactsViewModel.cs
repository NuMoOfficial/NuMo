using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace NuMo_Tabbed.ViewModels
{
    public class NutrFactsViewModel : BaseViewModel
    {
        public NutrFactsViewModel()
        {
            Title = "Nutrition Facts";
        }

        public ICommand OpenWebCommand { get; }
    }
}
using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace NuMo_Tabbed.ViewModels
{
    public class VisualizeViewModel : BaseViewModel
    {
        public VisualizeViewModel()
        {
            Title = "Visualize Nutrients";
        }

        public ICommand OpenWebCommand { get; }
    }
}
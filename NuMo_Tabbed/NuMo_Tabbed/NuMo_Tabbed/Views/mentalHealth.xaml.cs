using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NuMo_Tabbed.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class mentalHealth : ContentPage
    {
        public ICommand TapCommand => new Command<string>(async (url) => await Launcher.OpenAsync(url));
        public mentalHealth()
        {
            InitializeComponent();
            BindingContext = this;
        }
        public ICommand ClickCommand => new Command<string>(async (url) =>
        {
            if (await Xamarin.Essentials.Launcher.CanOpenAsync(url))
                await Xamarin.Essentials.Launcher.OpenAsync(new Uri(url));
            else
                await DisplayAlert("Alert", "The link was unable to open", "OK");
        });
    }
}
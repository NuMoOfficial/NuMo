using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NuMo_Tabbed.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SelectVisualizePage : ContentPage
	{
		public SelectVisualizePage ()
		{
			InitializeComponent ();
		}

        async void AddVisualize(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new VisualizePage());
        }

        async void AddDRI(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new DRIPage());
        }

        async void AddHydration(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new HydrationPage());
        }
    }
}
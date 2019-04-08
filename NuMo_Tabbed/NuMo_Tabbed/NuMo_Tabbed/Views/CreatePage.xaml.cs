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
	public partial class CreatePage : ContentPage
	{
		public CreatePage ()
		{
			InitializeComponent ();
		}

        async void AddCreateFood(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new CreateFoodPage());
        }

        async void AddCreateRecipe(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new CreateRecipePage());
        }
    }
}
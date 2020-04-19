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
            // Remember to commit database. User can no longer use Memento Pattern to undo 
            // previous transaction. By committing the database, database savepoints are
            // no longer able to rollback the database.
            DataAccessor db = DataAccessor.getDataAccessor();
            db.commit();

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
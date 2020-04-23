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
            
            var db = DataAccessor.getDataAccessor();

            /* check to see if the DRI values are blank. Blank values means no profile is setup
             * No DRI values will make the visualization crash and the app will crash too
             * to prevent app crashing a pop up box is displayed telling the user to create a profile
             */

            if (db.getDRIValue(db.GetDRINames()[0]) == "")
            {
                Empty_profile_error();
                return;  // return is used to keep the screen from navigating to the visualize page.
            }

            await Navigation.PushAsync(new VisualizePage());  // waits for the visualize page button to be pushed
        }
        async void AddDRI(object sender, EventArgs args)
        {
            var db = DataAccessor.getDataAccessor();

            /* Check if the age and gender from the profile are blank
             * if these fields are blank then a profile has not been created and DRI values cannot be calculated
             * No DRI values will make the visualization crash and the app will crash too
             * to prevent app crashing a pop up box is displayed telling the user to create a profile
             */
            if(db.getSettingsItem("age") == "" || db.getSettingsItem("gender") == "")
            {
                Empty_profile_error();
                return;  // return is used to keep the screen from navigating to the visualize page.
            }
            await Navigation.PushAsync(new DRIPage());
        }

        async void AddHydration(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new HydrationPage());
        }

        // This method creates a pop up boxing telling the user to create a profile
        private async void Empty_profile_error()
        {
            //pop up box stays on the screen until the ok button is selected
            await DisplayAlert("No Profile Exists", "Create a Profile to see Visualization", "OK");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Remember to commit database. User can no longer use Memento Pattern to undo 
            // previous transaction. By committing the database, database savepoints are
            // no longer able to rollback the database.
            DataAccessor db = DataAccessor.getDataAccessor();
            db.commit();
        }
    }
}
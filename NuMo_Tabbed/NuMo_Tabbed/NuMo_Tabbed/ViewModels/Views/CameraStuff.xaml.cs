using NuMo_Tabbed.ItemViews;
using Plugin.Media;
using Plugin.Media.Abstractions;

using System;
using System.Drawing; //for Image.FromFile
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.IO;


/* Camera page for uploading or taking pictures to save as reminders
 */

namespace NuMo_Tabbed.Views
{
    public partial class CameraStuff : ContentPage
    {
        Image saveImage = new Image();
        String pPath = "";
        DateTime date;
        DataAccessor db = DataAccessor.getDataAccessor();

        public CameraStuff()
        {
            //    new CameraStuff(DateTime.Today);
            //}
            //
            //public CameraStuff(DateTime dateS)
            //{
            // Initialize the toolbar with save button, date, and title
            this.date = DateTime.Today;
            ToolbarItem refresh = new ToolbarItem();
            refresh.Text = "Refresh";
            refresh.Clicked += refreshButtonClicked;
            ToolbarItems.Add(refresh);
            InitializeComponent();
            this.Title = "Upload Reminder Photo";
            loadImagesForDate(date.ToString("MM/dd/yyyy"));

        }
        private async void TakePhotoButton_OnClicked(object sender, EventArgs e, String picNum)
        {
            // If the take picure button is clicked, check if a camera is supported and use it if so
            await CrossMedia.Current.Initialize();
            //getPermissions();
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }
            var file = await CrossMedia.Current.TakePhotoAsync(
                new StoreCameraMediaOptions
                {
                    SaveToAlbum = true,
                    Directory = "NuMo"
                });

            if (file == null)
                return;

            //get file path for pic
            pPath = file.AlbumPath;

            // Display photo on page for user to see
            saveImage.Source = ImageSource.FromStream(() => file.GetStream());
            ImageButton src = (ImageButton)sender;
            src.Source = saveImage.Source;
            db.savePicReminder(date.ToString("MM/dd/yyyy"), pPath, picNum); //save to database

        }

        private async void PickPhotoButton_OnClicked(object sender, EventArgs e, String picNum)
        {
            // If the pick picure button is clicked, check if there is a photo album and access it if so
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Oops", "Pick photo is not supported!", "OK");
                return;
            }

            var file = await CrossMedia.Current.PickPhotoAsync();

            if (file == null)
                return;

            //file path
            pPath = file.Path;
            // Display photo on page for user to see
            saveImage.Source = ImageSource.FromStream(() => file.GetStream());
            ImageButton src = (ImageButton)sender;
            src.Source = saveImage.Source;
            db.savePicReminder(date.ToString("MM/dd/yyyy"), pPath, picNum); //save to database
        }

        //delete clicked
        private async void delete_OnClicked(object sender, EventArgs e, String picNum)
        {
            String filePath = db.getPicReminder(date.ToString("MM/dd/yyyy"), picNum);
            if (filePath.Equals(""))
            {
                await DisplayAlert("Oops", "No Image to Delete", "OK");
            }
            else
            {
                bool success = db.deletePicture(date.ToString("MM/dd/yyyy"), picNum);
                pPath = "";
                switch (picNum)
                {
                    case "1":
                        MainImage.Source = "Plus.png";
                        break;
                    case "2":
                        Image2.Source = "Plus.png";
                        break;
                    case "3":
                        Image3.Source = "Plus.png";
                        break;
                    case "4":
                        Image4.Source = "Plus.png";
                        break;
                    case "5":
                        Image5.Source = "Plus.png";
                        break;
                    case "6":
                        Image6.Source = "Plus.png";
                        break;
                    default:
                        break;
                }

            }


        }
        //Date changed
        void dateClicked(object sender, DateChangedEventArgs e)
        {

            date = e.NewDate;
            //check if new date is in db
            loadImagesForDate(date.ToString("MM/dd/yyyy"));

            this.OnAppearing();
        }
        private async void Image_OnClicked(object sender, EventArgs args)
        {
            String action = await DisplayActionSheet("Select Option for Image: ", "Cancel", "Delete", "Take Photo", "Pick Photo");
            String picNumber = "";
            ImageButton clicked = (ImageButton)sender;
            //find which image button was pressed
            if (clicked.ClassId == MainImage.ClassId) { picNumber = "1"; }
            else if (clicked.ClassId == Image2.ClassId) { picNumber = "2"; }
            else if (clicked.ClassId == Image3.ClassId) { picNumber = "3"; }
            else if (clicked.ClassId == Image4.ClassId) { picNumber = "4"; }
            else if (clicked.ClassId == Image5.ClassId) { picNumber = "5"; }
            else if (clicked.ClassId == Image6.ClassId) { picNumber = "6"; }

            //take selected action
            if (action.Equals("Delete"))
            {
                delete_OnClicked(sender, args, picNumber);
            }
            else if (action.Equals("Take Photo"))
            {
                TakePhotoButton_OnClicked(sender, args, picNumber);
            }
            else if (action.Equals("Pick Photo"))
            {
                PickPhotoButton_OnClicked(sender, args, picNumber);
            }
        }

        private async void refreshButtonClicked(object sender, EventArgs args)
        {
            // Upon clicking refresh button, reload all 6 images, useful for when images reset

            loadImagesForDate(date.ToString("MM/dd/yyyy"));
            await Navigation.PopAsync();
        }
        private void loadImagesForDate(String formattedDate)
        {
            String dbPicPath = "";
            //check if new date is in db
            for (int i = 1; i < 7; i++)
            {
                dbPicPath = db.getPicReminder(formattedDate, i + "");
                pPath = dbPicPath;
                //make sure file still exists (incase user deletes album)
                if (!File.Exists(dbPicPath))
                {
                    // Don't want to save memento because the file already doesn't exist
                    bool success = db.deletePicture(formattedDate, i + "", saveMemento: false);
                    dbPicPath = "";
                }
                switch (i)
                {
                    //changes image placement depending on counter
                    case 1:
                        if (!dbPicPath.Equals("")) { MainImage.Source = ImageSource.FromFile(dbPicPath); } //if there is an entry in db
                        else { MainImage.Source = "Plus.png"; } //no pic exists
                        break;
                    case 2:
                        if (!dbPicPath.Equals("")) { Image2.Source = ImageSource.FromFile(dbPicPath); }
                        else { Image2.Source = "Plus.png"; }
                        break;
                    case 3:
                        if (!dbPicPath.Equals("")) { Image3.Source = ImageSource.FromFile(dbPicPath); }
                        else { Image3.Source = "Plus.png"; }
                        break;
                    case 4:
                        if (!dbPicPath.Equals("")) { Image4.Source = ImageSource.FromFile(dbPicPath); }
                        else { Image4.Source = "Plus.png"; }
                        break;
                    case 5:
                        if (!dbPicPath.Equals("")) { Image5.Source = ImageSource.FromFile(dbPicPath); }
                        else { Image5.Source = "Plus.png"; }
                        break;
                    case 6:
                        if (!dbPicPath.Equals("")) { Image6.Source = ImageSource.FromFile(dbPicPath); }
                        else { Image6.Source = "Plus.png"; }
                        break;
                }
            }
        }
        /*
        //make sure permissions are on (NOT IN USE ATM)
        public async void getPermissions()
        {
            var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            if(cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionAsync(new[] { Permission.Camera, Peemission.Storage });
                cameraStatus = results[Permission.Camera];
                storageStatus = results[Permission.Storage];
            }
            if (cameraStatus == PermissionStatus.Granted && storageStatus == PermissionStatus.Granted)
            {
            }
            else
            {
                await DisplayAlert("Permissions Denied", "Unable to take photos.", "OK");
            }

        }
        */
    }
}

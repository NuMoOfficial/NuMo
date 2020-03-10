using NuMo_Tabbed.ItemViews;

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Tesseract;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services.Media;
using Xamarin.Forms.Xaml;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.IO;

namespace NuMo_Tabbed.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OCRview : ContentPage, INotifyPropertyChanged
    {
        private Button _takePictureButton;
        private Label _recognizedTextLabel;
        private Image _takenImage;

        private readonly ITesseractApi _tesseractApi;
        private readonly IDevice _device;

        Image saveImage = new Image();
        String pPath = "";
        DateTime date;
        DataAccessor db = DataAccessor.getDataAccessor();

        public OCRview()
        {
            InitializeComponent();
            this.Title = "Reciept Scanner";

            

            if (Device.RuntimePlatform == Device.iOS)
                Padding = new Thickness(0, 25, 0, 0);
        
            _tesseractApi = Resolver.Resolve<ITesseractApi>();
            _device = Resolver.Resolve<IDevice>();
            TesseractInit();

            BuildUi();

            WireEvents();
        }

        private void BuildUi()
        {
            _takePictureButton = new Button
            {
                Text = "New scan"
            };

            _recognizedTextLabel = new Label();


            _takenImage = new Image() { HeightRequest = 200 };

            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    Children =
                    {
                        _takePictureButton,
                        _takenImage,
                        _recognizedTextLabel
                    }
                }
            };

        }
        private void WireEvents()
        {
            _takePictureButton.Clicked += Image_OnClicked;
        }

        // The take a picture option has been selected so take a picture and run it through the OCR
        async void TakePictureButton_Clicked(object sender, EventArgs e)
        {
            
            //update the onscreen buttons to show the user progress is being made
            _takePictureButton.Text = "Working...";
            _takePictureButton.IsEnabled = false;

            //Take the actual photo of the receipt
            var photo = await TakePic(sender, e, "data");

            if (photo != null)
            {
                    //change the photo to bytes for processing
                    byte[] imageAsBytes = null;
                    using (var memoryStream = new MemoryStream())
                    {
                        photo.GetStream().CopyTo(memoryStream);
                        photo.Dispose();
                        imageAsBytes = memoryStream.ToArray();
                    }

                    // runs the OCR on the text
                    var tessResult = await _tesseractApi.SetImage(imageAsBytes);

                // Check to make sure the OCR came back
                if (tessResult)
                    {
                        //put the OCR on the screen
                        _takenImage.Source = ImageSource.FromStream(() => photo.GetStream());
                        _recognizedTextLabel.Text = _tesseractApi.Text;
                    }
            }
            _takePictureButton.Text = "New scan";
            _takePictureButton.IsEnabled = true;
        }

        //Initialize the OCR API so it is ready for a scan
        private async void TesseractInit()
        {
            await _tesseractApi.Init("eng");
        }

        // Accesses the camera to take a picture for the OCR
        private async Task<Plugin.Media.Abstractions.MediaFile> TakePic(object sender, EventArgs e, String picNum)
        {
           
            await CrossMedia.Current.Initialize();

            //getPermissions();
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return null;
            }
            var file = await CrossMedia.Current.TakePhotoAsync(
                new StoreCameraMediaOptions
                {
                    SaveToAlbum = true,
                    Directory = "NuMo"
                });

            if (file == null)
                return null;


            //get file path for pic
            pPath = file.AlbumPath;

            db.savePicReminder(date.ToString("MM/dd/yyyy"), pPath, picNum); //save to database

            return file;
        }

        // The scan button has been clicked
        // check to see if the user wants to take a picture or select and already taken picture
        private async void Image_OnClicked(object sender, EventArgs args)
        {
            String action = await DisplayActionSheet("Select Option for Image: ", "Cancel", "Take Photo", "Pick Photo");
            String picNumber = "data";

            //take selected action
            if (action.Equals("Take Photo"))
            {
                TakePictureButton_Clicked(sender, args);
            }
            else if (action.Equals("Pick Photo"))
            {
                PickPhotoButton_OnClicked(sender, args, picNumber);
            }
        }

        // User has chosen to pick a picture
        // access the photos on their phone, have them select a picture and the run the OCR on the picture
        private async void PickPhotoButton_OnClicked(object sender, EventArgs e, String picNum)
        {
            // If the pick picure button is clicked, check if there is a photo album and access it if so
            await CrossMedia.Current.Initialize();

            _takePictureButton.Text = "Working...";
            _takePictureButton.IsEnabled = false;

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Oops", "Pick photo is not supported!", "OK");
                return;
            }

            var file = await CrossMedia.Current.PickPhotoAsync();

            if (file == null)
                return;

            if (file != null)
            {

                byte[] imageAsBytes = null;
                using (var memoryStream = new MemoryStream())
                {
                    file.GetStream().CopyTo(memoryStream);
                    imageAsBytes = memoryStream.ToArray();
                }

                var tessResult = await _tesseractApi.SetImage(imageAsBytes);

                if (tessResult)
                {

                    _takenImage.Source = ImageSource.FromStream(() => file.GetStream());
                    _recognizedTextLabel.Text = _tesseractApi.Text;
                }
            }

        }

    }
}
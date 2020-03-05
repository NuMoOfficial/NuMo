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
        //private ProgressBar _progressBar;
        private Image _takenImage;
        //private bool _loaded;
        //private MediaFile media;
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
            _takePictureButton.Clicked += TakePictureButton_Clicked;
        }

        async void TakePictureButton_Clicked(object sender, EventArgs e)
        {
            
            _takePictureButton.Text = "Working...";
            _takePictureButton.IsEnabled = false;

            //if (!_tesseractApi.Initialized)
            //{
            //    await _tesseractApi.Init("eng");
            //}
            
            //System.Threading.Thread.Sleep(20000);
            // Image_OnClicked(sender, e);
            var photo = await TakePic(sender, e, "data");

            if (photo != null)
            {
               
                    byte[] imageAsBytes = null;
                    using (var memoryStream = new MemoryStream())
                    {
                        photo.GetStream().CopyTo(memoryStream);
                        photo.Dispose();
                        imageAsBytes = memoryStream.ToArray();
                    }

                    var tessResult = await _tesseractApi.SetImage(imageAsBytes);

                if (tessResult)
                    {

                        _takenImage.Source = ImageSource.FromStream(() => photo.GetStream());
                        _recognizedTextLabel.Text = _tesseractApi.Text;
                    }
            }
            _takePictureButton.Text = "New scan";
            _takePictureButton.IsEnabled = true;
        }
        private async void TesseractInit()
        {
            await _tesseractApi.Init("eng");
        }
        private async Task<Plugin.Media.Abstractions.MediaFile> TakePic(object sender, EventArgs e, String picNum)
        {
            //var picNum = "data";
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


            ////get file path for pic
            //pPath = file.AlbumPath;

            //// Display photo on page for user to see
            //saveImage.Source = ImageSource.FromStream(() => file.GetStream());
            //ImageButton src = (ImageButton)sender;
            //src.Source = saveImage.Source;
            //db.savePicReminder(date.ToString("MM/dd/yyyy"), pPath, picNum); //save to database

            return file;
        }
        //private async void Image_OnClicked(object sender, EventArgs args)
        //{
        //    String action = await DisplayActionSheet("Select Option for Image: ", "Cancel", "Take Photo", "Pick Photo");
        //    String picNumber = "data";
        //    //         ImageButton clicked = (ImageButton)sender;

        //    //take selected action
        //    if (action.Equals("Take Photo"))
        //    {
        //        TakePic(sender, args, picNumber);
        //    }
        //    else if (action.Equals("Pick Photo"))
        //    {
        //        PickPhotoButton_OnClicked(sender, args, picNumber);
        //    }
        //}

        //private async void PickPhotoButton_OnClicked(object sender, EventArgs e, String picNum)
        //{
        //    // If the pick picure button is clicked, check if there is a photo album and access it if so
        //    await CrossMedia.Current.Initialize();

        //    if (!_tesseractApi.Initialized)
        //        await _tesseractApi.Init("eng");

        //    if (!CrossMedia.Current.IsPickPhotoSupported)
        //    {
        //        await DisplayAlert("Oops", "Pick photo is not supported!", "OK");
        //        return;
        //    }

        //    var file = await CrossMedia.Current.PickPhotoAsync();

        //    if (file == null)
        //        return;

        //    byte[] imageAsBytes = null;
        //    using (var memoryStream = new MemoryStream())
        //    {
        //        file.GetStream().CopyTo(memoryStream);
        //        file.Dispose();
        //        imageAsBytes = memoryStream.ToArray();
        //    }


        //    var tessResult = await _tesseractApi.SetImage(imageAsBytes);

        //    if (tessResult)
        //    {

        //        _takenImage.Source = ImageSource.FromStream(() => file.GetStream());
        //        _recognizedTextLabel.Text = _tesseractApi.Text;
        //    }

        //    //file path
        //    pPath = file.Path;
        //    // Display photo on page for user to see
        //    saveImage.Source = ImageSource.FromStream(() => file.GetStream());
        //    ImageButton src = (ImageButton)sender;
        //    src.Source = saveImage.Source;
        //    db.savePicReminder(date.ToString("MM/dd/yyyy"), pPath, picNum); //save to database
        //}

        //private async Task<MediaFile> checkPic()
        //{
        //    var mediaStorageOptions = new CameraMediaStorageOptions
        //    {
        //        DefaultCamera = CameraDevice.Rear
        //    };
        //    var mediaFile = await _device.MediaPicker.TakePhotoAsync(mediaStorageOptions);

        //    return mediaFile;
        //}

    }
}
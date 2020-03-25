
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Web;


namespace NuMo_Tabbed.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OCRview : ContentPage, INotifyPropertyChanged
    {
        private Button _takePictureButton;
        private Label _recognizedTextLabel;
        private Image _takenImage;

        private string subscriptionKey = "07440da4c92f43518c5851ca0bfd9983";

        //private string endpoint = "https://numo-ocr.cognitiveservices.azure.com/";

        // the Batch Read method endpoint
        private string uriBase = "https://numo-ocr.cognitiveservices.azure.com/vision/v3.0-preview/read/analyze";

        Image saveImage = new Image();
        String pPath = "";
        DateTime date;
        DataAccessor db = DataAccessor.getDataAccessor();
        


        public OCRview()
        {
            InitializeComponent();
            this.Title = "Reciept Scanner";
            //subscriptionKey = "07440da4c92f43518c5851ca0bfd9983";
            //endpoint = "https://numo-ocr.cognitiveservices.azure.com/";
            //uriBase = endpoint + "vision/v3.0-preview/read/analyze";


            if (Device.RuntimePlatform == Device.iOS)
                Padding = new Thickness(0, 25, 0, 0);
        


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
            _takePictureButton.Text = ("Wait a moment for the results to appear.");
            _takePictureButton.IsEnabled = false;

            //Take the actual photo of the receipt
            var photo = await TakePic(sender, e, "data");

            if (photo != null)
            {
                string imageFilePath = photo.Path;
                    //change the photo to bytes for processing
                    byte[] imageAsBytes = null;
                    using (var memoryStream = new MemoryStream())
                    {
                        photo.GetStream().CopyTo(memoryStream);
                        photo.Dispose();
                        imageAsBytes = memoryStream.ToArray();
                    }

                // runs the OCR on the text
                if (File.Exists(imageFilePath))
                {
                    // Call the REST API method.
                    _takePictureButton.Text = "\nWait a moment for the results to appear.\n";
                    await ReadText(imageFilePath, "en");
                }
            }
            _takePictureButton.Text = "New scan";
            _takePictureButton.IsEnabled = true;
        }


        /// <summary>
        /// Gets the text from the specified image file by using
        /// the Computer Vision REST API.
        /// </summary>
        /// <param name="imageFilePath">The image file with text.</param>
        private async Task ReadText(string imageFilePath, string language)
        {
            try
            {
                HttpClient client = new HttpClient();

                // Request headers.
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", subscriptionKey);

                var builder = new UriBuilder(uriBase);
                builder.Port = -1;
                var query = HttpUtility.ParseQueryString(builder.Query);
                query["language"] = language;
                builder.Query = query.ToString();
                string url = builder.ToString();

                HttpResponseMessage response;

                // Two REST API methods are required to extract text.
                // One method to submit the image for processing, the other method
                // to retrieve the text found in the image.

                // operationLocation stores the URI of the second REST API method,
                // returned by the first REST API method.
                string operationLocation;

                // Reads the contents of the specified local image
                // into a byte array.
                byte[] byteData = GetImageAsByteArray(imageFilePath);

                // Adds the byte array as an octet stream to the request body.
                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    // This example uses the "application/octet-stream" content type.
                    // The other content types you can use are "application/json"
                    // and "multipart/form-data".
                    content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");

                    // The first REST API method, Batch Read, starts
                    // the async process to analyze the written text in the image.
                    response = await client.PostAsync(url, content);
                }

                // The response header for the Batch Read method contains the URI
                // of the second method, Read Operation Result, which
                // returns the results of the process in the response body.
                // The Batch Read operation does not return anything in the response body.
                if (response.IsSuccessStatusCode)
                    operationLocation =
                        response.Headers.GetValues("Operation-Location").FirstOrDefault();
                else
                {
                    // Display the JSON error data.
                    string errorString = await response.Content.ReadAsStringAsync();
                    _recognizedTextLabel.Text = errorString;
                    Console.WriteLine("\n\nResponse:\n{0}\n",
                        JToken.Parse(errorString).ToString());
                    return;
                }

                // If the first REST API method completes successfully, the second 
                // REST API method retrieves the text written in the image.
                //
                // Note: The response may not be immediately available. Text
                // recognition is an asynchronous operation that can take a variable
                // amount of time depending on the length of the text.
                // You may need to wait or retry this operation.
                //
                // This example checks once per second for ten seconds.
                string contentString;
                int i = 0;
                do
                {
                    System.Threading.Thread.Sleep(1000);
                    response = await client.GetAsync(operationLocation);
                    contentString = await response.Content.ReadAsStringAsync();
                    ++i;
                }
                while (i < 60 && contentString.IndexOf("\"status\":\"succeeded\"") == -1);

                if (i == 60 && contentString.IndexOf("\"status\":\"succeeded\"") == -1)
                {
                    _recognizedTextLabel.Text = ("\nTimeout error.\n");
                    Console.WriteLine("\nTimeout error.\n");
                    return;
                }

                // Display the JSON response.
                _recognizedTextLabel.Text = JToken.Parse(contentString).ToString();
                Console.WriteLine("\nResponse:\n\n{0}\n",
                    JToken.Parse(contentString).ToString());
            }
            catch (Exception e)
            {
                _recognizedTextLabel.Text = ("\n" + e.Message);
                Console.WriteLine("\n" + e.Message);
            }
        }

        /// <summary>
        /// Returns the contents of the specified file as a byte array.
        /// </summary>
        /// <param name="imageFilePath">The image file to read.</param>
        /// <returns>The byte array of the image data.</returns>
        private byte[] GetImageAsByteArray(string imageFilePath)
        {
            // Open a read-only file stream for the specified file.
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                // Read the file's contents into a byte array.
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
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

            _takePictureButton.Text = ("Wait a moment for the results to appear.");
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
                string imageFilePath = file.Path;
                if (File.Exists(imageFilePath))
                {
                    // Call the REST API method.
                    _takePictureButton.Text = ("\nWait a moment for the results to appear.\n");
                    await ReadText(imageFilePath, "en");
                }


            }

        }

    }
}
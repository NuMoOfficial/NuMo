
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
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NuMo_Tabbed.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OCRview : ContentPage, INotifyPropertyChanged
    {
        // Changes the label on the screen whenever the OcrText string is set
        public string OcrText
        {
            get { return ocr; }
            set
            {
                ocr = value;
                OnPropertyChanged(nameof(OcrText));
            }

        }

            // Changes the photo on the screen to whatever photo is taken or selected
            public string Photo
        {
            get { return PhotoPath; }
            set
            {
                PhotoPath = value;
                OnPropertyChanged(nameof(Photo));
            }

        }

        string PhotoPath = string.Empty;
        
        string ocr = string.Empty;

        private string subscriptionKey = "07440da4c92f43518c5851ca0bfd9983";

        // the Batch Read method endpoint for the OCR
        private string uriBase = "https://numo-ocr.cognitiveservices.azure.com/vision/v3.0-preview/read/analyze";

        Image saveImage = new Image();
        String pPath = "";
        DateTime date;
        DataAccessor db = DataAccessor.getDataAccessor();
        string dataToPass = "";

        public OCRview()
        {
            InitializeComponent();

            this.Title = "Reciept Scanner";
            BindingContext = this;
            OcrText = "Recognized Text";
            Photo = "camera.png";

            if (Device.RuntimePlatform == Device.iOS)
                Padding = new Thickness(0, 25, 0, 0);
        
        }

        // The take a picture option has been selected so take a picture and run it through the OCR
        async void TakePictureButton_Clicked(object sender, EventArgs e)
        {
            // If the take picure button is clicked, check if a camera is supported and use it if so
            await CrossMedia.Current.Initialize();
            //getPermissions();
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            //Take the actual photo of the receipt
            var photo = await TakePic(sender, e, "data");

            if (photo != null)
            {
                string imageFilePath = photo.Path;
                Photo = imageFilePath;
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
                    OcrText = "\nWait a moment for the results to appear.\n";
                    await ReadText(imageFilePath, "en");
                }
            }
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
                    OcrText = errorString;
                    //Console.WriteLine("\n\nResponse:\n{0}\n",
                    //    JToken.Parse(errorString).ToString());
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
                    OcrText = ("\nTimeout error.\n");
                    //Console.WriteLine("\nTimeout error.\n");
                    return;
                }

                // Display the JSON response.
                // Parse the JSON returned to find the lines of text only
                JObject rss = JObject.Parse(contentString);
                JArray results = (JArray)rss["analyzeResult"]["readResults"][0]["lines"];
                string TempResults = "Start Results:\n";
                // Go through each line of text and add it to the temporary string
                for (int r = 0; r < results.Count-1; r++)
                {
                    TempResults += (results[r]["text"]).ToString() + "\n";
                }

                // Update the screen with the results of the OCR
                OcrText = TempResults;
                parseText(TempResults);
                
            }
            catch (Exception e)
            {
                OcrText = ("\n" + e.Message);
                //Console.WriteLine("\n" + e.Message);
            }
        }

        void parseText(String results)
        {
            results = cleanString(results);
            string[] words = results.Split(' ');
            var picker = new Picker { Title = "Select Text", IsVisible = false, IsEnabled = false, HorizontalOptions = LayoutOptions.Start, TitleColor = Color.Red };
            var button1 = new Button { Text = "Select Restaurant Name", Style = (Style)Application.Current.Resources["buttonStyle"], Margin = new Thickness(0, 10, 0, 0) };
            var label = new Label { Text = "Search: ", Margin = new Thickness(0, 20, 0, 20), FontSize = 25, TextColor = Color.Black};
            var button2 = new Button { Text = "Search" , Style = (Style) Application.Current.Resources["buttonStyle"], VerticalOptions = LayoutOptions.End}; //, Style = (Style)Resources["buttonStyle"] 
            var textList = new List<string>();
            button1.Clicked += delegate
            {
                picker.IsEnabled = true;
                picker.Focus();
            };
            button2.Clicked += openSearch;

            foreach (string word in words)
            {
                string temp = word;
                //temp = removeNumbers(temp);
                if (temp.Length >= 3 && temp != "Start" && temp != "Results")
                {
                    textList.Add(temp);
                }
            }
            picker.ItemsSource = textList;
            picker.SelectedIndexChanged += (sender, args) =>
            {
                if (picker.SelectedIndex == -1)
                {
                    picker.IsVisible = false;
                    picker.IsEnabled = false;
                }
                else
                {
                    string option = picker.Items[picker.SelectedIndex];
                    //picker.Title = "Select more text";
                    //need a line of code here that makes the picker display title instead of item
                    label.Text += (option + " ");
                    dataToPass += (option + " ");
                    picker.IsEnabled = false;
                    button1.Text = "Select Food Item";

                }
            };
            StackLayout stackLayout = new StackLayout();
            stackLayout.Children.Add(picker);
            stackLayout.Children.Add(button1);
            stackLayout.Children.Add(label);
            stackLayout.Children.Add(button2);

            Content = stackLayout;
            //Content = picker; //this makes only the picker appear on content page

        }

        public string cleanString(string word)
        {
            return Regex.Replace(word, "[^A-Za-z]", " ");
        }

        async void openSearch(object sender, EventArgs args)
        {
            //date = datePicker.Date;
            date = DateTime.Now;
            await Navigation.PushAsync(new AddItemToFoodHistory(date, dataToPass));
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
           
            var file = await CrossMedia.Current.TakePhotoAsync(
                new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Medium, // reduce the file size of the pic when taken so it is small enough for OCR
                    SaveToAlbum = true,
                    Directory = "NuMo"
                });

            if (file == null)
                return null;


            //get file path for pic
            pPath = file.AlbumPath;
            Photo = pPath;
            this.date = DateTime.Today;

            db.savePicReminder(date.ToString("MM/dd/yyyy"), pPath, picNum); //save to database

            return file;
        }

        // The scan button has been clicked
        // check to see if the user wants to take a picture or select and already taken picture
        private async void Image_OnClicked(object sender, EventArgs args)
        {
            String action = await DisplayActionSheet("Select Option for Image: ", "Cancel", null, "Take Photo", "Pick Photo");
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

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Oops", "Pick photo is not supported!", "OK");
                return;
            }

            var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                CompressionQuality = 92  // reduce the file size to 92% of the original size. Make pic smaller for OCR
            }
                );

            if (file == null)
                return;

            if (file != null)
            {
                string imageFilePath = file.Path;
                Photo = imageFilePath;
                if (File.Exists(imageFilePath))
                {
                    // Call the REST API method.
                    OcrText = ("\nWait a moment for the results to appear.\n");
                    await ReadText(imageFilePath, "en");
                }

            }

        }

    }
}

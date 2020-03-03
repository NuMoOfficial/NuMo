using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Plugin.CurrentActivity; //for camera

// The following imports are for OCR Camera
using Android.Support.V7.App;
using Android.Gms.Vision;
using Android.Gms.Vision.Texts;
using Android.Util;
using Android.Graphics;
using Android.Support.V4.App;
using Android;
using static Android.Gms.Vision.Detector;
using System.Text;

namespace NuMo_Tabbed.Droid
{
    [Activity(Label = "NuMo_Tabbed", Icon = "@drawable/NuMoLogo", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    // OCR Camera [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.Light.NoActionBar", MainLauncher = true)]

    // May need AppCompatActivity instead of Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, ISurfaceHolderCallback, IProcessor
    {
        // The following instance variables are for OCR Camera
        private SurfaceView cameraView;
        private TextView textView;
        private CameraSource cameraSource;
        private const int RequestCameraPermissionID = 1001;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            // These two lines of code from OCR Camera
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);


            CrossCurrentActivity.Current.Init(this, savedInstanceState); //needed for camera activity
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init(); //needed for OxyPlot

            // The following is for OCR Camera
            cameraView = FindViewById<SurfaceView>(Resource.Id.surfaceView1);
            textView = FindViewById<TextView>(Resource.Id.textView1);

            TextRecognizer textRecognizer = new TextRecognizer.Builder(ApplicationContext).Build();
            if (!textRecognizer.IsOperational)
            {
                Log.Error("Main Activity", "Detector dependencies are not yet available");
            }
            else
            {
                cameraSource = new CameraSource.Builder(ApplicationContext, textRecognizer)
                    .SetFacing(CameraFacing.Back)
                    .SetRequestedPreviewSize(1280, 1024)
                    .SetRequestedFps(2.0f)
                    .SetAutoFocusEnabled(true)
                    .Build();

                cameraView.Holder.AddCallback(this);
                textRecognizer.SetProcessor(this);
            }

            LoadApplication(new App());
        }


        // All the following methods are for OCR Camera
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RequestCameraPermissionID:
                    {
                        if (grantResults[0] == Android.Content.PM.Permission.Granted)
                        {
                            cameraSource.Start(cameraView.Holder);
                        }
                    }
                    break;
            }
        }


        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {

        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            if (ActivityCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted)
            {
                // Request permission
                ActivityCompat.RequestPermissions(this, new string[]
                {
                    Android.Manifest.Permission.Camera
                }, RequestCameraPermissionID);
                return;
            }
            cameraSource.Start(cameraView.Holder);
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            cameraSource.Stop();
        }

        public void ReceiveDetections(Detections detections)
        {
            SparseArray items = detections.DetectedItems;
            if (items.Size() != 0)
            {
                textView.Post(() =>
                {
                    StringBuilder strBuilder = new StringBuilder();
                    for (int i = 0; i < items.Size(); i++)
                    {
                        strBuilder.Append(((TextBlock)items.ValueAt(i)).Value);
                        strBuilder.Append("\n");
                    }
                    textView.Text = strBuilder.ToString();
                });
            }
        }

        public void Release()
        {

        }
    }
}
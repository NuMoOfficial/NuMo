using System;

using Android.Content;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using TinyIoC;
using Tesseract;
using Tesseract.Droid;
using XLabs.Ioc;
using XLabs.Ioc.TinyIOC;
using XLabs.Platform.Device;

using Plugin.CurrentActivity; //for camera

namespace NuMo_Tabbed.Droid
{
    [Activity(Label = "NuMo_Tabbed", Icon = "@drawable/NuMoLogo", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            CrossCurrentActivity.Current.Init(this, savedInstanceState); //needed for camera activity
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init(); //needed for OxyPlot
            var container = TinyIoCContainer.Current;
            container.Register<IDevice>(AndroidDevice.CurrentDevice);
            container.Register<ITesseractApi>((cont, parameters) =>
            {
                return new TesseractApi(ApplicationContext, Tesseract.Droid.AssetsDeployment.OncePerInitialization);
            });
            Resolver.SetResolver(new TinyResolver(container));

            LoadApplication(new App());
        }
    }
}
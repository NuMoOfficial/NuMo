using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NuMo_Tabbed.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : TabbedPage
    {
        public MainPage()
        {
            InitializeComponent();
            var pages = Children.GetEnumerator();
            pages.MoveNext();
            pages.MoveNext();
            pages.MoveNext();
            pages.MoveNext();
            pages.MoveNext();


            CurrentPage = pages.Current;
        }
    }
}
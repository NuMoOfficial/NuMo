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
            pages.MoveNext(); //when first starting app start at the first tab on the left and move to the next one to the right of it
            pages.MoveNext();
            pages.MoveNext();
            pages.MoveNext();
            pages.MoveNext();
            pages.MoveNext(); // ends on the last tab on the right

            CurrentPage = pages.Current;
        }
    }
}
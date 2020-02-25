using NuMo_Tabbed.ItemViews;
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
    public partial class OCRview : ContentPage
    {
        public OCRview()
        {
            InitializeComponent();
            this.Title = "Reciept Scanner";
        }
    }
}
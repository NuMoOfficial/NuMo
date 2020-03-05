using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace NuMo_Tabbed.ViewModels
{
    public class OCRViewModel : BaseViewModel
    {
        public OCRViewModel()
        {
            Title = "OCR Receipt Scanning";
        }

        public ICommand OpenWebCommand { get; }
    }
}
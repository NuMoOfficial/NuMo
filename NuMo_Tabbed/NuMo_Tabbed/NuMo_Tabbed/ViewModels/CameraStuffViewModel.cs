using System;
using System.Collections.Generic;
using System.Windows.Input;

using Xamarin.Forms;

namespace NuMo_Tabbed.ViewModels
{
    public class CameraStuffViewModel : BaseViewModel
    {
        List<NumoNameSearch> searchResults { get; set; }

        public CameraStuffViewModel()
        {
            Title = "Camera Page";
        }

        public ICommand OpenWebCommand { get; }
    }
}

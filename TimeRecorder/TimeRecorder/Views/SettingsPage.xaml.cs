using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeRecorder.Modules;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TimeRecorder.ViewModels;

namespace TimeRecorder.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            BindingContext = Startup.ServiceProvider.GetService<SettingsPageViewModel>();
        }

        
    }
}
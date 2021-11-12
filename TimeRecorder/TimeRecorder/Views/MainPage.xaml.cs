using System;
using System.Collections.Generic;
using System.ComponentModel;
using TimeRecorder.ViewModels;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TimeRecorder.Modules;

namespace TimeRecorder.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = Startup.ServiceProvider.GetService<MainPageViewModel>();
        }
    }
}

using System;
using System.Threading.Tasks;
using TimeRecorder.Modules;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TimeRecorder
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Startup.Init();
            MainPage = new TimeRecorder.Views.MainPage(); 
            GetPermissions();
        }
        async void GetPermissions()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
            if (status == PermissionStatus.Unknown)
            {
                status = await Permissions.RequestAsync<Permissions.Microphone>();
            }

        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
           
        }

        protected override void OnResume()
        {
        }
    }
}

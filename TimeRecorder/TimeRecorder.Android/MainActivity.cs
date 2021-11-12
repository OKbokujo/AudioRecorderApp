using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using AndroidX.Core.Content;
using Android;
using AndroidX.Core.App;
using MediaManager;
using Android.Views;
using Xamarin.Forms;
using TimeRecorder.ViewModels;

namespace TimeRecorder.Droid
{
    [Activity(Label = "TimeRecorder", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
           
            Rg.Plugins.Popup.Popup.Init(this);
            CrossMediaManager.Current.Init(this);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.RecordAudio) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.RecordAudio }, 1);
            }
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.ReadExternalStorage }, 1);
            }
            LoadApplication(new App());
            
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.VolumeDown)
            {
                MessagingCenter.Send(App.Current as App, "1");
                Window.AddFlags(WindowManagerFlags.Fullscreen);
                Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
                return true;
            }
            else 
            {
                MessagingCenter.Send(App.Current as App, "2");
                Window.AddFlags(WindowManagerFlags.ForceNotFullscreen);
                Window.ClearFlags(WindowManagerFlags.Fullscreen);
                
            }
            return false;
        }
     

    }
}
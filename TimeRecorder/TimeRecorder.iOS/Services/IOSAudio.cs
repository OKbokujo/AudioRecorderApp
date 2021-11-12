using AVFoundation;
using Foundation;
using Plugin.AudioRecorder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeRecorder.Interfaces;
using TimeRecorder.iOS.Services;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(IOSAudio))]
namespace TimeRecorder.iOS.Services
{
    public class IOSAudio : IAudio
    {
        public void Initialize()
        {
            AudioRecorderService.RequestAVAudioSessionCategory(AVAudioSessionCategory.Record);
        }


    }
}
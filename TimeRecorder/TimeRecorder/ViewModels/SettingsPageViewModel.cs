using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeRecorder.Models;
using TimeRecorder.Services;
using Xamarin.Forms;

namespace TimeRecorder.ViewModels
{
    public class SettingsPageViewModel : TabbedPage, INotifyPropertyChanged
    {
        readonly LocalCache LocalCache;
       
        private bool _speechToText;
        public bool SpeechToText
        {
            get => _speechToText;
            set
            {
                _speechToText = value;
                OnPropertyChanged();
                SetAll();
            }
        }
        private bool _blackWhileRecording;
        public bool BlackWhileRecording
        {
            get => _blackWhileRecording;
            set
            {
                _blackWhileRecording = value;
                OnPropertyChanged();
                SetAll();
            }
        }
        int? _bufferSeconds;
        public int? BufferSeconds
        {
            get => _bufferSeconds;
            set
            {
                _bufferSeconds = value;
                OnPropertyChanged();
            }
        }
        bool LoadFinished;
        public ICommand  PromptNumericEntryCM => new Command(async () => await PromptNumericInput());
        private bool _saveAudio;
        public bool SaveAudio
        {
            get => _saveAudio;
            set
            {
                _saveAudio = value;
                OnPropertyChanged();
                SetAll();
            }
        }
        private Settings Settings;
        public SettingsPageViewModel(LocalCache localCache, ManageData manageData)
        {
            LocalCache = localCache;
            Init();
        }
        async void Init()
        {
            Settings = await LocalCache.GetSettings();
            BufferSeconds = Settings.RecordStartDelay != null ? Settings.RecordStartDelay : 0;
            BlackWhileRecording = Settings.BlackScreenWhileRecording;
            SaveAudio = Settings.SaveWholeAudioSession;
            SpeechToText = Settings.SpeechToText;
            LoadFinished = true;
      
        }

        async void SetAll()
        {
            if (LoadFinished)
            {
                Settings.BlackScreenWhileRecording = BlackWhileRecording;
                Settings.RecordStartDelay = BufferSeconds;
                Settings.SaveWholeAudioSession = SaveAudio;
                Settings.SpeechToText = SpeechToText;
                await SaveSettings();
                
            }
        }
        [Obsolete]
        async Task PromptNumericInput()
        {
            try
            {
                BufferSeconds = Int32.Parse(await Application.Current.MainPage.DisplayPromptAsync("", "Set buffer length in seconds", "OK", "Cancel", "", -1, Keyboard.Numeric));
            }
            catch
            { 
            }
             SetAll();
        }
        async Task SaveSettings()
        {
            await LocalCache.AddUpdateSetting(Settings);
        }
    }
}

using MediaManager;
using MediaManager.Library;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Player;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using TimeRecorder.Interfaces;
using TimeRecorder.Models;
using TimeRecorder.Services;
using TimeRecorder.Views;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace TimeRecorder.ViewModels
{
    public class MainPageViewModel : BaseMainPageViewModel
    {
        string _audioTime;
        public string AudioTime
        {
            get => _audioTime;
            set
            {
                _audioTime = value;
                OnPropertyChanged();
            }
        }
        int _bufferSeconds;
        public int BufferSeconds
        {
            get => _bufferSeconds;
            set
            {
                _bufferSeconds = value;
                OnPropertyChanged();
            }
        }
        Dictionary<int?, HighlightedAudioClip> CurrentHighlightedAudioClips;
        public ICommand CreateNotesCM => new Command<object>(async (x) => await NavigateToNotesPage(x));
        Session CurrentSession;
        public ICommand DeleteAudioFile => new Command<int?>(async (x) => await DeleteHighlightedAudioPressed(x));
        Slider Element;
        public ICommand InvertDisposeStream => new Command(() => InvertDisposeBool());
        Dictionary<int?, HighlightedAudioClip> HighlightedAudioClips;

        int? LastHighlightedAudioClipID;
        int? LastSessionID;
        ManageData ManageData;
        public ICommand OpenSettings => new Command(async () => await NavigateToSettingsPage());
        public ICommand PauseStartAudioPlayerCM => new Command(async () => await PauseStartAudioPlayer());
        public ICommand PlayAudio => new Command<int?>(async (x) => await StartAudioPlayBack(x));
        Brush _recordButtonColor;
        public Brush RecordButtonColor
        {
            get => _recordButtonColor;
            set
            {
                _recordButtonColor = value;
                OnPropertyChanged();
            }
        }
        string _recording;

        [Obsolete]
        public string Recording
        {
            get => _recording;
            set
            {
                _recording = value;
                OnPropertyChanged();
            }
        }
   
        Stopwatch StopWatch;
        Dictionary<int?, List<HighlightedAudioClip>> _sessionHighlightedAudioClips;
        public Dictionary<int?, List<HighlightedAudioClip>> SessionHighlightedAudioClips
        {
            get => _sessionHighlightedAudioClips;
            set
            {
                _sessionHighlightedAudioClips = value;
                OnPropertyChanged();

            }
        }
        Dictionary<int?, Session> Sessions;
        Settings _settings;
        public Settings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                OnPropertyChanged();
            }
        }
        bool _sliderVisible;
        public bool SliderVisible
        {
            get => _sliderVisible;
            set
            {
                _sliderVisible = value;
                OnPropertyChanged();
            }
        }
        public ICommand StartMenuItemCM => new Command(async (x) => await StartMenuItem(x));
        public ICommand StartRecorder => new Command(async () => await StartRecording());
        public ICommand StopRecorder => new Command(async () => await StopRecording());
        ObservableCollection<Session> _viewSessions;

        public ObservableCollection<Session> ViewSessions
        {
            get => _viewSessions;
            set
            {
                _viewSessions = value;
                OnPropertyChanged();
            }
        }
        public MainPageViewModel(LocalCache localCache, ManageData manageData)
        {
            LocalCache = localCache;
            ManageData = manageData;
            Init();
            MessagingCenter.Subscribe<App>(App.Current as App, "1",(snd) =>
            {
                InvertDisposeBool();
            });
            MessagingCenter.Subscribe<App>(App.Current as App, "2", (snd) =>
            {
                StartRecording();
            });


        }
        async  Task  Init()
        {
            RecordButtonColor =Brush.White;
            
            ManageData = new ManageData();
            Sessions = await LocalCache.GetSessions();
            Recording = "";
            IsRecording = false;
            SliderVisible = false;
            
            if (Sessions == null)
                Sessions = new Dictionary<int?, Session>();
            HighlightedAudioClips = await LocalCache.GetHighlightedAudioClips();
            if (HighlightedAudioClips == null)
                HighlightedAudioClips = new Dictionary<int?, HighlightedAudioClip>();

            ManageData.SetSessions(Sessions);
            ManageData.SetHighlightedAudioClips(HighlightedAudioClips);

            ViewSessions = new ObservableCollection<Session>();
            SessionHighlightedAudioClips = new Dictionary<int?, List<HighlightedAudioClip>>();
            ManageData.MakeHighlightedAudioClipLists(Sessions);
            AddSessionsToObservable(Sessions);
            await GetSettings();
            Element = Application.Current.MainPage.FindByName<Slider>("Slider");
            Element.ValueChanged += Element_ValueChanged;
           
        }

        async Task LoadSettings()
        {
            BufferLength = (int)Settings.RecordStartDelay;
            BufferSeconds = BufferLength;
            //Must implement other settings

        }

        private void Element_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if(!CrossMediaManager.Current.IsPlaying())
            {
                CrossMediaManager.Current.SeekTo(new TimeSpan( (long)Element.Value));
            }
        }

        void AddSessionsToObservable(Dictionary<int?, Session> sessions)
        {
            foreach(var x in sessions)
            {
                ViewSessions.Add(x.Value);
            }
        }
        //The below method is a temporary solution until I come up with a better way to store the last Id. This is a waste of cpu power. Perhaps store it in the settings model's data.
        void GetIDs()
        {
            
            int? id = 0;
            foreach(var x in Sessions)
            {
                if (x.Key > id)
                    id = x.Key;
            }
            LastSessionID = id;
            id = 0;

            foreach(var x in HighlightedAudioClips)
            {
                if(x.Key > id)
                    id = x.Key;
            }
            LastHighlightedAudioClipID = id;
        }
        async Task NavigateToSettingsPage()
        {
            SettingsPage settingsPage = new SettingsPage();
            settingsPage.Disappearing += Page_Disappearing;
             
            await Navigation.PushPopupAsync(settingsPage);

        }

        private void Page_Disappearing(object sender, EventArgs e)
        {
            GetSettings();
        }
        async Task GetSettings()
        {
            Settings = await LocalCache.GetSettings();
            if (Settings == null)
                Settings = new Settings();
            await LoadSettings();
        }
        protected override void ReadyIt(object sender, RunWorkerCompletedEventArgs e)
        {
           
            if (Sessions.Count > 0 || streams.Count > 0)
            {
                GetIDs();
                LastSessionID++;
                CurrentSession = new Session
                {
                    CreationDate = DateTime.Now,
                    Id = LastSessionID

                };
                if (streams.Count > 0)
                    StartSessionClipProcess();
            }
    }
        async Task StartSessionClipProcess()
        {
            await CreateClips();
            ManageData.UpdateSession(CurrentSession, CurrentHighlightedAudioClips);
            await SaveSession(CurrentSession);
            await SaveCurrentHighlightedClips();
            

            Sessions = ManageData.GetSessions();
            HighlightedAudioClips = ManageData.GetHighlightedAudioClips();

            await ManageCurrentSession();
        }
        async Task ManageCurrentSession()
        { 
            Dictionary<int?, Session> currentSession = new Dictionary<int?, Session>();
            currentSession.Add(CurrentSession.Id, CurrentSession);
            ManageData.MakeHighlightedAudioClipLists(currentSession);
            AddSessionsToObservable(currentSession);
        }
        async Task StartMenuItem(object element)
        {
            ItemMenuPage itemMenuPage = new ItemMenuPage(element);
            itemMenuPage.Disappearing += Page_Disappearing;

            await Navigation.PushPopupAsync(itemMenuPage);
            await MenuListeners();
        }
        async Task MenuListeners()
        {
            MessagingCenter.Subscribe<ItemMenuViewModel, string>(this, "1", async (sender, arg) =>
            {
                var message = JsonConvert.DeserializeObject<MessengerItem>(arg);
                if (message.ItemType == "DeleteSession")
                {
                    await StartDeleteSession(message.Session.Id);
                }
                else if (message.ItemType == "DeleteHighlightedAudioClip")
                {
                    await DeleteHighlightedAudioPressed(message.HighlightedAudioClip.Id);
                }
                else
                {
                    int id = Int32.Parse(message.ItemType);
                   
                    await  UpdateDataAndView(id, message);
                }
                MessagingCenter.Unsubscribe<ItemMenuViewModel, string>(this, "1");
            });
        }
        async Task UpdateDataAndView(int id, MessengerItem message)
        {
            
            if (message.Session != null)
            {
                Sessions[id] = await LocalCache.GetSession(id);
            }
            else
            {
                HighlightedAudioClips = await LocalCache.GetHighlightedAudioClips();
               
                ManageData.SetHighlightedAudioClips(HighlightedAudioClips);
            }

           ManageData.MakeHighlightedAudioClipLists(Sessions);
           await UpdateViewSessions(id);
        }
        async Task NavigateToNotesPage(object element)
        {
            NotesPage notesPage = new NotesPage(element);
            notesPage.Disappearing += Page_Disappearing;

            await Navigation.PushPopupAsync(notesPage);

        }
        async Task SaveCurrentHighlightedClips()
        {
            foreach (var x in CurrentHighlightedAudioClips)
            {
                await SaveHighlightedAudioClip(x.Value);
            }
        }
        async Task CreateClips()
        {
            AudioFileManagement audioFileManagement = new AudioFileManagement(streams, CurrentSession.Id);
            await audioFileManagement.ConvertAndSave(LastHighlightedAudioClipID);
            CurrentHighlightedAudioClips = audioFileManagement.GetHighlightedAudioClips();
            streams.Clear();
        }
        void InvertDisposeBool()
        {
            Mark = true;
            StartStopRecordingAnimation();
            //if(Timer.Enabled)
            //{
            //    disposeStream = false;
            //    Mark = true;
            //    Timer.Stop();
            //    Timer.Enabled = false;
            //}
            //else
            //{
            //    disposeStream = true;
            //    Timer.Enabled = true;
            //    Timer.Start();
            //}
        
        }
        void StartStopRecordingAnimation()
        {
            if(Recording == "" && recorder.IsRecording)
            {
                StopWatch = new Stopwatch();
                StopWatch.Start();
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    Recording = StopWatch.IsRunning ?((int)StopWatch.Elapsed.TotalSeconds).ToString() : "";
                    return Recording == "" ? false : true;
                });
            }
            else if (Recording !=  "")
            {
                StopWatch.Stop();
               
            }

        }

        async Task SaveSession(Session session)
        {
            await LocalCache.AddUpdateSession(session);
        }
        async Task SaveHighlightedAudioClip(HighlightedAudioClip highlightedAudioClip)
        {
            await LocalCache.AddUpdateHighlightedAudioClip(highlightedAudioClip);
        }
        async Task PauseStartAudioPlayer()
        {
            if (CrossMediaManager.Current.IsPlaying())
                await CrossMediaManager.Current.Pause();
            else if (CrossMediaManager.Current.IsPrepared())
                await CrossMediaManager.Current.Play();
        }
        async Task StartRecording()
        {
            IsRecording = !IsRecording;
            if (!IsRecording)
            {
                await StopRecording();
                RecordButtonColor = Brush.White;
            }
            else
            {
                await StartRecord();
                RecordButtonColor = Brush.Red;
            }
        }
        async Task StopRecording()
        {
           await  recorder.StopRecording();
           //Timer.Dispose();
        }
        async Task StartAudioPlayBack(int? id)
        {
            try
            {
                SliderVisible = true;
              

                CrossMediaManager.Current.StateChanged += Current_OnStateChanged;
                CrossMediaManager.Current.PositionChanged += Current_PositionChanged;
                CrossMediaManager.Current.MediaItemChanged += Current_MediaItemChanged;
                var currentMediaItem = await CrossMediaManager.Current.Play(System.IO.Path.Combine(FileSystem.AppDataDirectory, "highlightedAudioClips/") + $"{id}.wav");
                SetupCurrentMediaDetails(currentMediaItem);
                SetupCurrentMediaPositionData(CrossMediaManager.Current.Position);
                SetupCurrentMediaPlayerState(CrossMediaManager.Current.State);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        async Task DeleteHighlightedAudioPressed(int? id)
        {
            var canExecute = await DeleteSnackBarAsync("About to delete the audio clip", "Cancel", null);
            if (canExecute)
            {
                int? parentId = HighlightedAudioClips[id].ParentID;
                UpdateViewSessions(parentId);
                await StartDeleteAudioFile(id);
                await ManageData.DeleteHighlightedAudioClip(id, HighlightedAudioClips[id].ParentID);
                await LocalCache.AddUpdateSession(Sessions[parentId]);
            }
        }
        private void SetupCurrentMediaDetails(IMediaItem currentMediaItem)
        {
            // Set up Media item details in UI
            var displayDetails = string.Empty;
            if (!string.IsNullOrEmpty(currentMediaItem.DisplayTitle))
                displayDetails = currentMediaItem.DisplayTitle;

            if (!string.IsNullOrEmpty(currentMediaItem.Artist))
                displayDetails = $"{displayDetails} - {currentMediaItem.Artist}";

           // LabelMediaDetails.Text = displayDetails.ToUpper();
        }
        private void SetupCurrentMediaPlayerState(MediaPlayerState currentPlayerState)
        {
            // LabelPlayerStatus.Text = $"{currentPlayerState.ToString().ToUpper()}";
            
            if (currentPlayerState == MediaManager.Player.MediaPlayerState.Loading)
            {
                Element.Value = 0;
            }
            else if (currentPlayerState == MediaManager.Player.MediaPlayerState.Playing
                    && CrossMediaManager.Current.Duration.Ticks > 0)
            {
                Element.Maximum = CrossMediaManager.Current.Duration.Ticks;
            }
        }
        async Task StartDeleteAudioFile(int? id)
        {
            AudioFileManagement audioFileManagement = new AudioFileManagement();
            audioFileManagement.DeleteAudioFile(id);
            await LocalCache.DeleteHighlightedAudioClip(id);
        }

        async Task StartDeleteSession(int? id )
        {
            var canExecute = await DeleteSnackBarAsync("About to delete the session", "Cancel", null);
            if (canExecute)
            {
                await LocalCache.DeleteSession(Sessions[id].Id);
                ViewSessions.Remove(Sessions[id]);
                Sessions[id].Children.ForEach(async (x) => await StartDeleteAudioFile(x.Id));
                ManageData.DeleteSession(Sessions[id]);
            }
           
            
        }
        async Task UpdateViewSessions( int? parentId)
        {
            //a different search method should be implemented and use of task worker if the array gets large enough to cause a UI freeze.
            for (int i = 0; i < ViewSessions.Count; i++)
            {
                if (ViewSessions[i].Id == parentId)
                {
                    ViewSessions[i] = Sessions[parentId];
                    break;
                }
            }

        }
        async Task ToastAsync(string title)
        {
            //await Application.Current.MainPage.DisplayToastAsync(title);
            var element = Application.Current.MainPage.FindByName<Ellipse>("Scrolls");
            await element.DisplayToastAsync(new ToastOptions
            {
                BackgroundColor = Color.Green,
                Duration = TimeSpan.FromSeconds(3),
                CornerRadius = 10,
                MessageOptions = new MessageOptions
                {
                    Message = title,
                    Padding = new Thickness(10),
                   
                }
            });

        }
        async Task<bool> DeleteSnackBarAsync(string title, string buttonText, Func<Task> task)
        {
            return  !await Application.Current.MainPage.DisplaySnackBarAsync(title, buttonText, task, TimeSpan.FromSeconds(3));
        }
        private void SetupCurrentMediaPositionData(TimeSpan currentPlaybackPosition)
        {
            var formattingPattern = @"hh\:mm\:ss";
            if (CrossMediaManager.Current.Duration.Hours <= 0)
                formattingPattern = @"mm\:ss";
            var element = Application.Current.MainPage.FindByName<Slider>("Slider");
            var fullLengthString = CrossMediaManager.Current.Duration.ToString(formattingPattern);
            AudioTime = $"{currentPlaybackPosition.ToString(formattingPattern)}/{fullLengthString}";

           element.Value = currentPlaybackPosition.Ticks;
        }

        private void Current_MediaItemChanged(object sender, MediaItemEventArgs e)
        {
            SetupCurrentMediaDetails(e.MediaItem);
        }

        private void Current_PositionChanged(object sender, MediaManager.Playback.PositionChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                SetupCurrentMediaPositionData(e.Position);
            });
        }

        private void Current_OnStateChanged(object sender, StateChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                SetupCurrentMediaPlayerState(e.State);
            });
        }
    }
}

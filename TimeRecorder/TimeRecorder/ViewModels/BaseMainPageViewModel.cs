

namespace TimeRecorder.ViewModels
{
    public abstract class BaseMainPageViewModel : ContentPage , INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(
           [CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }
        protected AudioRecorderService recorder;

        public virtual int BufferLength
        {
            get => _bufferLength;
            set
            {
                _bufferLength = value;
                OnPropertyChanged();
            }
        }
        public bool disposeStream = false;
        public bool Mark = false; 
        public virtual bool IsRecording
        {
            get => _isRecording;
            set
            {
                _isRecording = value;
                OnPropertyChanged();
            }
        }
        public virtual bool StartHighlighedAudioClip
        {
            get => _startHighlightedAudioClip;
            set
            {
                _startHighlightedAudioClip = value;
                OnPropertyChanged();
            }
        }
        public virtual LocalCache LocalCache 
        {
            get => _localCache;
            set
            {
                _localCache = value;
                OnPropertyChanged();
            }
        }
        public virtual Timer Timer
        {
            get => _timer;
            set
            {
                _timer = value;
                OnPropertyChanged();
            }
        }
      
        protected BaseMainPageViewModel()
        {
           
        }
        public async Task StartRecord()
        {
            if (!recorder.IsRecording)
            {
                await RecordAudio();
            }

        }


        protected virtual async Task RecordAudio()
        {
            try
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    IAudio audio = DependencyService.Get<IAudio>();
                    audio.Initialize();
                }
                await DoWork();

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        protected async Task DoWork()
        {

            var worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
           
            worker.RunWorkerAsync();
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ReadyIt);


        }

        protected virtual async void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //There is a fade in effect that is noticable when the second stream is spliced together.
            try
            {
               
            }
            catch (Exception ex)
            {
                throw ex;

            }

        }
      void  MakeStreams(List<List<long>> audioClips, Stream stream)
        {
            try
            {
               
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

      

        protected abstract void ReadyIt(object sender, RunWorkerCompletedEventArgs e);
    }
}

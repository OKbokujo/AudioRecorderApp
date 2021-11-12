using Rg.Plugins.Popup.Services;
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
    public class NotesViewModel: BaseViewModel
    {
        readonly LocalCache LocalCache;
        readonly ManageData ManageData;

        private HighlightedAudioClip HighlightedAudioClip;
        
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private string _notes;
        public string Notes
        {
            get => _notes;
            set
            {
                _notes = value;
                OnPropertyChanged();
            }
        }
        private Session Session;



        public ICommand ClosePageCM => new Command(async () => await ClosePage());
        public ICommand SaveDataCM => new Command(async () => await SaveData());
        public NotesViewModel(LocalCache localCache, ManageData manageData)
        {
            LocalCache = localCache;
            ManageData = manageData;

        }
  

         public void SetObject(object element)
        {
           if(element.GetType() == typeof(Session) )
            {
                Session = (Session)element;

            }
           else
            {
                HighlightedAudioClip = (HighlightedAudioClip)element;
            }
            SetVariables();
        }
        async Task SaveData()
        {
            if(Session != null)
            {
                Session.SessionName = Name;
                Session.Notes = Notes;
                await LocalCache.AddUpdateSession(Session);
            }
            else
            {
                HighlightedAudioClip.Name = Name;
                HighlightedAudioClip.Notes = Notes;
                await  LocalCache.AddUpdateHighlightedAudioClip(HighlightedAudioClip);
            }
        }
        void SetVariables()
        {
            Name = Session != null ? Session.SessionName : HighlightedAudioClip.Name;
            if (Name == null)
                Name = "Name";
            Notes = Session != null ? Session.Notes : HighlightedAudioClip.Notes;
            if (Notes == null)
                Notes = "Notes";
        }
        async Task ClosePage()
        {
            await PopupNavigation.Instance.PopAsync();
        }
    }
}

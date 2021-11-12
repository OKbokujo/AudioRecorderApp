using Newtonsoft.Json;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeRecorder.Models;
using TimeRecorder.Services;
using TimeRecorder.Views;
using Xamarin.Forms;

namespace TimeRecorder.ViewModels
{
    public class ItemMenuViewModel: BaseViewModel
    {
        public ICommand DeleteItemCM => new Command(async () => await DeleteItem());
        public ICommand EditItemCM => new Command(async () => await EditItem());
        private object Element;
        string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        private Session Session;
        private HighlightedAudioClip HighlightedAudioClip;
        private MessengerItem MessengerItem;
       
        public ItemMenuViewModel()
        {
           
            MessengerItem = new MessengerItem();
        }
        public void SetObject(object element)
        {
            if (element.GetType() == typeof(Session))
            {
                Session = (Session)element;
                Name = Session.SessionName;
            }
            else
            {
                HighlightedAudioClip = (HighlightedAudioClip)element;
                Name = HighlightedAudioClip.Name;
            }
            
         
        }

        async Task DeleteItem()
        {
            MessengerItem.ItemType = Session != null ? "DeleteSession" : "DeleteHighlightedAudioClip";
            MessengerItem.Session = Session;
            MessengerItem.HighlightedAudioClip = HighlightedAudioClip;
            SendMessage();
            await ClosePage();
        }
        async Task EditItem()
        {
            Element = Session != null ? (object)Session : (object)HighlightedAudioClip;
            NotesPage notesPage = new NotesPage(Element);
            notesPage.Disappearing += NotesPage_Disappearing;
          
            await Navigation.PushPopupAsync(notesPage);
        }

        private void NotesPage_Disappearing(object sender, EventArgs e)
        {
            ClosePage();
            MessengerItem.ItemType = Session != null? Session.Id.ToString() : HighlightedAudioClip.ParentID.ToString();
            MessengerItem.Session = Session;
            SendMessage();
           
        }

        async Task ClosePage()
        {
            await PopupNavigation.Instance.PopAllAsync();
           
        }
        void SendMessage()
        {
            MessagingCenter.Send(this, "1",JsonConvert.SerializeObject(MessengerItem));
        }
    }
}

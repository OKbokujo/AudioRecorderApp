using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeRecorder.Modules;
using TimeRecorder.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TimeRecorder.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotesPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public NotesPage(object element)
        {
            InitializeComponent();
            var service = Startup.ServiceProvider.GetService<NotesViewModel>();
            service.SetObject(element);
            BindingContext = service;


        }
    }
}
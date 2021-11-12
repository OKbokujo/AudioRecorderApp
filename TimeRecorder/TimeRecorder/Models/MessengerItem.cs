using System;
using System.Collections.Generic;
using System.Text;

namespace TimeRecorder.Models
{
    public class MessengerItem
    {

        public string ItemType { get; set; }
        public Session Session { get; set; }
        public HighlightedAudioClip HighlightedAudioClip { get; set; }
    }
}

using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeRecorder.Models
{
    [Table("Settings")]
    public class Settings
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
        [MaxLength(100)]

        public int AudioType { get; set; }
        public bool SpeechToText { get; set; }
        public bool BlackScreenWhileRecording { get; set; }
        public int? RecordStartDelay { get; set; }
        public bool SaveWholeAudioSession { get; set; }
        
        



    }
}

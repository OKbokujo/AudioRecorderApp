using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeRecorder.Models
{
    [Table("AudioClips")]
    public class HighlightedAudioClip
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
        public int? ParentID { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public string TimeLength { get; set; }
        public string FileSizeMB { get; set; }
        public string FormatType { get; set; }

    }
}

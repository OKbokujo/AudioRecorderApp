using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeRecorder.Models
{
    [Table("Sessions")]
    public class Session
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
        public string ChildrenID { get; set; }
        [MaxLength(100)]
        public string SessionName { get; set; }
        public string Notes { get; set; }
        public DateTime CreationDate { get; set; }

        public int RecordingsCount { get; set; }
        [Ignore]
        public List<HighlightedAudioClip>  Children {get;set;}
    }
}

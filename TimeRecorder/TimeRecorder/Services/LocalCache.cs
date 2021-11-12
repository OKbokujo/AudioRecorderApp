using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TimeRecorder.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TimeRecorder.Services
{
    public class LocalCache
    {
        readonly SQLiteAsyncConnection _conn;
        readonly SQLiteConnectionString connect;
        readonly string FolderPath;
        Dictionary<int?, HighlightedAudioClip> HighlightedAudioClips;
        Dictionary<int?, Session> Sessions;
        Settings Settings;
        public LocalCache()
        {
            FolderPath = System.IO.Path.Combine(FileSystem.AppDataDirectory, "TimeRecorder.db3");
            connect = new SQLiteConnectionString(FolderPath, true);
            _conn = new SQLiteAsyncConnection(connect);
            _conn.CreateTableAsync<Session>();
            _conn.CreateTableAsync<Settings>();
            _conn.CreateTableAsync<HighlightedAudioClip>();
            HighlightedAudioClips = new Dictionary<int?, HighlightedAudioClip>();
            Sessions = new Dictionary<int?, Session>();
            Settings = new Settings();
        }
   
        public async Task<Settings> GetSettings()
       {
           
            var entries = await _conn.Table<Settings>().ToArrayAsync();
            foreach(var x in entries)
            {
                Settings = new Settings
                {
                    Id = x.Id,
                    AudioType = x.AudioType,
                    RecordStartDelay = x.RecordStartDelay,
                    BlackScreenWhileRecording = x.BlackScreenWhileRecording,
                    SpeechToText = x.SpeechToText,
                    SaveWholeAudioSession = x.SaveWholeAudioSession
                };
            }
            return Settings;
        }
        public async Task<Dictionary<int?, Session>> GetSessions()
        {
            try
            {


                var entries = await _conn.Table<Session>().ToArrayAsync();
                foreach (var x in entries)
                {
                    Sessions.Add(x.Id, new Session
                    {
                        Id = x.Id,
                        ChildrenID = x.ChildrenID,
                        SessionName = x.SessionName,
                        Notes = x.Notes,
                        RecordingsCount = x.RecordingsCount,
                        CreationDate = x.CreationDate,
                        
              
                    });
                }
            }
            catch(Exception  ex)
            {
                throw ex;
            }
            return Sessions;
        }
        public async Task<Session> GetSession(int? id)
        {
            TableMapping tableMapping = new TableMapping(typeof(Session));
            return (Session)await _conn.GetAsync(id, tableMapping);
        }
        public async Task<Dictionary<int?, HighlightedAudioClip>> GetHighlightedAudioClips()
        {
            try
            {
                HighlightedAudioClips = new Dictionary<int?, HighlightedAudioClip>();
                var entries = await _conn.Table<HighlightedAudioClip>().ToArrayAsync();
                foreach (var x in entries)
                {
                    HighlightedAudioClips.Add(x.Id, new HighlightedAudioClip
                    {
                        Id = x.Id,
                        ParentID = x.ParentID,
                        Name = x.Name,
                        Notes = x.Notes,
                        TimeLength = x.TimeLength,
                        FileSizeMB = x.FileSizeMB,
                        FormatType = x.FormatType
                    }) ;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return HighlightedAudioClips;
        }
        public async Task AddUpdateSession(Session entry)
        {
            await _conn.InsertOrReplaceAsync(entry);
        }
        public async Task AddUpdateSetting(Settings entry)
        {
            await _conn.InsertOrReplaceAsync(entry);
        }
        public async Task AddUpdateHighlightedAudioClip(HighlightedAudioClip entry)
        {
            await _conn.InsertOrReplaceAsync(entry);
        }
        public async Task DeleteSession(object entry)
        {
            await _conn.DeleteAsync<Session>(entry);
        }
        public async Task DeleteHighlightedAudioClip(object entry)
        {
            await _conn.DeleteAsync<HighlightedAudioClip>(entry);
        }

        public async Task CreateSaveToFolder(string folderName)
        {
           var saveLocation = System.IO.Path.Combine(FileSystem.AppDataDirectory, folderName);
        }
    }
}

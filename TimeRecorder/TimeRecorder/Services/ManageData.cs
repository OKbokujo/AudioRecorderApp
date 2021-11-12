using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TimeRecorder.Models;

namespace TimeRecorder.Services
{
    public class ManageData
    {
        Dictionary<int?,Session> Sessions;
        Dictionary<int?,HighlightedAudioClip> HighlightedAudioClips;
       
     
        public async Task DeleteHighlightedAudioClip(int? id, int? parentID = -1)
        {
           
            if (parentID != -1)
            {
                Sessions[parentID].ChildrenID = UpdateParentsChildrenIDs(Sessions[parentID].ChildrenID, id);
                Sessions[parentID].Children.Remove(HighlightedAudioClips[id]);
                Sessions[parentID].RecordingsCount -= 1;
            }
            HighlightedAudioClips.Remove(id);
        }
        public void DeleteSession(Session session)
        {
            try
            {
                List<int?> childrenID = JsonConvert.DeserializeObject<List<int?>>(Sessions[session.Id].ChildrenID);
                childrenID.ForEach( (x) =>  DeleteHighlightedAudioClip(x));
                Sessions.Remove(session.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<int?, HighlightedAudioClip> GetHighlightedAudioClips()
        {
            return HighlightedAudioClips;
        }
        public Dictionary<int?, Session> GetSessions()
        {
            return Sessions;
        }
        public void MakeHighlightedAudioClipLists(Dictionary<int?, Session> sessions)
        {
            try
            {
                foreach (var x in sessions)
                {
                    x.Value.Children = new List<HighlightedAudioClip>();
                    var group = JsonConvert.DeserializeObject<List<int?>>(x.Value.ChildrenID);
                    foreach (var y in group)
                    {
                        x.Value.Children.Add(HighlightedAudioClips[y]);
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task StartHighlightedAudioClipProcess(int? id)
        {
             DeleteHighlightedAudioClip(id, HighlightedAudioClips[id].ParentID);
        }
        public void SetSessions(Dictionary<int?, Session> sessions)
        {
            Sessions = sessions;
        }
        public async void SetHighlightedAudioClips(Dictionary<int?, HighlightedAudioClip> highlightedAudioClips)
        {
            HighlightedAudioClips = new Dictionary<int?, HighlightedAudioClip>();
            HighlightedAudioClips = highlightedAudioClips;
        }
        public string UpdateParentsChildrenIDs(string ids, int? id)
        {
            //If I allow users to add to a specific session, I'll just add an additional parameter that signifies to flips between childrenID "remove" and "add"
            List<int?> childrenID = JsonConvert.DeserializeObject<List<int?>>(ids);
            childrenID.Remove(id);
            return JsonConvert.SerializeObject(childrenID);
        }
        public void UpdateSession(Session session, Dictionary<int?, HighlightedAudioClip> highlightedAudioClips)
        {
            try
            {
                List<int?> childrenID = new List<int?>();
                foreach (var x in highlightedAudioClips)
                {
                    childrenID.Add(x.Value.Id);
                    x.Value.ParentID = session.Id;
                    HighlightedAudioClips.Add(x.Key, x.Value);
                }
                session.ChildrenID = JsonConvert.SerializeObject(childrenID);
                session.RecordingsCount = childrenID.Count;
                Sessions.Add(session.Id, session);
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }
        
      
      
       
      
    }
}

using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TimeRecorder.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TimeRecorder.Services
{
   public class AudioFileManagement
    {
        List<Stream> Streams;
        Dictionary<int?, HighlightedAudioClip> HighlightedAudioClips;
        int? ParentID;
        string HighlightedAudioClipsPath;
        public AudioFileManagement(List<Stream> streams = null, int? parentID = -1)
        {
            Streams = streams;
            ParentID = parentID;
            HighlightedAudioClipsPath = Path.Combine(FileSystem.AppDataDirectory, "highlightedAudioClips/");
            System.IO.Directory.CreateDirectory(HighlightedAudioClipsPath);
            HighlightedAudioClips = new Dictionary<int?, HighlightedAudioClip>();

        }
        public async Task ConvertAndSave(int? id)
        {
            try
            {   
                for (int i = 0; i < Streams.Count; i++)
                {
                    HighlightedAudioClip highlightedAudioClip = new HighlightedAudioClip();
                    highlightedAudioClip.Id = id + 1 + i;
                    highlightedAudioClip.Name = (i+1).ToString();
                    // new method to make
                    highlightedAudioClip.FormatType = "wav";
                    highlightedAudioClip.TimeLength = await GetTime((Streams[i].Length / ((double)44100 * (double)(16 / 8))));
                    string filesize = (Streams[i].Length / 500000.0 * 0.4768).ToString();
                    highlightedAudioClip.FileSizeMB = filesize.Substring(0,filesize.IndexOf(".") + 3) + " MB";
                    // end of new method
                    WaveFormat waveFormat = new WaveFormat(44100, 1);
                    Streams[i].Seek(0, SeekOrigin.Begin);
                    var ResourseWaveStream = new RawSourceWaveStream(Streams[i], waveFormat);
                    var SampleProvider = ResourseWaveStream.ToSampleProvider().ToWaveProvider16();
                    string path = HighlightedAudioClipsPath + $"{highlightedAudioClip.Id}.wav";

                    using (var fileWriter = new WaveFileWriter(path , SampleProvider.WaveFormat))
                    {
                        ResourseWaveStream.CopyTo(fileWriter);
                    }
                    HighlightedAudioClips.Add(highlightedAudioClip.Id, highlightedAudioClip);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
 
        }
        async Task<string> GetTime(double time)
        {
            string minutes = ((int)time / 60).ToString();
            string seconds = ((int)(time % 60)).ToString();
            minutes = minutes.Length == 1 ? $"0{minutes}:" : minutes;
            seconds = seconds.Length == 1 ? $"0{seconds}" : seconds;
            return minutes + seconds;
        }
        public Dictionary<int?, HighlightedAudioClip> GetHighlightedAudioClips()
        {
            return HighlightedAudioClips;
        }
        public void DeleteAudioFile(int? id)
        {
           
                string file = HighlightedAudioClipsPath + id + ".wav";
                File.Delete(file);
    
        }
    }
}

using System;
using System.Collections.Generic;
using Controllers;
using UnityEngine;
using Utilities;
using Utilities.Files;

namespace Managers.Settings.Local
{
    [Serializable]
    public class LocalSettings
    {
        [NonSerialized] private bool _needWaitFrameToSave;
        
        public string microphoneName = "";
        public float sensitivityMicrophone = 1;
        public float trackVolume = 0.85f;
        public float recordVolume = 0.85f;
        public bool autoRecord;
        public List<TrackData> tracks = new List<TrackData>();
        public List<PlayerData> players = new List<PlayerData>();
        
        public event Action OnChangeMicrophoneName;
        
        #region Microphone

        public void SetMicrophoneName(string value)
        {
            microphoneName = value;
            
            Save();
            
            OnChangeMicrophoneName?.Invoke();
        }

        public string GetMicrophoneName()
        {
            return microphoneName;
        }

        public void SetSensitivityMicrophone(float value)
        {
            value = Mathf.Clamp(value, 0f, 100f);
            
            sensitivityMicrophone = value;
            
            Save();
        }

        public float GetSensitivityMicrophone()
        {
            return sensitivityMicrophone;
        }

        #endregion

        #region Tracks

        public void AddTrack(Action<TrackData> onComplete)
        {
            File.OpenFile(TypeContent.Sound,MainController.Instance.TextManager.GetText(538), path =>
            {
                if (string.IsNullOrEmpty(path))
                {
                    onComplete?.Invoke(null);
                    return;
                }
                
                var localPath = File.CopyMusicFileToStreamingAssets(path);

                var data = new  TrackData();
                data.SetNameTrack(File.GetNameFile(path));
                data.SetPathTrack(localPath);
                
                tracks.Add(data);
                Save();
                
                onComplete?.Invoke(data);
            });
        }

        public void RemoveTrack(TrackData trackData)
        {
            if(!tracks.Contains(trackData))
                return;
            
            tracks.Remove(trackData);
            Save();
        }

        public List<TrackData> GetTracks()
        {
            var result =  new List<TrackData>();

            for (int i = 0; i < tracks.Count; i++)
            {
                var trackData = tracks[i];
                if(trackData.IsExist())
                    result.Add(trackData);
            }
            
            return result;
        }

        #endregion

        #region Players

        public List<PlayerData> GetPlayers()
        {
            return players;
        }

        public PlayerData CreatePlayer()
        {
            var playerData = new PlayerData();
            playerData.SetNamePlayer(MainController.Instance.TextManager.GetText(1008));
            
            players.Add(playerData);
            Save();
            
            return playerData;
        }
        
        public void RemovePlayer(PlayerData playerData)
        {
            if(!players.Contains(playerData))
                return;
            
            playerData.OnRemove();
            players.Remove(playerData);
            Save();
        }

        #endregion
        
        #region Volume track / record

        public float GetTrackVolume()
        {
            return trackVolume;
        }

        public void SetTrackVolume(float value)
        {
            value = Mathf.Clamp01(value);
            trackVolume = value;
            Save();
        }
        
        public float GetRecordVolume()
        {
            return  recordVolume;
        }

        public void SetRecordVolume(float value)
        {
            value = Mathf.Clamp01(value);
            recordVolume = value;
            Save();
        }

        #endregion

        public bool GetAutoRecord()
        {
            return autoRecord;
        }

        public void SetAutoRecord(bool value)
        {
            autoRecord = value;
            
            Save();
        }

        private static string GetPath()
        {
            return File.PathCombine(File.GetPathToStreamingAssets(), "Settings.json");
        }
        
        public static LocalSettings Load()
        {
            var path = GetPath();
            Log.Message($"[PATH TO LOCAL CONFIGURATION] {path}");
            
            if (File.FileExists(path))
            {
                try
                {
                    return JsonUtility.FromJson<LocalSettings>(File.ReadAllText(path));
                }
                catch (Exception e)
                {
                    return new LocalSettings();
                }
            }
            else
            {
                return new LocalSettings();
            }
        }
        
        public void Save(bool needWaitFrame = true)
        {
            var path = GetPath();
            
            if (needWaitFrame)
            {
                _needWaitFrameToSave = true;
                return;
            }

            _needWaitFrameToSave = false;
            
            var serilizeble = JsonUtility.ToJson(this,true);
            File.WriteAllText(path, serilizeble);
        }
        
        public void OnUpdate()
        {
            if (_needWaitFrameToSave)
                Save(false);
        }
        //----
    }
}

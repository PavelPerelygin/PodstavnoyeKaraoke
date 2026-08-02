using System;
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
            if (value < 1) value = 1;
            else if (value > 100) value = 100;
            
            sensitivityMicrophone = value;
            
            Save();
        }

        public float GetSensitivityMicrophone()
        {
            return sensitivityMicrophone;
        }

        #endregion

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
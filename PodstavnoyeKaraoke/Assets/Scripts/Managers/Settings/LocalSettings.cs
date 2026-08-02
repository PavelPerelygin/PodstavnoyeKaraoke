using System;
using Controllers.Skins;
using UnityEngine;
using Utilities;
using Utilities.Files;

namespace Managers.Settings.Local
{
    [Serializable]
    public class LocalSettings
    {
        [NonSerialized] private bool _needWaitFrameToSave;

        public SourceData mainScreenBackground = new SourceData();
        public SourceData selectScreenBackground = new SourceData();
        public SourceData gameScreenBackground = new SourceData();
        public string microphoneName = "";
        public float sensitivityMicrophone = 1;
        public int ballWeight = 900;
        public int liftingForce = 50;
        public string skinName = SkinData.DefaultSkinName;
        
        public event Action OnChangeMicrophoneName;
        public event Action OnChangeSkin;

        #region Backgrounds

        public SourceData GetMainScreenBackground()
        {
            return mainScreenBackground;
        }
        
        public SourceData GetSelectScreenBackground()
        {
            return selectScreenBackground;
        }
        
        public SourceData GetGameScreenBackground()
        {
            return gameScreenBackground;
        }

        #endregion
        
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
        
        public void SetBallWeight(int value)
        {
            ballWeight = value;
        }

        public int GetBallWeight()
        {
            return ballWeight;
        }
        
        #region Lift force

        public void SetLiftingForce(int value)
        {
            liftingForce = value;
        }

        public int GetLiftingForce()
        {
            return liftingForce;
        }
        
        #endregion

        #endregion

        #region Skin

        public string GetSkinName()
        {
            return skinName;
        }

        public void SetSkinName(string value)
        {
            skinName = value;
            
            Save();
            
            OnChangeSkin?.Invoke();
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
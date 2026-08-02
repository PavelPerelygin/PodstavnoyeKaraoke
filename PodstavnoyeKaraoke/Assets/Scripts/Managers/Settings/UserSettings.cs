using System;
using System.Collections.Generic;
using Controllers;
using UnityEngine;
using Utilities;
using Utilities.Files;

namespace Managers.Settings
{
    [Serializable]
    public class FlagInfo
    {
        public string _name = "";
        public string _value = "";
    }
    
    [Serializable]
    public class UserSettings
    {
        [NonSerialized] private bool _needWaitFrameToSave;
        //----
        public float _musicVolume = 0.85f;
        public float _soundVolume = 0.85f;
        public bool _checkNeedUpdateGame = true;
        public List<FlagInfo> _flags = new List<FlagInfo>();
        //----
        private static string GetPath()
        {
            return File.PathCombine(Application.persistentDataPath, "Profile.json");
        }
        
        public static UserSettings Load()
        {
            var path = GetPath();
            Log.Message($"[PATH TO USER CONFIGURATION] {path}");
            
            if (File.FileExists(path))
            {
                try
                {
                    return JsonUtility.FromJson<UserSettings>(File.ReadAllText(path));
                }
                catch (Exception e)
                {
                    return new UserSettings();
                }
            }
            else
            {
                return new UserSettings();
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
            
            string serilizebleLicense = JsonUtility.ToJson(this,true);
            File.WriteAllText(path, serilizebleLicense);
        }
        
        public void OnUpdate()
        {
            if (_needWaitFrameToSave)
                Save(false);
        }
        
        //----

        #region Volume music sound

        public float GetMusicVolume()
        {
            return _musicVolume;
        }

        public void SetMusicVolume(float value)
        {
            _musicVolume = value;
            Save();
        }
        
        public float GetSoundVolume()
        {
            return  _soundVolume;
        }

        public void SetSoundVolume(float value)
        {
            _soundVolume = value;
            Save();
        }

        #endregion

        #region Update game

        public bool GetNeedUpdateGame()
        {
            return  _checkNeedUpdateGame;
        }

        public void SetNeedUpdateGame(bool value)
        {
            _checkNeedUpdateGame = value;
            Save();
        }

        #endregion

        #region Flags

        private FlagInfo GetFlag(string name)
        {
            for (int i = 0; i < _flags.Count; i++)
            {
                var flag = _flags[i];
                if (flag._name == name)
                    return flag;
            }
            
            return null;
        }

        public bool IsNotNullFlag(string name)
        {
            for (int i = 0; i < _flags.Count; i++)
            {
                var flag = _flags[i];
                if (flag._name == name)
                    return true;
            }

            return false;
        }

        public string GetFlagString(string name)
        {
            var flagInfo = GetFlag(name);
            if (flagInfo != null)
                return flagInfo._value;
            
            Log.Assert();
            return "";
        }
        
        public float GetFlagFloat(string name)
        {
            var flagInfo = GetFlag(name);
            if (flagInfo != null)
            {
                if (float.TryParse(flagInfo._value,MainController.Instance.NumberStyle,MainController.Instance.CultureInfo, out var f))
                {
                    return f;
                }
                else
                {
                    Log.Assert();
                    return 0f;
                }
            }

            Log.Assert();
            return 0f;
        }

        public void SetFlag(string name, string value)
        {
            var flagInfo = GetFlag(name);
            if (flagInfo == null)
            {
                _flags.Add(new FlagInfo()
                {
                    _name = name,
                    _value = value
                });
            }
            else
            {
                flagInfo._value = value;
            }
            
            Save();
        }

        #endregion
    }
}
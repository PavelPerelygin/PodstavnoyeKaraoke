using System;
using Controllers;
using Utilities.Files;

namespace Managers.Settings.Local
{
    [Serializable]
    public class TrackData
    {
        public string nameTrack = "";
        public string pathTrack = "";
        
        public event Action OnChangeName;
        
        public void SetNameTrack(string value)
        {
            nameTrack = value;
            
            MainController.Instance.LocalSettings.Save();
            
            OnChangeName?.Invoke();
        }

        public string GetNameTrack()
        {
            return nameTrack;
        }

        public void SetPathTrack(string value)
        {
            pathTrack = value;
            
            MainController.Instance.LocalSettings.Save();
        }

        public string GetPathTrack()
        {
            return pathTrack;
        }

        public bool IsExist()
        {
            if(string.IsNullOrEmpty(pathTrack))
                return false;
            
            return File.FileExistFromStreamingAssets(pathTrack);
        }
    }
}
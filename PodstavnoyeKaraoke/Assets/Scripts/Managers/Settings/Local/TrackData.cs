using System;
using Controllers;

namespace Managers.Settings.Local
{
    [Serializable]
    public class TrackData
    {
        public string nameTrack = "";
        public string pathTrack = "";
        
        public void SetNameTrack(string value)
        {
            nameTrack = value;
            
            MainController.Instance.LocalSettings.Save();
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
    }
}
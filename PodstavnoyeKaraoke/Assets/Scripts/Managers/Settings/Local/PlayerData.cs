using System;
using Controllers;

namespace Managers.Settings.Local
{
    [Serializable]
    public class PlayerData
    {
        public string namePlayer = "";
        
        public event Action OnChangeName;
        
        public void SetNamePlayer(string value)
        {
            namePlayer = value;
            
            MainController.Instance.LocalSettings.Save();
            
            OnChangeName?.Invoke();
        }

        public string GetNamePlayer()
        {
            return namePlayer;
        }
    }
}
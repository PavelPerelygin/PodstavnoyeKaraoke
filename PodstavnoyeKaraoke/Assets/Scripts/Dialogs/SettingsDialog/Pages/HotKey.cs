using System;
using UnityEngine;

namespace Dialogs.SettingsDialog.Pages
{
    public class HotKey : MonoBehaviour
    {
        [SerializeField] private TypePlatform typePlatform;

        private void Awake()
        {
            CheckNeedHideHotkey();
        }

        private void CheckNeedHideHotkey()
        {
            if(typePlatform == TypePlatform.Any)
                return;

            if (typePlatform == TypePlatform.Windows)
            {
#if !UNITY_STANDALONE_WIN
                Hide();
#endif
            }else if (typePlatform == TypePlatform.MacOs)
            {
#if !UNITY_STANDALONE_OSX
                Hide();
#endif  
            }
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
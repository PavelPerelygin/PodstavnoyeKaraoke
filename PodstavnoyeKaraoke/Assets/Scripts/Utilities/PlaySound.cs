using Controllers;
using Managers.Audio;
using UnityEngine;

namespace Utilities
{
    public class PlaySound : MonoBehaviour
    {
        [SerializeField] private string _path;
        [SerializeField] private bool _isExternal;
        [SerializeField] private bool _loop;
        //----------------------------------------------------------------------------------
        public void Play()
        {
            MainController.Instance.AudioManager.Create(_path,TypeGroup.Sound,_isExternal).Play(_loop);
        }
    }
}
using Controllers;
using Managers.Audio;
using Scenes.Base;
using UnityEngine;

namespace Scenes
{
    public class LoaderScene : Scene
    {
        [SerializeField] private MainController _mainController;
        protected override void Awake()
        {
            TypeScene = TypeScene.Loader;
            
            Instantiate(_mainController);

            base.Awake();
        }
        
        protected override void Start()
        { 
            ApplySettings();
            
            base.Start();
        }

        protected override void SceneLoadingComplete()
        {
            base.SceneLoadingComplete();
            
            MainController.Instance.LoadScene(TypeScene.Main, false);
        }

        private void ApplySettings()
        {
            float musicVolume = MainController.Instance.UserSettings.GetMusicVolume();
            MainController.Instance.AudioManager.SetVolumeAuidoGroup(TypeGroup.Music,musicVolume);
        
            float soundVolume = MainController.Instance.UserSettings.GetSoundVolume();
            MainController.Instance.AudioManager.SetVolumeAuidoGroup(TypeGroup.Sound,soundVolume);
        
            MainController.Instance.AudioManager.Create("Background",TypeGroup.Music).Play(true);
        }
    }
}

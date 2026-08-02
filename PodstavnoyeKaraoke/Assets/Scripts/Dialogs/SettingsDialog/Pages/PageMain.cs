using Controllers;
using Extensions;
using Managers.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogs.SettingsDialog.Pages
{
    public class PageMain : BasePage
    {
        [SerializeField] private Button _changeScreenCountButton;
        [SerializeField] private Button _changeScreenModeButton;
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _soundVolumeSlider;

        [SerializeField] private GameObject _secondScreenContainer;
        [SerializeField] private GameObject _fullScreenContainer;

        public override void Init()
        {
            InitButtons();
            InitSliders();
            UpdateVisualByEnableSecondScreen();
        }

        private void InitButtons()
        {
            _changeScreenCountButton.onClick.AddListener(ButtonPress);
            _changeScreenCountButton.DisableOverDownColors();
            if (!MainController.Instance.ScreensController.CheckAvailableSecondScreen())
                _changeScreenCountButton.interactable = false;

            _changeScreenModeButton.onClick.AddListener(ButtonPress);
            _changeScreenModeButton.DisableOverDownColors();
        }
    
        private void InitSliders ()
        {
            _musicVolumeSlider.onValueChanged.AddListener(SliderPress);
            _musicVolumeSlider.value = MainController.Instance.UserSettings.GetMusicVolume();
        
            _soundVolumeSlider.onValueChanged.AddListener(SliderPress);
            _soundVolumeSlider.value = MainController.Instance.UserSettings.GetSoundVolume();
        }

        private void OnEnableSecondScreen()
        {
            MainController.Instance.ScreensController.EnableSecondScreen();
            UpdateVisualByEnableSecondScreen();
        }

        private void OnChangeFullScreen()
        {
            MainController.Instance.ScreensController.ChangeFullScreen();
            UpdateVisualByEnableSecondScreen();
        }

        private void UpdateVisualByEnableSecondScreen()
        {
            if (!MainController.Instance.ScreensController.CheckAvailableSecondScreen())
                _secondScreenContainer.SetActive(false);

            if (MainController.Instance.ScreensController.IsEnableSecondScreen())
            {
                _secondScreenContainer.SetActive(false);
                _fullScreenContainer.SetActive(false);
            }
            else
            {
                var value = "";
        
                if (MainController.Instance.ScreensController.IsFullScreen())
                    value = MainController.Instance.TextManager.GetText(25);
                else
                    value = MainController.Instance.TextManager.GetText(26);
        
                _changeScreenModeButton.SetText(value);
            }
        }
    
        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if (!base.GameObjectClickHandler(selectedObj))
                return false;

            if (selectedObj == _changeScreenCountButton.gameObject)
            {
                OnEnableSecondScreen();
            }else if (selectedObj == _changeScreenModeButton.gameObject)
            {
                OnChangeFullScreen();
            }else if (selectedObj == _musicVolumeSlider.gameObject)
            {
                SetVolume(TypeGroup.Music, _musicVolumeSlider.value);
            }else if (selectedObj == _soundVolumeSlider.gameObject)
            {
                SetVolume(TypeGroup.Sound, _soundVolumeSlider.value);
            }

            return true;
        }
    
        private void SetVolume(TypeGroup audioGroup, float value)
        {
            if(audioGroup == TypeGroup.Music)
                MainController.Instance.UserSettings.SetMusicVolume(value);
            else if (audioGroup == TypeGroup.Sound)
                MainController.Instance.UserSettings.SetSoundVolume(value);
        
            MainController.Instance.AudioManager.SetVolumeAuidoGroup(audioGroup,value);
        }
    }
}

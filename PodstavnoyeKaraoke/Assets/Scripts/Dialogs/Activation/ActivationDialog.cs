using System.Collections.Generic;
using System.Text.RegularExpressions;
using Controllers;
using Dialogs.Base;
using Extensions;
using GameHelper;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogs.Activation
{
    public class ActivationDialog : Dialog
    {
        [SerializeField] private GameObject _offlinePage;
        [SerializeField] private GameObject _onlinePage;
        
        [SerializeField] private InputField _activationKeyInput;
        [SerializeField] private Transform _waitRoot;
        [SerializeField] private GameObject _waitPrefab;
        
        [SerializeField] private Text _requestCodeText;
        [SerializeField] private Button _copyRequestCodeButton;
        [SerializeField] private InputField _responseCodeInput;

        [SerializeField] private Button _modeButton;
        [SerializeField] private Image _modeButtonImage;
        [SerializeField] private Sprite _offlineSprite;
        [SerializeField] private Sprite _onlineSprite;
        
        [SerializeField] private Button _activationButton;
        [SerializeField] private Message _message;
        

        private TypeMode _typeMode = TypeMode.Offline;
        private GameObject _waitObj;
        private bool IsWaitingRespounce = false;
        private int _countPressXKey = 0;
        
        public override void Init()
        {
            InitButtons();
            InitText();
            InitInputFields();
            
            _message.Hide(false);

            UpdatePage();
        }

        private void InitButtons()
        {
            _activationButton.onClick.AddListener(ButtonPress);
            _activationButton.DisableOverDownColors();
            
            _copyRequestCodeButton.onClick.AddListener(ButtonPress);
            _copyRequestCodeButton.DisableOverDownColors();
            
            _modeButton.onClick.AddListener(ButtonPress);
            _modeButton.DisableOverDownColors();
        }

        private void InitText()
        {
            _requestCodeText.text = MainController.Instance.LicenseController.GetRequestCode();
        }
        
        private void InitInputFields()
        {
            _responseCodeInput.DisableOverDownColors();
            _activationKeyInput.DisableOverDownColors();
        }

        #region Offline

        private void CopyRequestCode()
        {
            GUIUtility.systemCopyBuffer = _requestCodeText.text;
        }

        #endregion

        #region Online

        private string RemovingSpaces(string value)
        {
            return Regex.Replace(value, @"^\s*", "");
        }
        
        private void ServerResponse(string message)
        {
            IsWaitingRespounce = false;

            CanvasGroup cg = _waitObj.GetComponent<CanvasGroup>();
            cg.AlphaCanvas(0f, 1f).setOnComplete(() =>
            {
                Destroy(_waitObj);
                _waitObj = null;
            });
            
            if (MainController.Instance.LicenseController.IsGameActivated())
            {
                Hide();
            }
            else
            {
                if (message == "")
                    _message.SetMessage(MainController.Instance.TextManager.GetText(202));
                else if (message == "6")
                    _message.SetMessage(MainController.Instance.TextManager.GetText(201));
                else if (message == "2")
                    _message.SetMessage(MainController.Instance.TextManager.GetText(204));
                else
                    _message.SetMessage(MainController.Instance.TextManager.GetText(202) + message);
            }
            
        }

        #endregion

        private void UpdatePage()
        {
            if (_typeMode == TypeMode.Offline)
            {
                _modeButtonImage.sprite = _onlineSprite;
                _offlinePage.SetActive(true);
                _onlinePage.SetActive(false);
            }
            else if (_typeMode == TypeMode.Online)
            {
                _modeButtonImage.sprite = _offlineSprite;
                _onlinePage.SetActive(true);
                _offlinePage.SetActive(false);
            }
        }

        private void ActivateGame()
        {
            if (_typeMode == TypeMode.Offline)
            {
                if(_responseCodeInput.text == "")
                    return;
            
                MainController.Instance.LicenseController.OfflineActivateGame(_responseCodeInput.text, (bool v) =>
                {
                    if(!v) return;
                
                    Hide();
                });   
            }
            else if (_typeMode == TypeMode.Online)
            {
                if(_activationKeyInput.text == "")
                    return;
        
                if(IsWaitingRespounce)
                    return;

                string key = RemovingSpaces(_activationKeyInput.text);

                _waitObj = Instantiate(_waitPrefab,_waitRoot);
                CanvasGroup cg = _waitObj.GetComponent<CanvasGroup>();
                cg.alpha = 0f;
                cg.AlphaCanvas(1f, 0.1f);

                IsWaitingRespounce = true;
                MainController.Instance.LicenseController.OnlineActivateGame(key,ServerResponse);
            }
        }

        private void ChangePage()
        {
            if (_typeMode == TypeMode.Offline) _typeMode = TypeMode.Online;
            else if (_typeMode == TypeMode.Online) _typeMode = TypeMode.Offline;
            
            UpdatePage();
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if (!base.GameObjectClickHandler(selectedObj))
                return false;
            
            if (IsWaitingRespounce)
                return false;

            if (selectedObj == _activationButton.gameObject)
            {
                ActivateGame();
            }
            else if (selectedObj == _copyRequestCodeButton.gameObject)
            {
                CopyRequestCode();
            }
            else if (selectedObj == _modeButton.gameObject.gameObject)
            {
                ChangePage();
            }

            return true;
        }
        
        protected override bool KeyPressHandler()
        {
            if (!base.KeyPressHandler())
                return false;

            if (IsWaitingRespounce)
                return false;
            
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ActivateGame();
            }

            return true;
        }
    }
}
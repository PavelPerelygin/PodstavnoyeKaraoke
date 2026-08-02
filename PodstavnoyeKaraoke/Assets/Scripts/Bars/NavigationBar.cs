using Boards;
using Boards.Base;
using Controllers;
using Extensions;
using Game.Pages;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Bars
{
    public class NavigationBar : Interactable
    {
        [SerializeField] private Button _mainPageButton;
        [SerializeField] private Button _listLevelsPageButton;
        [SerializeField] private Button _previousPageButton;
        [SerializeField] private Button _stopButton;
        [SerializeField] private Button _openSettingsDialogButton;

        private bool _gameHasBeenLaunched;
        private LTDescr _moveLtd;

        public void Init()
        {
            MainController.Instance.RememberGameObject(gameObject);
            
            InitButtons();
        }
        
        
        private void InitButtons()
        {
            _mainPageButton.onClick.AddListener(ButtonPress);
            _mainPageButton.DisableOverDownColors();
        
            _listLevelsPageButton.onClick.AddListener(ButtonPress);
            _listLevelsPageButton.DisableOverDownColors();
            
            _previousPageButton.onClick.AddListener(ButtonPress);
            _previousPageButton.DisableOverDownColors();
            
            _stopButton.onClick.AddListener(ButtonPress);
            _stopButton.DisableOverDownColors();
        
            _openSettingsDialogButton.onClick.AddListener(ButtonPress);
            _openSettingsDialogButton.DisableOverDownColors();
        }
        
        public void OnChangePage(bool smoothly, float delay)
        {
            var showPosition = gameObject.transform.position;
            var hidePosition = gameObject.GetPositionOffScreenByDirection(Vector2.down);

            if (!_gameHasBeenLaunched)
            {
                _gameHasBeenLaunched = true;
                smoothly = false;
            }

            TryCancelMoveLtd();

            if (smoothly)
            {
                _moveLtd = gameObject.LeanMove(hidePosition, 0.3f).setEase(LeanTweenType.easeInBack).setOnComplete(() =>
                {
                    _moveLtd = null;
                    
                    UpdateVisual();
                
                    gameObject.LeanMove(showPosition, 0.3f).setDelay(delay).setEase(LeanTweenType.easeOutBack);
                });
            }
            else
            {
                UpdateVisual();
            }
        }

        private void TryCancelMoveLtd()
        {
            if(_moveLtd == null)
                return;
            
            LeanTween.cancel(_moveLtd.id);
            _moveLtd = null;
        }

        private void UpdateVisual()
        {
            var mainBoard = MainController.Instance.ActiveScene.ActiveBoard as MainBoard;
            if (mainBoard == null)
            {
                Log.Assert();
                return;
            }

            var currentTypePage = mainBoard.GetCurrentTypePage();
            
            if (currentTypePage == TypePage.Main || currentTypePage == TypePage.None)
            {
                _mainPageButton.gameObject.SetActive(false);
                _listLevelsPageButton.gameObject.SetActive(true);
                _previousPageButton.gameObject.SetActive(false);
                _stopButton.gameObject.SetActive(false);
                _openSettingsDialogButton.gameObject.SetActive(true);
            }
            else if (currentTypePage == TypePage.ListLevels)
            {
                _mainPageButton.gameObject.SetActive(true);
                _listLevelsPageButton.gameObject.SetActive(false);
                _previousPageButton.gameObject.SetActive(true);
                _stopButton.gameObject.SetActive(false);
                _openSettingsDialogButton.gameObject.SetActive(false);
            }
            else if (currentTypePage == TypePage.Level)
            {
                _mainPageButton.gameObject.SetActive(true);
                _listLevelsPageButton.gameObject.SetActive(false);
                _previousPageButton.gameObject.SetActive(true);
                _stopButton.gameObject.SetActive(true);
                _openSettingsDialogButton.gameObject.SetActive(false);
            }
        }
        
        private void OpenMainPage()
        {
            var board = MainController.Instance.ActiveScene.ActiveBoard as MainBoard;
            if (board == null)
            {
                Log.Assert();
                return;
            }
            
            board.OpenMainPage(true);
        }
        
        private void OpenListLevelsPage()
        {
            var board = MainController.Instance.ActiveScene.ActiveBoard as MainBoard;
            if (board == null)
            {
                Log.Assert();
                return;
            }
            
            board.OpenListLevelsPage(true);
        }
        
        private void OpenPreviousPage()
        {
            var board = MainController.Instance.ActiveScene.ActiveBoard as MainBoard;
            if (board == null)
            {
                Log.Assert();
                return;
            }
            
            board.OpenPreviousPage(true);
        }

        private void StopLevel()
        {
            var board = MainController.Instance.ActiveScene.ActiveBoard as MainBoard;
            if (board == null)
            {
                Log.Assert();
                return;
            }
            
            board.StopLevel();
        }
        
        private void OpenSettingsDialog()
        {
            var board = MainController.Instance.ActiveScene.ActiveBoard as MainBoard;
            if (board != null)
                board.OpenSettingsDialog();
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if (!base.GameObjectClickHandler(selectedObj))
                return false;

            if (selectedObj == _mainPageButton.gameObject)
            {
                OpenMainPage();
            }
            else if (selectedObj == _listLevelsPageButton.gameObject)
            {
                OpenListLevelsPage();
            }
            else if (selectedObj == _previousPageButton.gameObject)
            {
                OpenPreviousPage();
            }
            else if (selectedObj == _openSettingsDialogButton.gameObject)
            {
                OpenSettingsDialog();
            }
            else if (selectedObj == _stopButton.gameObject)
            {
                StopLevel();
            }

            return true;
        }
    }
}
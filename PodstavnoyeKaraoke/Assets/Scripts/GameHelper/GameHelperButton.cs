using System.Collections.Generic;
using Controllers;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace GameHelper
{
    [RequireComponent(typeof(Button))]
    public class GameHelperButton : Interactable
    {
        [SerializeField] private List<GameHelperInfo> _steps = new List<GameHelperInfo>();

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(ButtonPress);
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if (!base.GameObjectClickHandler(selectedObj))
                return false;
            
            if (selectedObj == _button.gameObject)
            {
                MainController.Instance.HelperController.StartGameHelper(_steps);
            }

            return true;
        }
    }
}
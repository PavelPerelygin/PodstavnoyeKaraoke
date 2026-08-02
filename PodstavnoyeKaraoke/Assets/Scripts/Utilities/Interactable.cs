using Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utilities
{
    public class Interactable : MonoBehaviour
    {
        protected float _ignoreTimeLeft = 0f;
    
        private GameObject _selectedGameObject;
    
        public float IgnoreTimeLeft => _ignoreTimeLeft;
    
        protected virtual void Update()
        {
            UpdateIgnorLeftTime();
            UpdateSelectedGameObject();
            KeyPressHandler();
        }
    
        private void UpdateIgnorLeftTime()
        {
            if(_ignoreTimeLeft <= 0)
                return;

            _ignoreTimeLeft -= Time.deltaTime;
        }
    
        private void UpdateSelectedGameObject()
        {
            GameObjectClickHandler(_selectedGameObject);
            _selectedGameObject = null;
        }

        protected virtual bool KeyPressHandler()
        {
            return true;
        }

        protected virtual bool GameObjectClickHandler(GameObject selectedObj)
        {
            if (selectedObj == null)
                return false;

            if (_ignoreTimeLeft > 0)
                return false;

            return true;
        }
    
        protected virtual void ButtonPress()
        {
            _selectedGameObject = EventSystem.current.currentSelectedGameObject;
        }
    
        protected virtual void TogglePress(bool state)
        {
            _selectedGameObject = EventSystem.current.currentSelectedGameObject;
        }

        protected virtual void SliderPress(float value)
        {
            _selectedGameObject = EventSystem.current.currentSelectedGameObject;
        }

    }
}

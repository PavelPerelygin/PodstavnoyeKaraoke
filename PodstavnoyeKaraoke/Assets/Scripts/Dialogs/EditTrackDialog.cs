using Dialogs.Base;
using Extensions;
using Managers.Settings.Local;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogs
{
    public class EditTrackDialog : Dialog
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private InputField _nameTrackInputField;

        private TrackData _trackData;
        
        public void Init(TrackData trackData)
        {
            _trackData = trackData;

            InitButtons();
            InitInputFields();
        }

        private void InitButtons()
        {
            _closeButton.onClick.AddListener(ButtonPress);
            _closeButton.DisableOverDownColors();
        }

        private void InitInputFields()
        {
            _nameTrackInputField.text = _trackData.GetNameTrack();
            _nameTrackInputField.onValueChanged.AddListener(value =>
            {
                _trackData.SetNameTrack(value);
            });
            _nameTrackInputField.DisableOverDownColors();
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if(!base.GameObjectClickHandler(selectedObj))
                return false;

            if (selectedObj == _closeButton.gameObject)
            {
                Hide();
            }
            
            return true;
        }
    }
}
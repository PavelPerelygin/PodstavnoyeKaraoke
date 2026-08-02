using Extensions;
using Managers.Settings.Local;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Game
{
    public class TrackItem : Interactable
    {
        [SerializeField] private Text _nameTrackText;
        [SerializeField] private Button _removeButton;
        
        
        private TrackData _trackData;

        public void Init(TrackData trackData)
        {
            _trackData = trackData;

            InitText();
            InitButton();
        }

        private void InitText()
        {
            _nameTrackText.text = _trackData.GetNameTrack();
        }

        private void InitButton()
        {
            _removeButton.onClick.AddListener(ButtonPress);
            _removeButton.DisableOverDownColors();
        }

        private void RemoveTrack()
        {
            
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if(!base.GameObjectClickHandler(selectedObj))
                return false;

            if (selectedObj == _removeButton.gameObject)
            {
                
            }
            
            return true;
        }
    }
}
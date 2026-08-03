using System;
using Extensions;
using Managers.Settings.Local;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Game
{
    public class TrackItem : Interactable
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _substarteImage;
        [SerializeField] private Text _nameTrackText;
        
        private TracksPanel _tracksPanel;
        
        public TrackData TrackData {get; private set;}

        public void Init(TrackData trackData, TracksPanel tracksPanel)
        {
            TrackData = trackData;
            _tracksPanel =  tracksPanel;

            UpdateName();
            
            InitButton();

            TrackData.OnChangeName += OnChangeName;
        }

        private void UpdateName()
        {
            _nameTrackText.text = TrackData.GetNameTrack();
        }

        private void InitButton()
        {
            _button.onClick.AddListener(ButtonPress);
            _button.DisableOverDownColors();
        }

        private void OnClick()
        {
            _tracksPanel.OnClickTrackItem(this);
        }

        public void SelectTrack()
        {
            _substarteImage.sprite = _tracksPanel.SelectedTrackSubstrate;
        }

        public void UnSelectTrack()
        {
            _substarteImage.sprite = _tracksPanel.UnSelectedTrackSubstrate;
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if(!base.GameObjectClickHandler(selectedObj))
                return false;

            if (selectedObj == _button.gameObject)
            {
                OnClick();
            }
            
            return true;
        }

        #region Events

        private void OnChangeName()
        {
            UpdateName();
        }

        private void OnDestroy()
        {
            TrackData.OnChangeName -= OnChangeName;
        }

        #endregion
    }
}
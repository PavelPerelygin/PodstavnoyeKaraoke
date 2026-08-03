using System.Collections.Generic;
using Boards;
using Controllers;
using Extensions;
using Managers.Settings.Local;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Game
{
    public class TracksPanel : Interactable
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Button _addTrackButton;
        [SerializeField] private Button _editTrackButton;
        [SerializeField] private Button _removeTrackButton;
        [SerializeField] private TrackItem _trackItemPrefab;
        
        private List<TrackItem> _trackItems = new List<TrackItem>();
        
        public MainBoard MainBoard {get; private set;}
        public TrackItem SelectedTrack {get; private set;}

        public void Init(MainBoard mainBoard)
        {
            MainBoard = mainBoard;

            InitButtons();

            BuildTrackItems();
        }

        private void InitButtons()
        {
            _addTrackButton.onClick.AddListener(ButtonPress);
            _addTrackButton.DisableOverDownColors();
            
            _editTrackButton.onClick.AddListener(ButtonPress);
            _editTrackButton.DisableOverDownColors();
            
            _removeTrackButton.onClick.AddListener(ButtonPress);
            _removeTrackButton.DisableOverDownColors();
        }
        
        private void BuildTrackItems()
        {
            var tracksData = MainController.Instance.LocalSettings.GetTracks();

            for (int i = 0; i < tracksData.Count; i++)
            {
                CreateTrackItem(tracksData[i]);
            }
        }
        
        private void CreateTrackItem(TrackData trackData)
        {
            var item = Instantiate(_trackItemPrefab, _scrollRect.content);
            item.Init(trackData, this);
            
            _trackItems.Add(item);
        }

        private void AddTrack()
        {
            MainController.Instance.LocalSettings.AddTrack(trackData =>
            {
                if(trackData == null)
                    return;
                
                CreateTrackItem(trackData);
            });
        }
        
        private void EditTrack()
        {
            if(SelectedTrack == null)
                return;
        }
        
        private void RemoveTrack()
        {
            if(SelectedTrack == null)
                return;
            
            MainController.Instance.LocalSettings.RemoveTrack(SelectedTrack.TrackData);
            
            if(_trackItems.Contains(SelectedTrack))
                _trackItems.Remove(SelectedTrack);
            
            Destroy(SelectedTrack.gameObject);
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if(!base.GameObjectClickHandler(selectedObj))
                return false;

            if (selectedObj == _addTrackButton.gameObject)
            {
                AddTrack();
            }
            else if (selectedObj == _editTrackButton.gameObject)
            {
                EditTrack();
            }
            else if (selectedObj ==  _removeTrackButton.gameObject)
            {
                RemoveTrack();
            }
            
            return true;
        }
    }
}
using System;
using System.Collections.Generic;
using Boards;
using Controllers;
using Dialogs;
using Dialogs.Base;
using Extensions;
using Managers.Settings.Local;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Game.TracksPanel
{
    public class TracksPanel : Interactable
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Button _addTrackButton;
        [SerializeField] private Button _editTrackButton;
        [SerializeField] private Button _removeTrackButton;
        [SerializeField] private TrackItem _trackItemPrefab;
        [SerializeField] private Sprite _selectedTrackSubstrate;
        [SerializeField] private Sprite _unSelectedTrackSubstrate;
        
        
        private List<TrackItem> _trackItems = new List<TrackItem>();
        
        public Sprite SelectedTrackSubstrate => _selectedTrackSubstrate;
        public Sprite UnSelectedTrackSubstrate => _unSelectedTrackSubstrate;
        public MainBoard MainBoard {get; private set;}
        public TrackItem SelectedTrack {get; private set;}

        public event Action OnChangeSelectedTrack;

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
            
            SortTrackItems();
        }
        
        private void CreateTrackItem(TrackData trackData)
        {
            var item = Instantiate(_trackItemPrefab, _scrollRect.content);
            item.Init(trackData, this);
            
            _trackItems.Add(item);
        }

        public void SortTrackItems()
        {
            _trackItems.Sort((firstItem, secondItem) =>
                string.Compare(
                    firstItem.TrackData.GetNameTrack(),
                    secondItem.TrackData.GetNameTrack(),
                    StringComparison.CurrentCultureIgnoreCase));

            for (int i = 0; i < _trackItems.Count; i++)
            {
                _trackItems[i].transform.SetSiblingIndex(i);
            }
        }

        public void OnClickTrackItem(TrackItem trackItem)
        {
            if(SelectedTrack == trackItem)
                return;
            
            SetSelectedTrackItem(trackItem);
        }

        private void SetSelectedTrackItem(TrackItem trackItem)
        {
            SelectedTrack = trackItem;
            
            for (int i = 0; i < _trackItems.Count; i++)
            {
                var item = _trackItems[i];
                
                if(item == SelectedTrack)
                    item.SelectTrack();
                else
                    item.UnSelectTrack();
            }

            OnChangeSelectedTrack?.Invoke();
        }

        private void AddTrack()
        {
            MainController.Instance.LocalSettings.AddTrack(trackData =>
            {
                if(trackData == null)
                    return;
                
                CreateTrackItem(trackData);
                SortTrackItems();
            });
        }
        
        private void EditTrack()
        {
            if(SelectedTrack == null)
                return;

            var dialog =
                MainController.Instance.DialogsController.CreateDialog(TypeDialog.EditTrack) as EditTrackDialog;
            if (dialog != null)
            {
                dialog.Init(SelectedTrack.TrackData);
                dialog.Show();
            }
        }
        
        private void RemoveTrack()
        {
            if(SelectedTrack == null)
                return;

            var text = $"{MainController.Instance.TextManager.GetText(1005)} \"{SelectedTrack.TrackData.GetNameTrack()}\"";
            
            MainController.Instance.DialogsController.OpenConfirmDialog(text, () =>
            {
                MainController.Instance.LocalSettings.RemoveTrack(SelectedTrack.TrackData);
            
                if(_trackItems.Contains(SelectedTrack))
                    _trackItems.Remove(SelectedTrack);
            
                Destroy(SelectedTrack.gameObject);
            
                SetSelectedTrackItem(null); 
            },null);
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

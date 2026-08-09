using System.Collections.Generic;
using Controllers;
using Managers.Audio;
using Managers.Settings.Local;
using UnityEngine;
using UnityEngine.UI;

namespace Game.PlayerPanel.RecordsPanel
{
    public class RecordsPanel : MonoBehaviour
    {
        [SerializeField] private RecordItem _recordItemPrefab;
        [SerializeField] private ScrollRect _scrollRect;

        private List<RecordItem> _recordItems = new List<RecordItem>();
        private global::Game.PlayerPanel.PlayerPanel _playerPanel;
        private RecordItem _playingRecordItem;
        private AudioInfo _playingRecordAudioInfo;

        public void Init(global::Game.PlayerPanel.PlayerPanel playerPanel)
        {
            _playerPanel = playerPanel;
        }

        public void BuildRecords(List<RecordData> records)
        {
            StopPlayingRecord();
            ClearRecordItems();

            for (int i = 0; i < records.Count; i++)
            {
                CreateRecordItem(records[i]);
            }
        }

        private void CreateRecordItem(RecordData recordData)
        {
            var recordItem = Instantiate(_recordItemPrefab, _scrollRect.content);
            recordItem.Init(recordData, this);

            _recordItems.Add(recordItem);
        }

        public void AddRecord(RecordData recordData)
        {
            CreateRecordItem(recordData);
        }

        public void PlayRecord(RecordItem recordItem)
        {
            if (recordItem == null || recordItem.RecordData == null || !recordItem.RecordData.IsExistRecord())
                return;

            _playerPanel.StopCurrentTrack();

            if (_playingRecordItem != recordItem)
            {
                StopPlayingRecord();
                _playingRecordItem = recordItem;
            }

            EnsurePlayingRecordAudioInfo(recordItem.RecordData);
            _playingRecordAudioInfo?.Play(false);
        }

        public void PauseRecord(RecordItem recordItem)
        {
            if (_playingRecordItem != recordItem || _playingRecordAudioInfo == null)
                return;

            _playingRecordAudioInfo.Pause();
        }

        public void StopRecord(RecordItem recordItem)
        {
            if (_playingRecordItem != recordItem)
                return;

            StopPlayingRecord();
        }

        public void StopPlayingRecord()
        {
            if (_playingRecordAudioInfo != null)
            {
                _playingRecordAudioInfo.Stop();
                _playingRecordAudioInfo.Remove();
                _playingRecordAudioInfo = null;
            }

            _playingRecordItem = null;
        }

        private void EnsurePlayingRecordAudioInfo(RecordData recordData)
        {
            if (_playingRecordAudioInfo != null &&
                MainController.Instance.AudioManager.CheckContainsAudioInfo(_playingRecordAudioInfo))
                return;

            _playingRecordAudioInfo = MainController.Instance.AudioManager
                .Create(recordData.GetPatchToRecord(), TypeGroup.Track, true)
                .OnCompleted(OnPlayingRecordCompleted);
        }

        private void OnPlayingRecordCompleted()
        {
            _playingRecordAudioInfo = null;
            _playingRecordItem = null;
        }

        private void ClearRecordItems()
        {
            for (int i = 0; i < _recordItems.Count; i++)
                Destroy(_recordItems[i].gameObject);

            _recordItems.Clear();
        }
    }
}

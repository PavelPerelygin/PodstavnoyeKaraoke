using System.Collections;
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
            StartCoroutine(ScrollToBottomAfterLayout());
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
            recordItem.SetDuration(GetPlayingRecordDuration());
            _playingRecordAudioInfo?.Play(false);
            _playingRecordAudioInfo?.SetProgress(recordItem.Progress);
            recordItem.SetPlaybackButtonsState(true);
        }

        public void PauseRecord(RecordItem recordItem)
        {
            if (_playingRecordItem != recordItem || _playingRecordAudioInfo == null)
                return;

            _playingRecordAudioInfo.Pause();
            recordItem.SetPlaybackButtonsState(false);
        }

        public void StopRecord(RecordItem recordItem)
        {
            if (_playingRecordItem != recordItem)
                return;

            StopPlayingRecord();
        }

        public void RemoveRecord(RecordItem recordItem)
        {
            if (recordItem == null || recordItem.RecordData == null)
                return;

            if (_playingRecordItem == recordItem)
                StopPlayingRecord();

            _playerPanel.RemoveRecord(recordItem.RecordData);
            _recordItems.Remove(recordItem);
            Destroy(recordItem.gameObject);
        }

        public string GetRecordSaveFileName(RecordData recordData)
        {
            return _playerPanel == null ? "" : _playerPanel.GetRecordSaveFileName(recordData);
        }

        public void SetRecordProgress(RecordItem recordItem, float value)
        {
            if (_playingRecordItem != recordItem || _playingRecordAudioInfo == null)
                return;

            _playingRecordAudioInfo.SetProgress(value);
        }

        public void StopPlayingRecord()
        {
            var recordItem = _playingRecordItem;

            if (_playingRecordAudioInfo != null)
            {
                _playingRecordAudioInfo.Stop();
                _playingRecordAudioInfo.Remove();
                _playingRecordAudioInfo = null;
            }

            _playingRecordItem = null;
            recordItem?.ResetProgress();
            recordItem?.SetPlaybackButtonsState(false);
        }

        private void EnsurePlayingRecordAudioInfo(RecordData recordData)
        {
            if (_playingRecordAudioInfo != null &&
                MainController.Instance.AudioManager.CheckContainsAudioInfo(_playingRecordAudioInfo))
                return;

            _playingRecordAudioInfo = MainController.Instance.AudioManager
                .Create(recordData.GetPatchToRecord(), TypeGroup.Record, true)
                .OnChangeProgress(OnPlayingRecordProgressChanged)
                .OnCompleted(OnPlayingRecordCompleted);
        }

        private float GetPlayingRecordDuration()
        {
            if (_playingRecordAudioInfo == null || _playingRecordAudioInfo.AudioClip == null)
                return 0f;

            return _playingRecordAudioInfo.GetAudioClipLenght();
        }

        private void OnPlayingRecordProgressChanged(float progress)
        {
            _playingRecordItem?.SetProgress(progress);
        }

        private void OnPlayingRecordCompleted()
        {
            _playingRecordItem?.ResetProgress();
            _playingRecordItem?.SetPlaybackButtonsState(false);
            _playingRecordAudioInfo = null;
            _playingRecordItem = null;
        }

        private void ClearRecordItems()
        {
            for (int i = 0; i < _recordItems.Count; i++)
                Destroy(_recordItems[i].gameObject);

            _recordItems.Clear();
        }

        private IEnumerator ScrollToBottomAfterLayout()
        {
            yield return null;
            Canvas.ForceUpdateCanvases();

            if (_scrollRect != null)
                _scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}

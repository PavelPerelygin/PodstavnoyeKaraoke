using System.Collections.Generic;
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
        
        public void BuildRecords(List<RecordData> records)
        {
            ClearRecordItems();

            for (int i = 0; i < records.Count; i++)
            {
                CreateRecordItem(records[i]);
            }
        }

        private void CreateRecordItem(RecordData recordData)
        {
            var recordItem = Instantiate(_recordItemPrefab, _scrollRect.content);
            recordItem.Init(recordData);
            
            _recordItems.Add(recordItem);
        }

        private void ClearRecordItems()
        {
            for (int i = 0; i < _recordItems.Count; i++)
                Destroy(_recordItems[i].gameObject);
            
            _recordItems.Clear();
        }
    }
}
using Managers.Settings.Local;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Game.PlayerPanel.RecordsPanel
{
    public class RecordItem : Interactable
    {
        [SerializeField] private Text _nameRecordText;
        [SerializeField] private Text _recordTimeText;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _stopButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _dowlandButton;
        
        public RecordData RecordData {get; private set;}
        
        public void Init (RecordData recordData)
        {
            RecordData = recordData;
        }
    }
}
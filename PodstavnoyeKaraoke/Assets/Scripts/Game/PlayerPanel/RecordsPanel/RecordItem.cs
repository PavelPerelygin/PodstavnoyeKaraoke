using Extensions;
using Managers.Settings.Local;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Game.PlayerPanel.RecordsPanel
{
    public class RecordItem : Interactable
    {
        [SerializeField] private Text _nameRecordText;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _stopButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _dowlandButton;
        
        private RecordsPanel _recordsPanel;

        public RecordData RecordData {get; private set;}
        
        public void Init (RecordData recordData, RecordsPanel recordsPanel)
        {
            RecordData = recordData;
            _recordsPanel = recordsPanel;
            _nameRecordText.text = RecordData.GetRecordName();

            InitButtons();
        }

        private void InitButtons()
        {
            _playButton.onClick.AddListener(ButtonPress);
            _playButton.DisableOverDownColors();
            
            _stopButton.onClick.AddListener(ButtonPress);
            _stopButton.DisableOverDownColors();
            
            _pauseButton.onClick.AddListener(ButtonPress);
            _pauseButton.DisableOverDownColors();
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if(!base.GameObjectClickHandler(selectedObj))
                return false;

            if (selectedObj == _playButton.gameObject)
            {
                _recordsPanel.PlayRecord(this);
            }
            else if (selectedObj == _pauseButton.gameObject)
            {
                _recordsPanel.PauseRecord(this);
            }
            else if (selectedObj == _stopButton.gameObject)
            {
                _recordsPanel.StopRecord(this);
            }
            
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using Controllers;

namespace Managers.Settings.Local
{
    [Serializable]
    public class PlayerData
    {
        public string namePlayer = "";
        public List<RecordData> records = new List<RecordData>();
        
        public event Action OnChangeName;
        
        public void SetNamePlayer(string value)
        {
            namePlayer = value;
            
            MainController.Instance.LocalSettings.Save();
            
            OnChangeName?.Invoke();
        }

        public string GetNamePlayer()
        {
            return namePlayer;
        }

        #region Records

        public List<RecordData> GetRecords()
        {
            return records;
        }

        public RecordData AddRecord(string recordName, string patchToRecord)
        {
            var recordData = new RecordData();
            recordData.SetRecordName(recordName);
            recordData.SetPatchToRecord(patchToRecord);

            records.Add(recordData);
            MainController.Instance.LocalSettings.Save();

            return recordData;
        }

        public void RemoveRecord(RecordData recordData)
        {
            if (!records.Contains(recordData))
                return;

            recordData.OnRemove();
            records.Remove(recordData);
            MainController.Instance.LocalSettings.Save();
        }

        #endregion

        #region Events

        public void OnRemove()
        {
            for (int i = 0; i < records.Count; i++)
            {
                records[i].OnRemove();
            }
        }

        #endregion
    }
}

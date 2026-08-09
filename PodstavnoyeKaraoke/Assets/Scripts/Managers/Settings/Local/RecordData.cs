using System;
using Controllers;
using Utilities.Files;

namespace Managers.Settings.Local
{
    [Serializable]
    public class RecordData
    {
        public string nameRecord = "";
        public string patchToRecord = "";

        public string GetRecordName()
        {
            return nameRecord;
        }

        public void SetRecordName(string value)
        {
            nameRecord = value;
            
            MainController.Instance.LocalSettings.Save();
        }
        
        public string GetPatchToRecord()
        {
            return patchToRecord;
        }

        public void SetPatchToRecord(string value)
        {
            patchToRecord = value;
            
            MainController.Instance.LocalSettings.Save();
        }

        public bool IsExistRecord()
        {
            if(patchToRecord == "")
                return false;
            
            return File.FileExistFromStreamingAssets(patchToRecord);
        }

        public void RemoveRecord()
        {
            if(!IsExistRecord())
                return;
            
            File.DeleteFiletFromStreamingAssets(patchToRecord);
            SetPatchToRecord("");
        }

        #region Events

        public void OnRemove()
        {
            RemoveRecord();
        }

        #endregion
    }
}
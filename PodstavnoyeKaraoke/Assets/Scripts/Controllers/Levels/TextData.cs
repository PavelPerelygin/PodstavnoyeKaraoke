using System;
using UnityEngine;

namespace Controllers.Levels
{
    [Serializable]
    public class TextData
    {
        public string textContent = "";
        public Vector3 position;
        
        #region Text content

        public string GetTextContent()
        {
            return textContent;
        }

        #endregion

        #region Position

        public Vector3 GetLocalPosition()
        {
            return position;
        }

        #endregion
    }
}
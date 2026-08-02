using System;
using UnityEngine;

namespace Controllers.Levels
{
    [Serializable]
    public class StarData
    {
        public Vector3 localPosition;
        
        #region Local position

        public Vector3 GetLocalPosition()
        {
            return localPosition;
        }

        #endregion
    }
}
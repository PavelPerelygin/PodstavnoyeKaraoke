using System;
using UnityEngine;

namespace Controllers.Levels
{
    [Serializable]
    public class ObstacleData
    {
        public string nameObstacle = "";
        public Vector3 localPosition;
        public Vector3 localScale = Vector3.one;
        public Vector3 angle;

        #region Name

        public string GetNameObstacle()
        {
            return nameObstacle;
        }

        #endregion

        #region Local position

        public Vector3 GetLocalPosition()
        {
            return localPosition;
        }

        #endregion

        #region Local scale

        public Vector3 GetLocalScale()
        {
            return localScale;
        }

        #endregion

        #region Angle

        public Vector3 GetAngle()
        {
            return angle;
        }

        #endregion
    }
}
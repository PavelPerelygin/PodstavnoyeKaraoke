using System;
using UnityEngine;

namespace Dialogs.Base
{
    [Serializable]
    public class VisualizationInfo
    {
        public TypeVisualization _type = TypeVisualization.Move;
        public LeanTweenType _tweenType = LeanTweenType.easeOutBack;
        public Vector2 _direction = Vector2.left;
        public float _time = 0.4f;

        public static VisualizationInfo ShowVisualization()
        {
            return new VisualizationInfo()
            {
                _type = TypeVisualization.Move,
                _tweenType = LeanTweenType.easeOutBack,
                _direction = Vector2.down,
                _time = 0.4f
            };
        }
        
        public static VisualizationInfo HideVisualization()
        {
            return new VisualizationInfo()
            {
                _type = TypeVisualization.Move,
                _tweenType = LeanTweenType.easeInBack,
                _direction = Vector2.up,
                _time = 0.4f
            };
        }
    }
}
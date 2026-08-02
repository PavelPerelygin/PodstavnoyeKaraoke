using System;
using UnityEngine;

namespace GameHelper
{
    [Serializable]
    public class GameHelperInfo
    {
        public GameObject _target = null;
        public int _headerId = 0;
        public int _messageId = 0;
        public Vector2 _direction = Vector2.left;
    }
}
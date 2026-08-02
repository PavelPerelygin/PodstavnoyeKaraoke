using UnityEngine;

namespace Boards.Base
{
    public class GameObjectInfo
    {
        public bool activeSelf;
        public Transform parent;
        public Vector3 position;
        public Vector3 localPosition;

        public GameObjectInfo(GameObject gameGameObject)
        {
            activeSelf = gameGameObject.activeSelf;
            parent = gameGameObject.transform.parent;
            position = gameGameObject.transform.position;
            localPosition = gameGameObject.transform.localPosition;
        }
    }
}

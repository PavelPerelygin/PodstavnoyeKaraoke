using System.Collections.Generic;
using UnityEngine;

namespace Controllers.GameChanges
{
    [CreateAssetMenu(fileName = "GameChanges", menuName = "Game/Game changes", order = 0)]
    public class GameChanges : ScriptableObject
    {
        [SerializeField] public List<GameChangesInfo> _data = new List<GameChangesInfo>();
    }
}
using Controllers.Levels;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Pages.Level.Items.Others
{
    public class Bomb : OtherItem
    {
        public BombData BombData { get; private set; }
        
        public void Init(Track track, BombData bombData)
        {
            base.Init(track);
            
            _track = track;
            
            BombData = bombData;
            
            SetLocalPosition(BombData.GetLocalPosition());
        }
    }
}
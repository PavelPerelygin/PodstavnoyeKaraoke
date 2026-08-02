using Controllers.Levels;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Pages.Level.Items.Others
{
    public class Star : OtherItem
    {
        public StarData StarData { get; private set; }
        
        public void Init(Track track, StarData starData)
        {
            base.Init(track);
            
            _track = track;
            
            StarData = starData;
            
            SetLocalPosition(StarData.GetLocalPosition());
        }
    }
}
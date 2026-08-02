using Controllers.Levels;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Pages.Level.Items.Others
{
    public class Gift : OtherItem
    {
        public GiftData GiftData { get; private set; }
        public void Init(Track track, GiftData giftData)
        {
            base.Init(track);
            
            _track = track;
            
            GiftData = giftData;
            
            SetLocalPosition(GiftData.GetLocalPosition());
        }
    }
}
using Controllers.Levels;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Pages.Level.Items.Others
{
    public class Ruby : OtherItem
    {
        public RubyData RubyData { get; private set; }
        public void Init(Track track, RubyData rubyData)
        {
            base.Init(track);
            
            _track = track;
            
            RubyData = rubyData;
            
            SetLocalPosition(RubyData.GetLocalPosition());
        }
    }
}
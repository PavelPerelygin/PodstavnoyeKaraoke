using Controllers.Levels;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Pages.Level.Items.Others
{
    public class Coin : OtherItem
    {
        public CoinData CoinData { get; private set; }
        public void Init(Track track, CoinData coinData)
        {
            base.Init(track);
            
            _track = track;
            
            CoinData = coinData;
            
            SetLocalPosition(CoinData.GetLocalPosition());
        }
    }
}
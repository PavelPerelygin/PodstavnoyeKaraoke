using Controllers;
using Controllers.Levels;
using Game.Pages.Common.SkinItem;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Level.Items.Obstacles
{
    public class Obstacle : GameFieldItem
    {
        [SerializeField] private RawImage _obstacleImage;
        [SerializeField] private ImageSkin _imageSkin;
        
        public ObstacleData ObstacleData { get; private set; }

        #region Init

        public void Init(Track track, ObstacleData obstacleData)
        {
            base.Init(track);
            
            ObstacleData = obstacleData;
            
            SetLocalPosition(ObstacleData.GetLocalPosition());
            SetScale(ObstacleData.GetLocalScale());
            SetAngle(ObstacleData.GetAngle());

            UpdateSkin();
        }

        #endregion
        
        private void SetAngle(Vector3 angle)
        {
            transform.eulerAngles = angle;
        }

        private void SetScale(Vector3 scale)
        {
            transform.localScale = scale;
        }
        
        protected override void SetLocalPosition(Vector3 position)
        {
            transform.localPosition = position;
        }

        public void UpdateSkin()
        {
            var nameResource = _imageSkin.GetNameResource();
            var sprite = MainController.Instance.SkinsController.GetSpriteByName(nameResource);
            _imageSkin.SetSprite(sprite);
        }
    }
}
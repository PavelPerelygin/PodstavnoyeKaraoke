using System;
using UnityEngine;

namespace Controllers.Skins.Default
{
    [Serializable]
    public class SpriteResource
    {
        public string resourceName = "";
        public Sprite resource;
        
        public string GetResourceName()
        {
            return resourceName;
        }

        public Sprite GetResource()
        {
            return resource;
        }

        public void UpdateName()
        {
            if(resource == null)
                return;
            
            if(resourceName == resource.name)
                return;
            
            resourceName = resource.name;
            
            DefaultSkin.Instance.Save();
        }
    }
}
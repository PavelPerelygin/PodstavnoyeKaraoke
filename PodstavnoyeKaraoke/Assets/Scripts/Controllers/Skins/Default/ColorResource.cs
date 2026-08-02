using System;
using UnityEngine;

namespace Controllers.Skins.Default
{
    [Serializable]
    public class ColorResource
    {
        public string resourceName = "";
        public Color resource;
        
        public string GetResourceName()
        {
            return resourceName;
        }

        public Color GetResource()
        {
            return resource;
        }
    }
}
using System;

namespace Controllers.Skins
{
    [Serializable]
    public class ColorResourceData
    {
        public TypeResource typeResource;
        public string nameResource = "";
        public string color = "#FFFFFF";

        #region Type

        public void SetTypeResource(TypeResource value)
        {
            typeResource = value;
        }

        public TypeResource GetTypeResource()
        {
            return typeResource;
        }

        #endregion

        #region Name

        public void SetNameResource(string value)
        {
            nameResource = value;
        }

        public string GetNameResource()
        {
            return nameResource;
        }

        #endregion

        #region Resource
        

        public void RemoveResource()
        {
            color = "#FFFFFF";
        }

        public string GetResource()
        {
            return color;
        }

        #endregion
    }
}
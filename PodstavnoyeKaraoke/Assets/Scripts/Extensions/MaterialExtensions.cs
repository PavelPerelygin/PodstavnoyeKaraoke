using UnityEngine;

namespace Extensions
{
    public class MaterialExtensions
    {
        public static Material CreateMaterial(string nameShader)
        {
            Shader shader = Shader.Find(nameShader);
            if (shader == null)
                return null;

            return new Material(shader);
        }
    }
}
using System;
using System.Collections.Generic;

namespace Extensions
{
    public static class EnumExtensions
    {
        public static T ParseEnum<T>(string value)
        {
            return (T) Enum.Parse(typeof(T), value, true);
        }
        
        public static T GetValueEnumByIndex<T>(int index)
        {
            return (T)Enum.GetValues(typeof(T)).GetValue(index);
        }

        public static List<T> GetNames<T>()
        {
            var names = Enum.GetNames(typeof(T));
            List<T> result = new List<T>();
            
            for (int i = 0; i < names.Length; i++)
            {
                result.Add(ParseEnum<T>(names[i]));
            }

            return result;
        }

        public static T GetRandom<T>()
        {
            var names =GetNames<T>();
            var random = names[UnityEngine.Random.Range(0, names.Count)];

            return random;
        }
    }
}
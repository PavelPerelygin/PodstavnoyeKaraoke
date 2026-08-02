using UnityEngine;

namespace Utilities
{
    public static class Log
    {
        public static void Assert()
        {
            Debug.Log("<color=red>Assert!</color>");
        }
        public static void Assert(string message = "")
        {
            Debug.Log($"{message}");
        }

        public static void Message(string message)
        {
            Debug.Log($"{message}"); 
        }
    }
}
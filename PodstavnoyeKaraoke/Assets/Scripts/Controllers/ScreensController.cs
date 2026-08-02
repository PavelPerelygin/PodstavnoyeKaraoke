using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using Utilities;
using Debug = UnityEngine.Debug;

namespace Controllers
{
    public class ScreensController
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(uint hwnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern uint GetActiveWindow();
        
        [DllImport("user32.dll", SetLastError=true)]
        static extern uint GetWindowThreadProcessId(uint hWnd, out uint processId);
        
        private delegate bool EnumWindowsProc(uint hWnd, uint lParam);
 
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EnumWindows(EnumWindowsProc callback, IntPtr extraData);
#endif

        public void MinimizeSelectedScreen()
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            ShowWindow(GetActiveWindow(), 2);
#endif
        }

        public bool CheckAvailableSecondScreen()
        {
#if UNITY_EDITOR
            return true;  
#endif
            if (Display.displays.Length < 2)
                return false;

            return true;
        }

        public bool IsEnableSecondScreen()
        {
#if UNITY_EDITOR
            return false;  
#endif
            if (!CheckAvailableSecondScreen())
                return false;

            return Display.displays[1].active;
        }

        public void EnableSecondScreen()
        {
#if UNITY_EDITOR
            return;  
#endif
            if (!CheckAvailableSecondScreen())
            {
                Log.Assert();
                return;
            }
            
            Display.displays[1].Activate();
        }

        public void ChangeFullScreen()
        {
#if UNITY_EDITOR
            return;  
#endif
            Screen.fullScreen = !Screen.fullScreen;
        }

        public bool IsFullScreen()
        {
#if UNITY_EDITOR
            return false;  
#endif
            return Screen.fullScreen;
        }
        
    }
}
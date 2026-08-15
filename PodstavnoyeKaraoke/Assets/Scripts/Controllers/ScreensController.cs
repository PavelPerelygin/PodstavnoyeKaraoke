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
        private const int MinWindowWidth = 640;
        private const int MinWindowHeight = 360;
        private const int MaxWindowWidth = 7680;
        private const int MaxWindowHeight = 4320;
        private const int DefaultWindowWidth = 1920;
        private const int DefaultWindowHeight = 1080;
        private const int AspectWidth = 16;
        private const int AspectHeight = 9;

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

        public void Init()
        {
            if (IsAspectRatioValid(Screen.width, Screen.height))
                return;

            SetWindowSize(DefaultWindowWidth, DefaultWindowHeight);
        }

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

        public Vector2Int SetWindowSizeByWidth(int width)
        {
            width = Mathf.Clamp(width, MinWindowWidth, MaxWindowWidth);
            var height = Mathf.RoundToInt(width * AspectHeight / (float)AspectWidth);

            return SetWindowSize(width, height);
        }

        public Vector2Int SetWindowSizeByHeight(int height)
        {
            height = Mathf.Clamp(height, MinWindowHeight, MaxWindowHeight);
            var width = Mathf.RoundToInt(height * AspectWidth / (float)AspectHeight);

            return SetWindowSize(width, height);
        }

        private Vector2Int SetWindowSize(int width, int height)
        {
            width = Mathf.Clamp(width, MinWindowWidth, MaxWindowWidth);
            height = Mathf.Clamp(height, MinWindowHeight, MaxWindowHeight);

            Screen.SetResolution(width, height, FullScreenMode.Windowed);
            return new Vector2Int(width, height);
        }

        private bool IsAspectRatioValid(int width, int height)
        {
            return width * AspectHeight == height * AspectWidth;
        }
        
    }
}

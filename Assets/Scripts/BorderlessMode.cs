// using System;
// using System.Collections;
// using System.Runtime.InteropServices;
// using System.Diagnostics;
// using UnityEngine;

// /// <summary>
// /// Allow a standalone windows application to run without border at a specific screen position or in fullscreen.
// /// You can also use the command argument -popupwindow in order to do the same thing with less control.
// /// This allows you to run other program over your unity application (as by example a virtual keyboard).
// /// </summary>
// [ExecuteInEditMode]
// public class BorderlessMode : MonoBehaviour
// {
    
//     public Rect ScreenPosition;
//     public bool IsFullscreen = false;

// #if UNITY_STANDALONE_WIN && !UNITY_EDITOR
// 	[DllImport("user32.dll")]
// 	static extern IntPtr SetWindowLong (IntPtr hwnd, int  _nIndex, int  dwNewLong);

// 	[DllImport("user32.dll")]
// 	static extern bool SetWindowPos (IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

// 	[DllImport("user32.dll")]
// 	static extern IntPtr GetForegroundWindow ();
 
// 	const uint SWP_SHOWWINDOW = 0x0040;
// 	const int GWL_STYLE = -16;
// 	const int WS_BORDER = 1;
 
// 	void Start ()
// 	{
// 		if(IsFullscreen)
// 			ScreenPosition = GetFullscreenResolution();
// 		SetWindowLong (GetForegroundWindow (), GWL_STYLE, WS_BORDER);
// 		bool result = SetWindowPos (GetForegroundWindow (), 0, (int)ScreenPosition.x, (int)ScreenPosition.y, (int)ScreenPosition.width, (int)ScreenPosition.height, SWP_SHOWWINDOW);
// 	}
// #endif

// #if UNITY_EDITOR
//     void Update()
//     {
//         if (IsFullscreen)
//             ScreenPosition = GetFullscreenResolution();
//     }
// #endif

//     Rect GetFullscreenResolution()
//     {
//         Resolution resolution = Screen.currentResolution;
//         return new Rect(0f, 0f, (float)resolution.width, (float)resolution.height);
//     }

// }
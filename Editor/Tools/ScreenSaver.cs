using System;
using System.IO;
using UnityEngine;

namespace GameKit.Editor.Tools
{
    public class ScreenSaver
    {
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            Loop.EventLateUpdate += LoopOnEventLateUpdate;
        }

        private static void LoopOnEventLateUpdate()
        {
            if (Input.GetKey(KeyCode.Print))
            {
                SaveScreen();
            }
        }
        
        private static void SaveScreen()
        {
            var d = DateTime.Now;

            var n = Screen.width + "x" + Screen.height + "_" + DateTime.Now.ToFileTime(); //d.ToString("yyMMddhhmmss");

            if (!Directory.Exists(Application.dataPath.Remove(Application.dataPath.Length - 6, 6) + "Screens"))
                Directory.CreateDirectory(Application.dataPath.Remove(Application.dataPath.Length - 6, 6) + "Screens");
            ScreenCapture.CaptureScreenshot(Application.dataPath.Remove(Application.dataPath.Length - 6, 6) +
                                            "Screens/screen_" + n + ".png");
        }
    }
}
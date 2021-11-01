#pragma warning disable 649
#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine.Networking;

namespace GameKit.Editor
{
    public class Tools
    {
        [MenuItem("Tools/Open persistent folder")]
        public static void OpenPersistentFolder()
        {
            Application.OpenURL(Application.persistentDataPath);
        }
    }
}

#endif
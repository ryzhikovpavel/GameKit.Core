#pragma warning disable 649
using UnityEditor;
using UnityEngine;

namespace GameKit.Editor.Tools
{
    public class FolderHelper
    {
        [MenuItem("GameKit/Folders/Open persistent folder")]
        public static void OpenPersistentFolder()
        {
            Application.OpenURL(Application.persistentDataPath);
        }
    }
}
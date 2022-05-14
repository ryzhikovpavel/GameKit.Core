using GameKit.Implementation;
using UnityEngine;

namespace GameKit
{
    public class SessionGroups
    {
        public readonly ISessionGroup UnityPlayerPrefs = new SessionPlayerPrefsGroup();
        public readonly ISessionGroup DefaultFilePrefs = new SessionFilePrefsGroup(Application.persistentDataPath, "default session");
    }
}
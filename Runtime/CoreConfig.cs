using UnityEngine;

namespace GameKit
{
    [CreateAssetMenu(fileName = "CoreConfig", menuName = "GameKit/CoreConfig", order = 0)]
    public class CoreConfig : ScriptableObject
    {
        private static CoreConfig _instance;
        public static CoreConfig Instance => _instance ??= Resources.Load<CoreConfig>("CoreConfig");


        public long appleAppId;
    }
}
using UnityEngine;

namespace GameKit.Core
{
    [CreateAssetMenu(fileName = "CoreConfig", menuName = "GameKit/CoreConfig", order = 0)]
    public partial class CoreConfig : ScriptableObject
    {
        private static CoreConfig _instance;
        protected static CoreConfig Instance => _instance ??= Resources.Load<CoreConfig>("CoreConfig");
    }
}
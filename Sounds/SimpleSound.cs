using UnityEngine;

namespace GameKit.Core.Sounds
{
    [CreateAssetMenu(fileName = "Sound", menuName = "GameKit/Sounds/Simple", order = 0)]
    public class SimpleSound : Sound
    {
        [SerializeField] private AudioClip clip;
        protected override AudioClip GetClip() => clip;
    }
}
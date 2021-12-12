using UnityEngine;

namespace GameKit.Implementation
{
    [CreateAssetMenu(fileName = "Sound", menuName = "GameKit/Audio/Simple", order = 0)]
    public class SimpleSound : Sound
    {
        [SerializeField] private AudioClip clip;
        protected override AudioClip GetClip() => clip;
    }
}
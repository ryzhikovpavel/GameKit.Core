using UnityEngine;

namespace GameKit.Core.Sounds.Implementation
{
    [CreateAssetMenu(fileName = "Sound", menuName = "GameKit/Sounds/Random", order = 0)]
    public class RandomSound : Sound
    {
        [SerializeField] private AudioClip[] clip;
        protected override AudioClip GetClip() => clip.GetRandom();
    }
}
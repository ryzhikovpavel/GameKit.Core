using UnityEngine;

namespace GameKit.Implementation
{
    [CreateAssetMenu(fileName = "Sound", menuName = "GameKit/Audio/Random", order = 2)]
    public class RandomSound : Sound
    {
        [SerializeField] private AudioClip[] clip;
        protected override AudioClip GetClip() => clip.GetRandom();
    }
}
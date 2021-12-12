using UnityEngine;

namespace GameKit.Implementation
{
    [CreateAssetMenu(fileName = "Music", menuName = "GameKit/Audio/Music", order = 0)]
    public class Music : Sound
    {
        [SerializeField] private AudioClip[] melodies;
        private int _index;

        protected override AudioClip GetClip()
        {
            if (_index == melodies.Length) _index = 0;
            return melodies[_index++];
        } 
    }
}
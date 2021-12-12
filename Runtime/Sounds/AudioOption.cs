using System;
using UnityEngine;

namespace GameKit
{
    [Serializable]
    public class AudioOption
    {
        [SerializeField] private bool[] channelMuted;

        public bool IsMuted(AudioChannel channel)
        {
            var index = (int)channel;
            if (index >= channelMuted.Length)
                Array.Resize(ref channelMuted, index + 1);
            return channelMuted[index];
        }
        
        public void ChangeMute(AudioChannel channel, bool value)
        {
            var index = (int)channel;
            if (index >= channelMuted.Length)
                Array.Resize(ref channelMuted, index + 1);
            channelMuted[index] = value;
        }

        public AudioOption()
        {
            channelMuted = new bool[Enum.GetValues(typeof(AudioChannel)).Length];
        }
    }
}
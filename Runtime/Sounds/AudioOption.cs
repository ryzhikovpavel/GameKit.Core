using System;
using UnityEngine;

namespace GameKit
{
    [Serializable]
    public struct AudioOptions
    {
        private static readonly int AudioChannelCount = Enum.GetValues(typeof(AudioChannel)).Length;

        [SerializeField] private bool[] channelMuted;

        public bool IsMuted(AudioChannel channel)
        {
            CheckAndFixChannelArray();
            return channelMuted[(int)channel];
        }
        
        public void ChangeMute(AudioChannel channel, bool value)
        {
            CheckAndFixChannelArray();
            channelMuted[(int)channel] = value;
        }

        private void CheckAndFixChannelArray()
        {
            if (channelMuted is null)
            {
                channelMuted = new bool[AudioChannelCount];
                return;
            }

            if (channelMuted.Length != AudioChannelCount)
            {
                Array.Resize(ref channelMuted, AudioChannelCount);
                return;
            }
        }
    }
}
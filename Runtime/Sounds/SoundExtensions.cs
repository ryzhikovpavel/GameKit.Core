using UnityEngine;

namespace GameKit
{
    public static class SoundExtensions
    {
        public static AudioInstance Play(this AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogWarning("Sound file not found");
                return null;
            }
            return Service<GameAudio>.Instance.Play(clip);
        }

        public static bool IsPlaying(this AudioClip clip)
        {
            if (clip == null) return false;
            return Service<GameAudio>.Instance.IsPlaying(clip);
        }

        public static void Stop(this AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogWarning("Sound file not found");
                return;
            }
            Service<GameAudio>.Instance.Stop(clip);
        }
    }
}
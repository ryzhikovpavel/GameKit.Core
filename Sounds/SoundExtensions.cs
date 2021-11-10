using System;
using GameKit.Sounds;
using UnityEngine;

namespace GameKit
{
    public static class SoundExtensions
    {
        public static void Play(this AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogWarning("Sound file not found");
                return;
            }
            Service<GameAudio>.Instance.Play(clip);
        }

        public static void Play(this AudioClip clip, Action onComplete)
        {
            if (clip == null)
            {
                Debug.LogWarning("Sound file not found");
                onComplete?.Invoke();
                return;
            }
            Service<GameAudio>.Instance.Play(clip, onComplete);
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
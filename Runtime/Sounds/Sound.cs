using System;
using UnityEngine;
using UnityEngine.Audio;

namespace GameKit
{
    public abstract class Sound : ScriptableObject
    {
        [SerializeField] private AudioMixerGroup mixer;
        [SerializeField, Range(0, 1)] private float volumeSuppression;  
        protected abstract AudioClip GetClip();

        public void Play() => Play(null);
        public void Play(Action onComplete)
        {
            var ai = PlayWithOption();
            if (onComplete == null) return;
            if (ai is null) onComplete();
            else ai.EventCompleted(onComplete);
        }

        public AudioInstance PlayWithOption()
        {
            var clip = GetClip();
            if (clip == null)
            {
                Debug.LogWarning("Sound file not found");
                return null;
            }

            return Service<GameAudio>.Instance
                .Play(clip)
                .SetMixer(mixer)
                .SetVolume(1 - volumeSuppression);
        }
    }
}
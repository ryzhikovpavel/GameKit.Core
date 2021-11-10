using System;
using GameKit.Sounds;
using UnityEngine;
using UnityEngine.Audio;

namespace GameKit
{
    public abstract class Sound : ScriptableObject
    {
        [SerializeField] private AudioMixerGroup mixer;
        protected abstract AudioClip GetClip();
        

        public void Play() => Play(null);
        public void Play(Action onComplete)
        {
            var clip = GetClip();
            if (clip == null)
            {
                Debug.LogWarning("Sound file not found");
                onComplete?.Invoke();
                return;
            }
            Service<GameAudio>.Instance.Play(clip, mixer, false, onComplete);
        }
    }
}
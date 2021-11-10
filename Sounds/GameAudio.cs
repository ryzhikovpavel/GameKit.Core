using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

namespace GameKit.Sounds
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class GameAudio
    {
        private readonly Dictionary<AudioSource, IEnumerator> _routines = new Dictionary<AudioSource, IEnumerator>(10);
        private readonly List<AudioSource> _sources = new List<AudioSource>(10);
        private readonly GameObject _object;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (Service<GameAudio>.IsInstantiated == false)
                Service<GameAudio>.Instantiate();
        }
        
        public GameAudio()
        {
            _object = new GameObject("[Sounds]");
            Object.DontDestroyOnLoad(_object);
        }

        public void Play(AudioClip clip) => Play(clip, null, false, null);
        public void Play(AudioClip clip, Action onComplete) => Play(clip, null, false, onComplete);
        public void Play(AudioClip clip, AudioMixerGroup mixer, bool loop, Action onComplete)
        {
            if (clip == null)
            {
                Debug.LogWarning($"Clip is null");
                onComplete?.Invoke();
                return;
            }
            var s = GetSource();
            s.outputAudioMixerGroup = mixer;
            s.clip = clip;
            s.loop = loop;
            s.Play();
            if (onComplete != null)
            {
                var r= AwaitPlay(s, onComplete);
                _routines[s] = r;
                Loop.StartCoroutine(r);
            }
        }
        
        public bool IsPlaying(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogWarning($"Clip is null");
                return false;
            }

            foreach (var source in _sources)
            {
                if (source.clip == clip && source.isPlaying) return true;
            }

            return false;
        }
        
        public void Stop(AudioClip clip)
        {
            foreach (var channel in _sources)
            {
                if (channel.clip == clip && channel.isPlaying)
                {
                    channel.Stop();
                    channel.loop = false;
                    channel.clip = null;
                    if (_routines.TryGetValue(channel, out var r))
                    {
                        Loop.StopCoroutine(r);
                        _routines.Remove(channel);
                    }
                }
            }
        }

        public void StopAll()
        {
            foreach (var channel in _sources)
            {
                if (channel.isPlaying)
                {
                    channel.Stop();
                    channel.clip = null;
                }
            }

            foreach (var r in _routines.Values)
            {
                Loop.StopCoroutine(r);
            }
            _routines.Clear();
        }

        private IEnumerator AwaitPlay(AudioSource source, Action completed)
        {
            while (source.isPlaying == false) yield return null;
            while (source.isPlaying) yield return null;
            source.clip = null;
            completed();
        }

        private AudioSource GetSource()
        {
            foreach (var s in _sources)
            {
                if (s.isPlaying == true) continue;
                if (s.time > 0) continue;
                return s;
            }
            
            var source = _object.AddComponent<AudioSource>();
            _sources.Add(source);
            source.playOnAwake = false;
            return source;
        }
    }
}
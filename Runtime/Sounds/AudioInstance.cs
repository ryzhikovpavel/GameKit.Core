using System;
using System.Collections;
using GameKit.Implementation;
using UnityEngine;
using UnityEngine.Audio;

namespace GameKit
{
    public class AudioInstance: IPoolEntity
    {
        private readonly WaitForEndOfFrame _endFrame = new WaitForEndOfFrame();
        private readonly AudioSource _source;
        private IEnumerator _runnable;
        private Action _completed;
        private float _delay;
        private float _fadeDuration;

        public bool IsPlaying => _runnable is null == false; 
        public AudioClip Clip => _source.clip;
        public AudioMixerGroup Mixer => _source.outputAudioMixerGroup;
        public bool Loop => _source.loop;
        public float Duration => Clip.length + _delay;
        
        public AudioChannel Channel { get; private set; }

        internal AudioInstance(AudioSource source)
        {
            _source = source;
        }
        
        public AudioInstance SetMixer(AudioMixerGroup mixer)
        {
            _source.outputAudioMixerGroup = mixer;
            return this;
        }

        public AudioInstance SetVolume(float volume)
        {
            _source.volume = volume;
            return this;
        }
        
        public AudioInstance SetLoop(bool loop)
        {
            _source.loop = loop;
            return this;
        }

        public AudioInstance SetChannel(AudioChannel channel)
        {
            Channel = channel;
            return this;
        }

        public AudioInstance SetDelay(float delay)
        {
            _delay = delay;
            return this;
        }

        public AudioInstance SetFade(float duration)
        {
            _fadeDuration = duration;
            return this;
        }
        
        public AudioInstance EventCompleted(Action completed)
        {
            if (completed is null) return this;
            _completed += completed;
            return this;
        }

        internal AudioInstance Play(AudioClip clip)
        {
            _source.gameObject.SetActive(true);
            _source.clip = clip;
#if UNITY_EDITOR
            _source.gameObject.name = clip.name;
#endif
            GameKit.Loop.StartCoroutine(_runnable = PlayAndWaitComplete());
            return this;
        }

        internal AudioInstance SetMute(bool mute)
        {
            _source.mute = mute;
            return this;
        }

        private IEnumerator PlayAndWaitComplete()
        {
            yield return _endFrame;
            if (_delay > 0) yield return new WaitForSeconds(_delay);
            _source.mute = Service<GameAudio>.Instance.IsMuted(Channel);
            _source.Play();
            if (_fadeDuration > 0) yield return Fade(0, _source.volume, _fadeDuration);
            float duration = Clip.length - _fadeDuration;
            yield return new WaitForSeconds(duration);
            // while (_source.isPlaying == false) yield return null;
            // while (_source.isPlaying) yield return null;
            if (_fadeDuration > 0) yield return Fade(_source.volume, 0, _fadeDuration);
            _runnable = null;
            Complete();
            Service<GameAudio>.Instance.ReleaseAudioInstance(this);
        }

        private IEnumerator Fade(float from, float to, float duration)
        {
            _source.volume = from;
            float time = 0;
            while (time < duration)
            {
                yield return null;
                time += Time.deltaTime;
                _source.volume = Mathf.Lerp(from, to, time / duration);
            }
            _source.volume = to;
        }

        private IEnumerator Stopping(float fadeDuration)
        {
            if (_source.time + fadeDuration > _source.clip.length) fadeDuration = _source.clip.length - _source.time;
            yield return Fade(_source.volume, 0, fadeDuration);
            Stop();
        }

        internal void FadeAndStop(float fadeDuration)
        {
            if (_runnable is null == false)
            {
                GameKit.Loop.StopCoroutine(_runnable);
                _runnable = null;
            }
            GameKit.Loop.StartCoroutine(Stopping(fadeDuration));
        }
        
        internal void Stop()
        {
            if (_runnable is null == false)
            {
                GameKit.Loop.StopCoroutine(_runnable);
                _runnable = null;
            }
            _source.Stop();
            Complete();
            Service<GameAudio>.Instance.ReleaseAudioInstance(this);
        }
        
        private void Reset()
        {
            if (_runnable is null == false)
            {
                GameKit.Loop.StopCoroutine(_runnable);
                _runnable = null;
            }
            if (_source.isPlaying) _source.Stop();
            _source.gameObject.SetActive(false);
            _source.outputAudioMixerGroup = null;
            _source.clip = null;
            _source.loop = false;
            _source.volume = 1;
            _completed = null;
            _delay = 0;
            Channel = 0;
            
#if UNITY_EDITOR
            _source.gameObject.name = "empty";
#endif
        }
        
        private void Complete()
        {
            if (_completed is null) return;
            try
            {
                _completed();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        int IPoolEntity.Id { get; set; }
        IPoolContainer IPoolEntity.Owner { get; set; }
        void IPoolEntity.Reset() => Reset();
    }
}
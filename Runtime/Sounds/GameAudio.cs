using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameKit
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class GameAudio
    {
        private readonly Pool<AudioInstance> _instances;
        private readonly Transform _root;
        private readonly ISession<AudioOptions> _session = Session.Resolve<AudioOptions>(Session.Groups.UnityPlayerPrefs);

        public event Action EventChannelMutedChanged;
        

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (Service<GameAudio>.IsInstantiated == false)
                Service<GameAudio>.Instantiate();
        }
        
        public GameAudio()
        {
            var obj = new GameObject("[Sounds]");
            Object.DontDestroyOnLoad(obj);
            _root = obj.transform;
            _instances = Pool.Build(CreateInstance);
            _instances.Initialize(25);
        }

        public bool IsMuted(AudioChannel channel) => _session.Get().IsMuted(channel);

        public void SetMute(AudioChannel channel, bool mute)
        {
            var options = _session.Get();
            options.ChangeMute(channel, mute);
            _session.Save(options);
            foreach (var instance in _instances)
                if (instance.Channel == channel) instance.SetMute(mute);

            if (EventChannelMutedChanged is null == false) EventChannelMutedChanged();
        }

        public AudioInstance Play(AudioClip clip) => _instances.Pull().Play(clip);

        public bool IsPlaying(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogWarning($"Clip is null");
                return false;
            }

            foreach (var instance in _instances)
                if (instance.Clip == clip) return true;
            
            return false;
        }
        
        public void Stop(AudioClip clip, float fadeDuration = 0)
        {
            foreach (var instance in _instances)
                if (instance.Clip == clip)
                    Stop(instance, fadeDuration);
        }
        
        public void StopChannel(AudioChannel channel, float fadeDuration = 0)
        {
            foreach (var instance in _instances)
                if (instance.Channel.Equals(channel)) 
                    Stop(instance, fadeDuration);
        }
        
        public void StopAll()
        {
            foreach (var instance in _instances)
                Stop(instance, 0);
        }

        private void Stop(AudioInstance instance, float fadeDuration)
        {
            if (instance.IsPlaying == false) return;
            if (fadeDuration > 0) instance.FadeAndStop(fadeDuration);
            else instance.Stop();
        }

        internal void ReleaseAudioInstance(AudioInstance instance) => _instances.Push(instance);
        
        private AudioInstance CreateInstance()
        {
            var obj = new GameObject();
            obj.transform.SetParent(_root, false);
            var source = obj.AddComponent<AudioSource>();
            source.playOnAwake = false;
#if UNITY_EDITOR
            source.gameObject.name = "empty";
#endif
            return new AudioInstance(source);
        }
    }
}
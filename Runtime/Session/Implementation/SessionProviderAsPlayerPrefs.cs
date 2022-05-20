using System.Text;
using UnityEngine;

namespace GameKit.Implementation
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class SessionPlayerPrefsGroup : ISessionGroup
    {
        public bool IsDirty { get; private set; }

        public T Load<T>(string name)
        {
            return JsonUtility.FromJson<T>(PlayerPrefs.GetString(name, "{}"));
        }

        public void Save<T>(string name, ref T data)
        {
            PlayerPrefs.SetString(name, JsonUtility.ToJson(data));
            MarkDirty();
        }

        public void Remove(string name)
        {
            if (PlayerPrefs.HasKey(name))
            {
                PlayerPrefs.DeleteKey(name);
                MarkDirty();
            }
        }

        public bool Contains(string name)
        {
            return PlayerPrefs.HasKey(name);
        }

        private void MarkDirty()
        {
            if (IsDirty == false)
            {
                Loop.EventEndFrame += Flush;
                IsDirty = true;
            }
        }

        public void Flush()
        {
            PlayerPrefs.Save();
            Loop.EventEndFrame -= Flush;
            IsDirty = false;
        }
    }
}
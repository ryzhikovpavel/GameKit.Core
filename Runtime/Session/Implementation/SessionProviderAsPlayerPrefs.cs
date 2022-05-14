using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace GameKit.Implementation
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class SessionPlayerPrefsGroup : ISessionGroup
    {
        private bool _isDirty;
        
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

        private void MarkDirty()
        {
            if (_isDirty == false)
            {
                Loop.EventEndFrame += Flush;
                _isDirty = true;
            }
        }

        private void Flush()
        {
            PlayerPrefs.Save();
            Loop.EventEndFrame -= Flush;
            _isDirty = false;
        }
    }

    public class SessionFilePrefsGroup : ISessionGroup
    {
        private List<string> _indexes;
        private List<string> _lines;
        private string _filePath;
        private bool _isDirty;
        
        public SessionFilePrefsGroup(string fileName) : this (Application.persistentDataPath, fileName) {}
        public SessionFilePrefsGroup(string directoryName, string fileName)
        {
            if (Directory.Exists(directoryName) == false) 
                Directory.CreateDirectory(directoryName);
            
            _filePath = Path.Combine(directoryName, fileName);
            
            if (File.Exists(_filePath) == false)
                File.WriteAllText(_filePath, "");
        }
        
        public T Load<T>(string name)
        {
            var index = _indexes.IndexOf(name);
            if (index < 0) return default;
            return JsonUtility.FromJson<T>(_lines[index + 1]);
        }

        public void Save<T>(string name, ref T data)
        {
            var index = _indexes.IndexOf(name);
            if (index < 0)
            {
                index = _indexes.Count;
                _indexes.Add(name);
                _lines.Add("");
                _lines[0] = _lines[0] + name + "\0";
            }
            _lines[index + 1] = JsonUtility.ToJson(data);
            MarkDirty();
        }

        public void Remove(string name)
        {
            var index = _indexes.IndexOf(name);
            if (index < 0) return;

            _lines[0] = _lines[0].Replace(name + "\0", "");
            _lines.RemoveAt(index + 1);
            _indexes.RemoveAt(index);
            MarkDirty();
        }

        private void MarkDirty()
        {
            if (_isDirty == false)
            {
                _isDirty = true;
                Loop.EventEndFrame += Flush;
            }
        }

        private void ReadFile()
        {
            _lines = new List<string>(File.ReadAllLines(_filePath));
            if (_lines.Count == 0)
            {
                _indexes = new List<string>();
                return;
            }
            
            _indexes = new List<string>(_lines[0].Split('\0', StringSplitOptions.RemoveEmptyEntries));
        }

        private void Flush()
        {
            _isDirty = false;
            Loop.EventEndFrame -= Flush;
            File.WriteAllLines(_filePath, _lines);
        }
    }
}
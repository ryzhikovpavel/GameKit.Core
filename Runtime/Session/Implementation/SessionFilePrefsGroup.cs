using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameKit.Implementation
{
    public class SessionFilePrefsGroup : ISessionGroup
    {
        private readonly string _filePath;
        private readonly bool _autoFlush;
        private List<string> _indexes;
        private List<string> _lines;
        
        public bool IsDirty { get; private set; }

        public SessionFilePrefsGroup(string directoryName, string fileName, bool autoFlush)
        {
            if (Directory.Exists(directoryName) == false) 
                Directory.CreateDirectory(directoryName);
            
            _filePath = Path.Combine(directoryName, fileName);
            
            if (File.Exists(_filePath) == false)
                File.WriteAllText(_filePath, "");

            _autoFlush = autoFlush;
        }

        public T Load<T>(string name)
        {
            if (_indexes is null) ReadFile();
            var index = _indexes.IndexOf(name);
            if (index < 0) return default;
            return JsonUtility.FromJson<T>(_lines[index + 1]);
        }

        public void Save<T>(string name, ref T data)
        {
            if (_indexes is null) ReadFile();
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
            if (Logger<Session>.IsDebugAllowed)
                Logger<Session>.Debug($"{_filePath} saving");
        }

        public void Remove(string name)
        {
            if (_indexes is null) ReadFile();
            var index = _indexes.IndexOf(name);
            if (index < 0) return;

            _lines[0] = _lines[0].Replace(name + "\0", "");
            _lines.RemoveAt(index + 1);
            _indexes.RemoveAt(index);
            MarkDirty();
        }

        public bool Contains(string name)
        {
            if (_indexes is null) ReadFile();
            return _indexes.Contains(name);
        }

        private void MarkDirty()
        {
            if (IsDirty == false)
            {
                IsDirty = true;
                if (_autoFlush) Loop.EventEndFrame += Flush;
            }
        }

        private void ReadFile()
        {
            _lines = new List<string>(File.ReadAllLines(_filePath));
            if (_lines.Count == 0)
            {
                _indexes = new List<string>();
                _lines.Add("");
                return;
            }
            
            _indexes = new List<string>(_lines[0].Split('\0', StringSplitOptions.RemoveEmptyEntries));
        }

        public void Flush()
        {
            IsDirty = false;
            if (_autoFlush) Loop.EventEndFrame -= Flush;
            File.WriteAllLines(_filePath, _lines);
            if (Logger<Session>.IsDebugAllowed)
                Logger<Session>.Debug($"{_filePath} saved");
        }
    }
}
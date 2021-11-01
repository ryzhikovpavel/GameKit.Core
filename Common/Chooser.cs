using System.Collections.Generic;
using UnityEngine;

namespace Sources
{
    public class Chooser<T>
    {
        public int Total { get; private set; }

        private readonly List<T> _items = new List<T>();
        private readonly List<int> _chances = new List<int>();        
        
        public void Clear()
        {
            _items.Clear();
            _chances.Clear();
            Total = 0;
        }

        public void Add(T item, int chance)
        {
            if (chance == 0) return;

            _items.Add(item);
            _chances.Add(chance);
            Total += chance;
        }
        
        public void Add(Chooser<T> other)
        {
            _items.AddRange(other._items);
            _chances.AddRange(other._chances);
            Total += other.Total;
        }

        public T Choice(int choice)
        {
            if ((choice < 0) || (choice >= Total))
            {
                Debug.LogError("Choice index out of range. Choice: " + choice + " Total:" + Total);
                return default(T);
            }

            for (int i = 0; i < _chances.Count; ++i)
            {
                if (choice < _chances[i]) return _items[i];
                else choice -= _chances[i];
            }

            return _items[0];
        }

        public T ChoiceRandom()
        {
            return Choice(Random.Range(0, Total));
        }
    }
}
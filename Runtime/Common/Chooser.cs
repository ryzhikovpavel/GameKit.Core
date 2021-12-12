using System.Collections.Generic;
using UnityEngine;

namespace GameKit
{
    public class Chooser<T>
    {
        public int Total { get; private set; }

        private readonly List<T> _items = new List<T>();
        private readonly List<int> _weights = new List<int>();        
        
        public void Clear()
        {
            _items.Clear();
            _weights.Clear();
            Total = 0;
        }

        public void Add(T item, int weight)
        {
            if (weight == 0) return;

            _items.Add(item);
            _weights.Add(weight);
            Total += weight;
        }
        
        public void Add(Chooser<T> other)
        {
            _items.AddRange(other._items);
            _weights.AddRange(other._weights);
            Total += other.Total;
        }

        public T Choice(int choiceWeight)
        {
            if ((choiceWeight < 0) || (choiceWeight >= Total))
            {
                Debug.LogError("Choice index out of range. Choice: " + choiceWeight + " Total:" + Total);
                return default(T);
            }

            for (int i = 0; i < _weights.Count; ++i)
            {
                if (choiceWeight < _weights[i]) return _items[i];
                else choiceWeight -= _weights[i];
            }

            return _items[0];
        }

        public T ChoiceRandom()
        {
            return Choice(Random.Range(0, Total));
        }
    }
}
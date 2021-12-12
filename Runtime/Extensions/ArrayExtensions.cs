#pragma warning disable 649
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace GameKit
{
    public static class ArrayExtensions
    {
        public static int IndexOf<T>(this T[] list, T obj)
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (Equals(list[i], obj)) return i;
            }
            return -1;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int index = list.Count;
            while (index > 1)
            {
                index--;
                int newPos = Random.Range(0, index + 1);
                T temp = list[newPos];
                list[newPos] = list[index];
                list[index] = temp;
            }
        }

        public static void Swap<T>(this IList<T> list, int indexOne, int indexTwo)
        {
            if (indexOne < 0 || indexTwo < 0 || indexOne == indexTwo || indexOne >= list.Count || indexTwo >= list.Count)
                return;

            T temp = list[indexOne];
            list[indexOne] = list[indexTwo];
            list[indexTwo] = temp;
        }

        public static int GetRandomIndex<T>(this IList<T> source)
        {
            if (source.Count == 0)
                return -1;
            return Random.Range(0, source.Count);
        }

        public static T GetRandom<T>(this IList<T> source)
        {
            if (source.Count == 0)
                return default(T);
            return source[Random.Range(0, source.Count)];
        }

        public static bool TryGetRandomFromActive<T>(this IList<T> source, out T item) where T : Component
        {
            item = default(T);
            if (source.Count == 0) return false;
            int rnd = Random.Range(0, source.Count);
            for (int i = 0; i < source.Count; i++)
            {
                int idx = (rnd + i).Repeat(0, source.Count - 1);
                if (source[idx].gameObject.activeSelf)
                {
                    item = source[idx];
                    return true;
                }
            }

            return false;
        }
        public static bool TryGetRandomFromInactive<T>(this IList<T> source, out T item) where T: Component
        {
            item = default(T);
            if (source.Count == 0) return false;
            int rnd = Random.Range(0, source.Count);
            for (int i = 0; i < source.Count; i++)
            {
                int idx = (rnd + i).Repeat(0, source.Count - 1);
                if (source[idx].gameObject.activeSelf == false)
                {
                    item = source[idx];
                    return true;
                }
            }

            return false;
        }

        public static T PickRandom<T>(this IList<T> source)
        {
            if (source.Count == 0)
                return default(T);

            int i = Random.Range(0, source.Count);
            T value = source[i];
            source.RemoveAt(i);

            return value;
        }

        public static T[] PickRange<T>(this IList<T> source, int index, int count)
        {
            T[] res = new T[count];
            for (int i = 0; i < count; i++)
            {
                res[i] = source[0];
                source.RemoveAt(0);
            }
            return res;
        }

        public static T First<T>(this IList<T> source)
        {
            if (source.Count == 0)
                return default(T);

            return source[0];
        }

        public static T Last<T>(this IList<T> source)
        {
            if (source.Count == 0)
                return default(T);

            return source[source.Count - 1];
        }

        public static T PickFirst<T>(this IList<T> source)
        {
            if (source.Count == 0)
                return default(T);

            T value = source.First();
            source.RemoveAt(0);

            return value;
        }

        public static T PickLast<T>(this IList<T> source)
        {
            if (source.Count == 0)
                return default(T);

            T value = source.Last();
            source.RemoveAt(source.Count - 1);

            return value;
        }

        public static T Pick<T>(this IList<T> source, int index)
        {
            if (source.Count <= index)
                return default(T);

            T value = source[index];
            source.RemoveAt(index);

            return value;
        }
    }
}
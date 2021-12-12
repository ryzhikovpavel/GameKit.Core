#pragma warning disable 649
using System;

namespace GameKit
{
    public static class ArrayTools
    {
        public static void Add<T>(ref T[] array, T item)
        {
            var i = array.Length;
            Array.Resize(ref array, i + 1);
            array[i] = item;
        }

        public static void Remove<T>(ref T[] array, int index)
        {
            for (int i = index; i < array.Length - 1; i++)
                array[i] = array[i + 1];
            Array.Resize(ref array, array.Length - 1);
        }

        public static bool Remove<T>(ref T[] array, T item)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(item))
                {
                    Remove(ref array, i);
                    return true;
                }
            }
            return false;
        }
    }
}
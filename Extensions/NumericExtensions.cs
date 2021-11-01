#pragma warning disable 649
using UnityEngine;
using System;

namespace GameKit
{
    public static class NumericExtensions
    {
        public static int Clamp(this int num, int min, int max)
        {
            if (num > max) return max;
            if (num < min) return min;
            return num;
        }

        public static float Clamp(this float num, float min, float max)
        {
            if (num > max) return max;
            if (num < min) return min;
            return num;
        }

        public static int Repeat(this int num, int min, int max)
        {
            if (min >= max) throw new ArgumentException();
            while (num > max) num -= max;
            while (num < min) num += min;
            return num;
        }
    }
}
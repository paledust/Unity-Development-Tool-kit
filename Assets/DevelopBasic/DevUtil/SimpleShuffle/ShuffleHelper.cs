using System.Collections.Generic;
using UnityEngine;

namespace SimpleShuffle
{
    public static class ShuffleHelper
    {
        public static void Shuffle<T>(ref T[] elements)
        {
            var rnd = new System.Random();
            for (int i = 0; i < elements.Length; i++)
            {
                int index = rnd.Next(i + 1);
                T tmp = elements[i];
                elements[i] = elements[index];
                elements[index] = tmp;
            }
        }
        public static void Shuffle<T>(ref List<T> elements)
        {
            var rnd = new System.Random();
            for (int i = 0; i < elements.Count; i++)
            {
                int index = rnd.Next(i + 1);
                T tmp = elements[i];
                elements[i] = elements[index];
                elements[index] = tmp;
            }
        }
    }
}

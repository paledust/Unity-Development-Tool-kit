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
        //重洗牌，并确保牌库底牌和洗牌后的初始牌不一致
        public static void Reshuffle<T>(ref T[] elements)
        {
            if (elements.Length <= 1)
            {
                Debug.Log("Only 1 elements, No Shuffle Apply");
                return;
            }
            T last = elements[elements.Length - 1];
            do
            {
                Shuffle(ref elements);
            } while (elements[0].Equals(last));
        }
    }
}

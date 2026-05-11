using System.Collections.Generic;

namespace SimpleShuffle
{
    public class ShuffleDeck<T>
    {
        private T[] deck;
        private int shuffleCounter;
        public ShuffleDeck(T[] items)
        {
            deck = new T[items.Length];
            items.CopyTo(deck, 0);
            Shuffle();
        }
        public ShuffleDeck(List<T> items)
        {
            deck = new T[items.Count];
            items.CopyTo(deck, 0);
            Shuffle();
        }
        public T PopNext()
        {
            if (shuffleCounter == 0)
            {
                Shuffle();
            }
            shuffleCounter --;
            return deck[shuffleCounter];
        }
        void Shuffle()
        {
            shuffleCounter = deck.Length;
            ShuffleHelper.Shuffle(ref deck);
        }
    }
}

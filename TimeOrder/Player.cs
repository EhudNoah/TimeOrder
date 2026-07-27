using System.Collections.Generic;

namespace TimeOrder
{
    public class Player
    {
        public string Name { get; private set; }
        public List<Card> Hand { get; private set; }

        public Player(string name)
        {
            Name = name;
            Hand = new List<Card>();
        }

        public Card DrawCard(Deck deck)
        {
            Card card = deck.DrawCard();

            if (card != null)
            {
                Hand.Add(card);
            }

            return card;
        }

        public void RemoveCard(Card card)
        {
            Hand.Remove(card);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackjackRound2
{
    public class Deck
    {
        private static Random rand = new Random();
        public List<Card> cards { get; private set; }

        public Deck(List<Card> cards)
        {
            this.cards = cards;
        }
    }

    public class Card
    {
        public string name { get; private set; }
        public string suit { get; private set; }
        public int value { get; private set; }
        public bool firstView { get; private set; }

        public Card(string name, string suit, int value)
        {
            this.name = name;
            this.suit = suit;
            this.value = value;
            this.firstView = true;
        }
    }

    public class Hand
    {
        public List<Card> cards { get; set; }
        public double scoreMultiplier { get; set; }
        public int state { get; set; } //0 for playing, 1 for finished

        public Hand(List<Card> cards)
        {
            this.cards = cards;
            this.scoreMultiplier = 1;
            this.state = 0;
        }
    }

    public static class Extensions
    {
        private static Random rand = new Random();

        public static Deck Shuffle(this Deck _deck)
        {
            int n = _deck.cards.Count;
            while (n > 1)
            {
                n--;
                int _r = rand.Next(n + 1);
                Card _temp = _deck.cards[_r];
                _deck.cards[_r] = _deck.cards[n];
                _deck.cards[n] = _temp;
            }
            return _deck;
        }

        public static Deck MakeNew()
        {
            List<Card> cards = new List<Card>();
            List<string> names = new List<string>() { "Ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King" };
            List<string> suits = new List<string>() { "spades", "hearts", "diamonds", "clubs" };
            int value;
            foreach (string suit in suits)
            {
                foreach (string name in names)
                {
                    value = GetValue(name);
                    Card _tempCard = new Card(name, suit, value);
                    cards.Add(_tempCard);
                }
            }
            return new Deck(cards);
        }

        public static int GetValue(this string name)
        {
            int value;
            if (int.TryParse(name, out value))
                return value;
            else if (name == "Ace")
                return -1;
            else if (name == "Jack" || name == "Queen" || name == "King")
                return 10;
            return -2;
        }

        public static int Score(this Hand _hand)
        {
            List<Card> cards = _hand.cards.OrderByDescending(p => p.value).ToList();
            int handValue = 0;
            foreach (Card c in _hand.cards)
            {
                if (c.value == -1)
                    if (handValue > 11)
                        handValue = handValue + 1;
                    else
                        handValue = handValue + 11;
                else
                    handValue = handValue + c.value;
            }
            return handValue;
        }
    }
}

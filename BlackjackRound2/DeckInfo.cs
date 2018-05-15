using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackjackRound2
{
    public class Deck
    {
        private List<Card> cards;

        public Deck(List<Card> cards)
        {
            this.cards = cards;
        }

        public Deck()
        {
            MakeNew();
        }

        private void MakeNew()
        {
            List<Card> cards = new List<Card>();
            List<string> names = new List<string>() { "Ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King" };
            List<string> suits = new List<string>() { "spades", "hearts", "diamonds", "clubs" };
            int value;
            foreach (string suit in suits)
            {
                foreach (string name in names)
                {
                    value = Card.GetValue(name);
                    Card _tempCard = new Card(name, suit, value);
                    cards.Add(_tempCard);
                }
            }
            this.cards = cards;
            Shuffle();
        }

        public void Shuffle()
        {
            Random rand = new Random();
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int _r = rand.Next(n + 1);
                Card _temp = cards[_r];
                cards[_r] = cards[n];
                cards[n] = _temp;
            }
        }

        public Card Deal()
        {
            if (cards.Count == 0)
                MakeNew();
            Card x = cards[0];
            cards.RemoveAt(0);
            return x;
        }
    }

    public class Card
    {
        public string name { get; private set; }
        public string suit { get; private set; }
        int value;

        private bool _firstView;
        public bool firstView {
            get
            {
                if (_firstView)
                {
                    _firstView = false;
                    return true;
                }
                return false;
            }
            private set { _firstView = value; } }

        public Card(string name, string suit, int value)
        {
            this.name = name;
            this.suit = suit;
            this.value = value;
            this.firstView = true;
        }

        public static int GetValue(string name)
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

        public int GetValue()
        {
            return value;
        }

        public override string ToString()
        {
            return name + " of " + suit;
        }
    }

    public class Hand
    {
        public List<Card> cards { get; set; }
        public double bet { get; private set; }
        public int winState;
        public bool inPlay {get;private set;} //0 for playing, 1 for finished

        public Hand(List<Card> cards, double bet)
        {
            this.cards = cards;
            this.bet = bet;
            this.inPlay = true;
        }

        public void DoubleDown()
        {
            bet *= 2;
        }

        public List<Card> ValueSortedCards()
        {
            List<Card> aces = new List<Card>();
            List<Card> x = new List<Card>();
            foreach(Card c in cards)
                if (c.GetValue() == -1)
                    aces.Add(c);
                else
                    x.Add(c);
            x.AddRange(aces);
            return x;
            //Used to ensure aces get their value calculated last
        }

        public int Score()
        {
            int value = 0;
            List<Card> tempList = ValueSortedCards();
            foreach (Card c in tempList)
            {
                value += (c.GetValue() == -1 && tempList.Count-tempList.IndexOf(c) == 1 ) ? ((value > 11) ? 1 : 11) : c.GetValue();
            }
            return value;
        }

        public double Bet(bool wasSplit)
        {
            if (Score() == 21 && cards.Count == 2)
                if (!wasSplit)
                    return bet *= 1.5;
            return Score();
        }

        public void Write(string owner, bool selected, int location = -1)
        {
            Console.CursorLeft = 2;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            if (selected)
                Console.BackgroundColor = ConsoleColor.DarkBlue;
            if (!inPlay)
                Console.BackgroundColor = ConsoleColor.DarkGray;
            if (location >= 0)
                Console.Write("Hand " + (location+1) + ": ");
            else
                Console.Write("Dealer's Hand: ");
            foreach(Card c in cards)
            {
                if (c.firstView && location != -1)
                    Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(c);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" | ");
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Score: ");
            DrawScore();
            Console.WriteLine();
            Console.WriteLine();
        }

        private void DrawScore()
        {
            int x = Score();
            if (x > 21)
                Console.BackgroundColor = ConsoleColor.DarkRed;
            else if (x == 21)
            {
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            else
                Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.Write(x);
        }

        public void EndPlay()
        {
            inPlay = false;
        }
    }
}

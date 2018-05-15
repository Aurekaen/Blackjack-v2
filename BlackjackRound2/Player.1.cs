using System;
using System.Collections.Generic;
using System.Text;

namespace BlackjackRound2
{
    public class Player
    {
        public List<Hand> Hands;

        public Player(ref Deck d)
        {
            Hands.Add(new Hand(new List<Card>() { d.cards[0], d.cards[1] }));
            d.cards.RemoveAt(0);
            d.cards.RemoveAt(0);
        }

        public int currHand()
        {
            foreach (Hand h in Hands)
                if (h.state == 0)
                    return Hands.IndexOf(h);
            return -1;
        }

        public void WriteCards()
        {
            int x = 0;
            foreach (Hand h in Hands)
            {
                x++;
                Console.ResetColor();
                if (h == Hands[currHand()])
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                if (h.state == 1)
                    Console.BackgroundColor = ConsoleColor.Gray;
                Console.Write("Hand " + x + ":");
                foreach (Card c in h.cards)
                {
                    if (c.firstView)
                        Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(c.name + "of" + c.suit);
                    Console.ResetColor();
                    Console.Write(" | ");
                }
                Console.WriteLine();
                Console.WriteLine(); //leave a blank line in between hands for readability
            }
        }

        public bool Hit(ref Deck d)
        {
            Hands[currHand()].cards.Add(d.cards[0]);
            d.cards.RemoveAt(0);
            return CheckHand();
        }

        public bool DoubleDown(ref Deck d)
        {
            int x = currHand();
            Hands[currHand()].cards.Add(d.cards[0]);
            d.cards.RemoveAt(0);
            Hands[x].scoreMultiplier += 1;
            Hands[x].state = 1;
            return CheckHand();
        }

        public bool Split(ref Deck d)
        {
            int x = currHand();
            if (Hands[x].cards.Count < 2)
                return false;
            if (Hands[x].cards[0].name == Hands[x].cards[1].name)
            {
                Hands.Add(new Hand(new List<Card>() { Hands[x].cards[1] }));
                Hands[x].cards.RemoveAt(1);
                return true;
            }
            else
                return false;
        }

        public void Stand()
        {
            Hands[currHand()].state = 1;
        }

        public bool CheckHand() //true means end game, false means there is still a playable hand
        {
            int x = currHand();
            if (x < 0) //just in case the hand is already finished by another method, such as DoubleDown
                return true;
            if (Hands[x].Score() > 21)
                Hands[x].state = 1;
            if (currHand() < 0)
                return true;
            else
                return false;
        }
    }
}

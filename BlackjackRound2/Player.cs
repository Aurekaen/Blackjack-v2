using System;
using System.Collections.Generic;
using System.Text;

namespace BlackjackRound2
{
    class Player
    {
        List<Hand> Hands;
        int currentHandIndex;
        bool split;

        public Player(ref Deck d)
        {
            Hands = new List<Hand>() { new Hand(new List<Card>() {d.Deal(), d.Deal() }, GetBet())};
            currentHandIndex = 0;
            split = false;
        }

        public double GetBet()
        {
            Console.ResetColor();
            double answer = 0;
            bool haveAnswer = false;
            while (!haveAnswer)
            {
                Console.WriteLine("How much would you like to bet?");
                string answerString = Console.ReadLine();
                if (double.TryParse(answerString, out answer))
                {
                    haveAnswer = true;
                    Console.Clear();
                }
                else
                {

                    haveAnswer = false;
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.WriteLine("Please input a number.");
                }
            }
            return answer;
        }


        public void Draw()
        {
            foreach (Hand h in Hands)
            {
                int index = Hands.IndexOf(h);
                h.Write("Player", index == currentHandIndex, index);
                Console.WriteLine();
            }
        }
        public int Hit(ref Deck d)
        {
            Hands[currentHandIndex].cards.Add(d.Deal());
            return CheckHand();
        }

        public int DoubleDown(ref Deck d)
        {
            Hands[currentHandIndex].cards.Add(d.Deal());
            Hands[currentHandIndex].DoubleDown();
            Hands[currentHandIndex].EndPlay();
            currentHandIndex++;
            return CheckHand();
        }

        public int Split(ref Deck d)
        {
            if (Hands[currentHandIndex].cards.Count != 2)
                return -2;
            if (Hands[currentHandIndex].cards[0].name == Hands[currentHandIndex].cards[1].name)
            {
                Hands.Add(new Hand(new List<Card>() { Hands[currentHandIndex].cards[1] }, Hands[currentHandIndex].bet));
                Hands[currentHandIndex].cards.RemoveAt(1);
                split = true;
                return 2;
            }
            else
                return -3;
        }

        public int Stand(ref Deck d) //ref just to allow it to match delegate signature
        {
            Hands[currentHandIndex].EndPlay();
            currentHandIndex++;
            return CheckHand();
        }

        public int CheckHand() //0 means end game, 1 means there is still a playable hand
        {
            if (currentHandIndex < Hands.Count && Hands[currentHandIndex].Score() > 21)
            {
                Hands[currentHandIndex].EndPlay();
                currentHandIndex++;
            }
            if (currentHandIndex >= Hands.Count)
                return 0;
            else
                return 1;
        }

        public double EndHand(Hand dealerHand)
        {
            double roundNetBet = 0;
            int dealerScore = dealerHand.Score();
            int[] winStates = new int[Hands.Count];
            /* -1 = tie
             * 0 = unprocessed
             * 1 = player won
             * 2 = dealer won
             * 3 = player won with natural
             * 
            */
            foreach(Hand h in Hands)
            {
                int index = Hands.IndexOf(h);

                winStates[index] = (dealerScore == 21 && dealerHand.cards.Count == 2) ? 
                    ((h.Score() == 21) ? -1 : 2) : ((h.Score() > 21) ? 2 : 
                    ((h.Score() == 21 && dealerScore != 21 && h.cards.Count == 2) ? ((split)? 1 : 3 ) : 
                        ((h.Score() > dealerScore || dealerScore > 21) ? 1 : ((h.Score() == dealerScore) ? -1 : 2))));
            }

            List<int> wins = new List<int>();
            wins.AddRange(winStates);

            Console.ForegroundColor = ConsoleColor.Black;
            int z = 1;
            foreach(int i in wins)
            {
                int j = wins.IndexOf(i);
                string result = (i == -1) ? "ties." :
                    ((i == 1) ? "wins." :
                    ((i == 2) ? "loses." : "was a blackjack!"));
                Console.BackgroundColor = (i == -1) ? ConsoleColor.Gray :
                    ((i == 1) ? ConsoleColor.Green :
                    (i == 2) ? ConsoleColor.Yellow : ConsoleColor.Cyan);
                Console.WriteLine("Hand " + (z) + " " + result);
                Console.WriteLine();
                roundNetBet += (i == -1) ? 0 :
                    ((i == 1) ? Hands[j].bet :
                    ((i == 2) ? (Hands[j].bet * -1) : Hands[j].bet * 1.5));
                z++;
            }

            return roundNetBet;
        }

        public bool WentBust()
        {
            bool hasScoringHand = false;
            foreach (Hand h in Hands)
            {
                if (h.Score() >= 21)
                    hasScoringHand = true;
            }

            return hasScoringHand;
        }
    }
}

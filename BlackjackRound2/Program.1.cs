using System;
using System.Collections.Generic;

namespace BlackjackRound2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Welcome to the Blackjack Table";
            Console.CursorVisible = false;
            double netWinLoss = 0;

            while (PlayGame(ref netWinLoss))
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(2, 2);
                Console.WriteLine("Current Score: " + netWinLoss);
                Console.WriteLine();
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(2, 2);
            Console.WriteLine("Final Score: " + netWinLoss);
            Console.CursorVisible = true;
            Console.SetCursorPosition(0, 0);
            Console.ReadKey();
        }

        public delegate int MyDelegate(ref Deck MainDeck);
        /* -3: Non-matching cards when trying to split
         * -2: Wrong number of cards when trying to split
         * -1: Exiting game entirely
         * 0: All hands played
         * 1: All is fine
         * 2: Successful Split, All is still fine
         */

        private static bool PlayGame(ref double netWinLoss)
        {
            bool stillPlaying = true;
            bool handOver = false;
            Deck MainDeck = new Deck();
            Hand DealerHand = new Hand(new List<Card> { MainDeck.Deal(), MainDeck.Deal() }, 1);
            Player User = new Player(ref MainDeck);
            List<ConsoleKey> validInput = new List<ConsoleKey>() { ConsoleKey.H, ConsoleKey.D, ConsoleKey.S, ConsoleKey.Spacebar, ConsoleKey.Escape };
            List<MyDelegate> postInput = new List<MyDelegate>()
            {
                User.Hit,
                User.DoubleDown,
                User.Split,
                User.Stand,
                Abort
            };
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("Controls: H to Hit, D to Double Down, S to Split, Space to Stand, ESC to Exit.");
            Console.WriteLine();
            WriteDealerHand(DealerHand);
            User.Draw();

            while (!handOver && DealerHand.Score() < 21)
            {
                int input = postInput[GetResponse(validInput)].Invoke(ref MainDeck);
                if (input < 0)
                    CheckError(input);
                else if (input == 0)
                    handOver = true;
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Clear();
                    Console.WriteLine("Controls: H to Hit, D to Double Down, S to Split, Space to Stand, ESC to Exit.");
                    Console.WriteLine();
                    WriteDealerHand(DealerHand);
                    User.Draw();
                }

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
            }

            if (!User.WentBust())
            {
                while (DealerHand.Score() < 17)
                {
                    DealerHand.cards.Add(MainDeck.Deal());
                }
            }


            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(2, 2);
            string x = string.Join(" | ", DealerHand.cards);
            Console.WriteLine("Dealer's Hand: " + x + " | Score: " + DealerHand.Score());
            Console.WriteLine();
            User.Draw();

            netWinLoss += User.EndHand(DealerHand);
            Console.WriteLine();

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Would you like to continue playing? (Y/N)");
            if (GetResponse(new List<ConsoleKey>() { ConsoleKey.Y, ConsoleKey.N }) == 1)
                stillPlaying = false;

            Console.Clear();
            return stillPlaying;
        }

        private static void WriteDealerHand(Hand DealerHand)
        {
            Console.CursorLeft = 2;
            Console.WriteLine("Dealer's Hand: " + DealerHand.cards[0] + " | [Hidden]");
            Console.WriteLine();
        }

        private static void CheckError(int input)
        {
            switch (input)
            {
                case -1:
                    ThrowErorr("How did you even get here? The program is supposed to have already force-closed.");
                    break;
                case -2:
                    ThrowErorr("You must have two cards in your hand to split.");
                    break;
                case -3:
                    ThrowErorr("You cannot split non-matching cards.");
                    break;
            }
        }

        private static void ThrowErorr(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Warning: " + error);
            Console.ResetColor();
        }

        private static int GetResponse(List<ConsoleKey> validResponses)
        {
            while (true)
            {
                ConsoleKey input = Console.ReadKey(true).Key;
                if (validResponses.Contains(input))
                    return validResponses.IndexOf(input);
            }
        }

        private static int Abort(ref Deck d)
        {
            Environment.Exit(0);
            return -1;
        }
    }
}
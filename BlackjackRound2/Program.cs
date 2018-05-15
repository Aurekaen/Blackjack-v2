using System;
using System.Collections.Generic;

namespace BlackjackRound2
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                if (!PlayGame())
                    break;
            }
        }

        private static bool PlayGame()
        {
            Deck MainDeck = Extensions.MakeNew();
            List<Hand> PlayerHands = new List<Hand>();
            int playerHandIndex = 0;
            Hand DealerHand, currentPlayerHand = null;
            bool playing = true;
            int gameState = 0; //0 for running, 1 for player wins, 2 for tie, 3 for loss
            List<ConsoleKey> validInput = new List<ConsoleKey>() { ConsoleKey.H, ConsoleKey.D, ConsoleKey.S, ConsoleKey.Spacebar, ConsoleKey.Escape };
            List<Action> postInput = new List<Action>() { () => Hit(ref currentPlayerHand, ref MainDeck), () => DoubleDown(ref currentPlayerHand, ref MainDeck), () => Split(ref currentPlayerHand), () => Stand(ref currentPlayerHand), () => Abort() };


            MainDeck.Shuffle();
            PlayerHands.Add(DealHand(MainDeck));
            MainDeck.cards.RemoveRange(0, 2);
            DealerHand = DealHand(MainDeck);
            MainDeck.cards.RemoveRange(0, 2);

            if (DealerHand.Score() == 21) //handle dealer blackjack
                if (PlayerHands[0].Score() == 21)
                    gameState = 2;
                else
                    gameState = 3;

            while(playing)
            {
                int inputIndex = GetPlayerInput(validInput);
                currentPlayerHand = PlayerHands[playerHandIndex];
                postInput[inputIndex]();
                PlayerHands[inputIndex] = currentPlayerHand;
            }



            return true;
        }

        private static Hand Abort()
        {
            Environment.Exit(0);
            return null;
        }

        private static void DoubleDown(ref Hand _hand, ref Deck _deck)
        {
            _hand.cards.Add(_deck.cards[0]);
            _deck.cards.RemoveAt(0);
            _hand.state = 1;
        }

        private static void Stand(ref Hand _hand, out Hand _hand2)
        {

        }

        private static void Hit(ref Hand _hand, ref Deck _deck)
        {
            
        }

        private static void Split(ref Hand _hand)
        {

        }


        private static Hand DealHand(Deck _deck)
        {
            Hand tempHand = new Hand(new List<Card>());
            tempHand.cards.Add(_deck.cards[0]);
            tempHand.cards.Add(_deck.cards[1]);
            return tempHand;
        }

        private static int GetPlayerInput(List<ConsoleKey> validResponses)
        {
            while (true)
            {
                ConsoleKey input = Console.ReadKey().Key;
                if (validResponses.Contains(input))
                    return validResponses.IndexOf(input);
            }
        }
    }
}
using Wordle.Core;
using Wordle.Core.Game;


internal class Program
{

    private static void Main(string[] args)
    {
        var game = new WordleGame();

        var tries = 6;
        while (tries > 0)
        {
            Console.WriteLine("please guess the 5 letter word");
            string? guess = Console.ReadLine();
            if (string.IsNullOrEmpty(guess) || guess.Length != 5)
                Console.WriteLine("Try again with a 5 letter word");
            else
            {
                GuessResult guessResult = game.IsValidGuess(guess.ToLower());


                if (guessResult.State == GuessResult.GameState.NotAWord)
                {
                    Console.WriteLine("Not a word");
                }
                else if (guessResult.State == GuessResult.GameState.Correct)
                {
                    Console.WriteLine("CONGRATULATIONS");
                    break;
                }
                else 
                {

                    string closeness = string.Join(", ", guessResult.finalGuessPositions);
                    Console.WriteLine(closeness);
                    Console.WriteLine(guessResult.State.ToString());
                    Console.WriteLine("You have " + --tries + " tries left");
                }
                //TODO: tests
                //TODO: UI
                //TODO: documentation

            }

            Console.WriteLine();
        }

        
    }
}
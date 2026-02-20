using Wordle.Core;


internal class Program
{

    private static void Main(string[] args)
    {

        var provider = new WordProvider("C:\\Users\\GeorgeVæra\\Desktop\\Github\\Projects\\Wordle\\Wordle\\wordle_ord.txt");

        var alphabet = new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        var game = new WordleGame(provider);

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

                    foreach (var item in alphabet)
                    {
                        if (guessResult.green.Contains(item))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        else if (guessResult.yellow.Contains(item))
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }
                        else if (guessResult.alphabet.Contains(item))
                        {
                            Console.ForegroundColor = ConsoleColor.Gray; // default color
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray; // not guessed
                        }

                        Console.Write(item);
                        Console.ResetColor();
                    }
                    Console.WriteLine();
                    Console.WriteLine("You have " + --tries + " tries left");
                    if (tries == 0)
                        Console.WriteLine("The word was: " + game.GetWord());
                }

            }

            Console.WriteLine();
        }

        
    }
}
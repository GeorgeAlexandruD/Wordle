using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Wordle.Core.Game
{
    public class WordleGame
    {
        private List<string> wordsList = new List<string>();
        private string chosenWord = null!;
        private GuessResult guessResult = null!;
        private char[] finalGuessLeftover = null!;
        private char[] finalWordLeftover = null!;

        public WordleGame()
        {
            ReadAllWords();
            chosenWord = ChooseWord();
            //chosenWord = "baaat";
            //IsValidGuess("aaacv");
            //Console.ReadLine();
        }

        public GuessResult IsValidGuess(string currentGuess)
        {

            guessResult = new GuessResult
            {
                finalGuessPositions = [0, 0, 0, 0, 0]
            };
            finalGuessLeftover = new char[5];
            finalWordLeftover = new char[5];

            guessResult.State = GuessResult.GameState.NotAWord;

            if (!wordsList.Contains(currentGuess)) 
                guessResult.State = GuessResult.GameState.NotAWord;

            else if (currentGuess.Equals(chosenWord))
            {
                guessResult.State = GuessResult.GameState.Correct;
            }
            else
            {
                AreCharsPositionedPerfectly(currentGuess);
                AreCharactersFindable(); 
            }

            return guessResult;
        }

        private void AreCharsPositionedPerfectly(string currentGuess)
        {
            
            for (int i = 0; i < 5; i++)
            {
                if (currentGuess[i] == chosenWord[i])
                {
                    guessResult.finalGuessPositions[i] = 2;
                    guessResult.State = GuessResult.GameState.PartiallyCorrect;
                    Console.WriteLine(currentGuess[i] + " IS CORRECT");
                    finalGuessLeftover[i] = ' ';
                    finalWordLeftover[i] = ' ';
                }
                else
                {
                    finalGuessLeftover[i] = currentGuess[i];
                    finalWordLeftover[i] = chosenWord[i];
                }
            }
            //Console.WriteLine(string.Join(", ", finalGuessPositions)+" BINGO");
            //Console.WriteLine(string.Join("", finalGuessLeftover) +" BINGO");
            //Console.WriteLine(string.Join("", finalWordLeftover) + " BINGO");

        }
        private void AreCharactersFindable()
        {
           
            for (int i = 0; i < 5; i++)
            {
                int index = finalWordLeftover.IndexOf(finalGuessLeftover[i]);

                if (index >= 0 && finalGuessLeftover[i]!= ' ')
                {
                    guessResult.finalGuessPositions[i] = 1;
                    guessResult.State = GuessResult.GameState.PartiallyCorrect;
                    Console.WriteLine(finalGuessLeftover[i]+ " Is foundable in the word");
                    finalGuessLeftover[i] = ' ';
                    finalWordLeftover[index] = ' ';
           
                }
            }
        }

        private string ChooseWord()
        {
            Random rand = new Random();
            var wordNumber = rand.Next(0, wordsList.Count);
            Console.WriteLine(wordNumber);
            Console.WriteLine(wordsList[wordNumber]);
            return wordsList[wordNumber];

        }

        public void ReadAllWords()
        {
            
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader("C:\\Users\\GeorgeVæra\\Desktop\\Github\\Projects\\Wordle\\Wordle\\wordle_ord.txt");
                //Read the first line of text
                var line = sr.ReadLine();
                //Continue to read until you reach end of file
                while (line != null )
                {
                    //write the line to console window
                    if(line.Length==5)
                        wordsList.Add(line);
                    
                    //Read the next line
                    line = sr.ReadLine();
                }
                //close the file
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

    }

    public class GuessResult
    {
        public enum GameState { Correct = 0, PartiallyCorrect = 1, NotAWord  = 2 };
        public GameState State;
        public List<int> finalGuessPositions = null!;

    }
}

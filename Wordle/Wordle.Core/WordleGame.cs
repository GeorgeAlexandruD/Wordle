using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Wordle.Core
{
    public class WordleGame
    {
        private List<string> wordsList = null!;
        private string chosenWord = null!;
        private GuessResult guessResult = null!;
        private char[] finalGuessLeftover = null!;
        private char[] finalWordLeftover = null!;
        
        public WordleGame(WordProvider provider)
        {
            wordsList = provider.GetAllWords().ToList();
            chosenWord = provider.GetRandomWord();
        }


        public WordleGame(List<string> words, string chosenWord)
        {
            wordsList = words;
            this.chosenWord = chosenWord;
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
                guessResult.finalGuessPositions = [2, 2, 2, 2, 2];
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
        public string GetWord()
        {
            return chosenWord;
        }
    }

    public class GuessResult
    {
        public enum GameState { Correct = 0, PartiallyCorrect = 1, NotAWord  = 2 };
        public GameState State;
        public List<int> finalGuessPositions = null!;

    }
}

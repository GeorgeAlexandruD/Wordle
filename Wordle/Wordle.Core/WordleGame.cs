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
        private List<char> alphabet = new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        
        public WordleGame(WordProvider provider)
        {
            wordsList = provider.GetAllWords().ToList();
            chosenWord = provider.GetRandomWord();
            guessResult = new GuessResult
            {
                finalGuessPositions = [0, 0, 0, 0, 0],
                alphabet = new List<char>()
            };
            
        }
        private void GetLeftowerAlphabet(string currentWord)
        {
            Console.WriteLine("current " + currentWord);
            Console.WriteLine("correct " + chosenWord);

            char[] leftoverAlphabet = new char[26];
            alphabet.RemoveAll(c => currentWord.Contains(c));

        }


        public WordleGame(List<string> words, string chosenWord)
        {
            wordsList = words;
            this.chosenWord = chosenWord;
        }

        public GuessResult IsValidGuess(string currentGuess)
        {

            guessResult.finalGuessPositions = [0, 0, 0, 0, 0];
            finalGuessLeftover = new char[5];
            finalWordLeftover = new char[5];
            guessResult.alphabet.Clear();

            guessResult.State = GuessResult.GameState.Incorrect;

            if (!wordsList.Contains(currentGuess)) 
                guessResult.State = GuessResult.GameState.NotAWord;

            else if (currentGuess.Equals(chosenWord))
            {
                GetLeftowerAlphabet(currentGuess);
                guessResult.State = GuessResult.GameState.Correct;
                guessResult.finalGuessPositions = [2, 2, 2, 2, 2];
            }
            else
            {
                GetLeftowerAlphabet(currentGuess);
                AreCharsPositionedPerfectly(currentGuess);
                AreCharactersFindable(); 
            }
            guessResult.alphabet = new List<char>(alphabet);

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
                    guessResult.green.Add(currentGuess[i]);
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
                    guessResult.yellow.Add(finalGuessLeftover[i]);
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
        public enum GameState { Correct = 0, PartiallyCorrect = 1, Incorrect =2, NotAWord  = 3 };
        public GameState State;
        public List<int> finalGuessPositions = null!;
        public List<char> alphabet = null!;
        public HashSet<char> yellow = [];
        public HashSet<char> green = [];

    }
}

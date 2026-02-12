using System;
using System.Collections.Generic;
using System.Text;

namespace Wordle.Core
{
    public class WordProvider
    {
        private readonly List<string> words;
        private readonly Random random;

        public WordProvider(string filePath, Random? random = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path must not be empty.", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Word file not found.", filePath);

            words = File.ReadAllLines(filePath)
                        .Where(w => w.Length == 5)
                        .Select(w => w.Trim().ToLower())
                        .ToList();

            if (words.Count == 0)
                throw new InvalidOperationException("No valid 5-letter words found in file.");

            this.random = random ?? new Random();
        }

        public IReadOnlyList<string> GetAllWords()
        {
            return words;
        }

        public string GetRandomWord()
        {
            return words[random.Next(words.Count)];
        }
    }
}
namespace Wordle.Core.Tests
{
    public class WordleGameTests
    {
        [Fact]
        public void Guess_NotInWordList_ReturnsNotAWord()
        {
            var words = new List<string> { "apple", "grape" };
            var game = new WordleGame(words, "apple");

            var result = game.IsValidGuess("zzzzz");

            Assert.Equal(GuessResult.GameState.NotAWord, result.State);
        }

        [Fact]
        public void Guess_CorrectWord_ReturnsCorrect()
        {
            var words = new List<string> { "apple" };
            var game = new WordleGame(words, "apple");

            var result = game.IsValidGuess("apple");

            Assert.Equal(GuessResult.GameState.Correct, result.State);
            Assert.Equal(new List<int> { 2, 2, 2, 2, 2 }, result.finalGuessPositions);
        }

        [Fact]
        public void Guess_PartialMatch_ReturnsPartiallyCorrect()
        {
            var words = new List<string> { "apple", "pleap" };
            var game = new WordleGame(words, "apple");

            var result = game.IsValidGuess("pleap");

            Assert.Equal(GuessResult.GameState.PartiallyCorrect, result.State);
        }


        [Fact]
        public void Guess_DuplicateLetters_HandledCorrectly()
        {
            var words = new List<string> { "apple", "ppppp" };
            var game = new WordleGame(words, "apple");

            var result = game.IsValidGuess("ppppp");

            // Adjust expected values depending on your logic
            Assert.Equal(GuessResult.GameState.PartiallyCorrect, result.State);
        }

        [Fact]
        public void Guess_SomeCorrectSomeMisplaced()
        {
            var words = new List<string> { "apple", "papal" };
            var game = new WordleGame(words, "apple");

            // 'a' correct, 'p' misplaced
            var result = game.IsValidGuess("papal");

            // Checks game state
            Assert.Equal(GuessResult.GameState.PartiallyCorrect, result.State);

            // Check finalGuessPositions contains at least one 2 and some 1
            Assert.Contains(2, result.finalGuessPositions);
            Assert.Contains(1, result.finalGuessPositions);
        }

        [Fact]
        public void Guess_NotInList()
        {
            var words = new List<string> { "apple" };
            var game = new WordleGame(words, "apple");

            //not a foundeable word 
            var result = game.IsValidGuess("papal");

            // Checks game state
            Assert.Equal(GuessResult.GameState.NotAWord, result.State);
        }
    }
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wordle.Core;

namespace Wordle.AvaloniaUI
{
    public partial class MainWindow : Window
    {
        private Border[,] _tiles = new Border[6, 5];
        private int _currentRow = 0;
        private int _currentColumn = 0;
        private WordleGame _wordleGame = null!;
        private Dictionary<char, Border> _keyboardTiles = new();
        public MainWindow()
        {

            var provider = new WordProvider("C:\\Users\\GeorgeVæra\\Desktop\\Github\\Projects\\Wordle\\Wordle\\wordle_ord.txt");
            this._wordleGame = new WordleGame(provider);

            InitializeComponent();

            this.KeyDown += OnKeyDown;
            this.Focus();

            CreateBoard();
            CreateKeyboard();
            UpdateTileHighlight();
        }
        private void CreateKeyboard()
        {
            for (char letter = 'A'; letter <= 'Z'; letter++)
            {
                var text = new TextBlock
                {
                    Text = letter.ToString(),
                    FontSize = 16,
                    FontWeight = FontWeight.Bold,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                };

                var border = new Border
                {
                    Width = 35,
                    Height = 35,
                    Margin = new Thickness(3),
                    Background = Brushes.DarkGray,
                    CornerRadius = new CornerRadius(4),
                    Child = text
                };

                _keyboardTiles[letter] = border;
                KeyboardPanel.Children.Add(border);
            }
        }
        private void UpdateKeyboardColors(GuessResult result)
        {
            foreach (var letter in result.green)
            {
                var upper = char.ToUpper(letter);
                if (_keyboardTiles.ContainsKey(upper))
                    _keyboardTiles[upper].Background = Brushes.Green;
            }

            foreach (var letter in result.yellow)
            {
                var upper = char.ToUpper(letter);
                if (_keyboardTiles.ContainsKey(upper))
                {
                    var tile = _keyboardTiles[upper];
                    if (tile.Background != Brushes.Green)
                        tile.Background = Brushes.Goldenrod;
                }
            }

            foreach (var kvp in _keyboardTiles) { char letter = kvp.Key; if (!result.alphabet.Contains(char.ToLower(letter)) && !result.green.Contains(char.ToLower(letter)) && !result.yellow.Contains(char.ToLower(letter))) { kvp.Value.Background = Brushes.Black; } }
        }

        private async void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (_currentRow >= 6)
                return;

            // LETTERS
            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
                if (_currentColumn < 5)
                {
                    var letter = e.Key.ToString();
                    var textBlock = (TextBlock)_tiles[_currentRow, _currentColumn].Child!;
                    textBlock.Text = letter;

                    _currentColumn++;

                    // If row is full, keep highlight on last tile
                    if (_currentColumn == 5)
                        _currentColumn = 4;
                }
            }

            // BACKSPACE
            else if (e.Key == Key.Back)
            {
                if (_currentColumn > 0 ||
                   (_currentColumn == 0 &&
                    !string.IsNullOrEmpty(((TextBlock)_tiles[_currentRow, 0].Child!).Text)))
                {
                    if (_currentColumn == 4 &&
                        !string.IsNullOrEmpty(((TextBlock)_tiles[_currentRow, 4].Child!).Text))
                    {
                        // If at end of full row, move properly
                        _currentColumn = 5;
                    }

                    _currentColumn--;

                    var textBlock = (TextBlock)_tiles[_currentRow, _currentColumn].Child!;
                    textBlock.Text = "";
                }
            }
            else if (e.Key == Key.Enter)
            {
                bool rowFull = true;
                string guess = "";

                for (int c = 0; c < 5; c++)
                {
                    var textBlock = (TextBlock)_tiles[_currentRow, c].Child!;
                    if (string.IsNullOrEmpty(textBlock.Text))
                    {
                        rowFull = false;
                        break;
                    }

                    guess += textBlock.Text;
                }

                if (!rowFull)
                    return;

                var result = _wordleGame.IsValidGuess(guess.ToLower());

                // ❌ NOT A WORD
                if (result.State == GuessResult.GameState.NotAWord)
                {
                    ClearCurrentRow();
                    return; // Do NOT advance row
                }

                // ✅ VALID WORD
                ApplyColors(result);
                UpdateKeyboardColors(result);

                bool isCorrect = result.State == GuessResult.GameState.Correct;

                if (isCorrect)
                {
                    await ShowGameOverDialog(true);
                    StartNewGame();
                    return;
                }

                _currentRow++;
                _currentColumn = 0;

                // ❌ Last attempt and wrong
                if (!isCorrect && _currentRow >= 6)
                {
                    
                    await ShowGameOverDialog(false);
                    StartNewGame();
                    return;
                }
            }

            UpdateTileHighlight();
        }

        private async Task ShowGameOverDialog(bool won)
        {
            string message = won
                ? "You guessed it! 🎉"
                : $"The correct word was: {_wordleGame.GetWord().ToUpper()}";

            var dialog = new Window
            {
                Width = 300,
                Height = 150,
                Content = new TextBlock
                {
                    Text = message,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    FontSize = 18,
                    TextAlignment = Avalonia.Media.TextAlignment.Center
                }
            };

            await dialog.ShowDialog(this);
        }


        private void StartNewGame()
        {
            var provider = new WordProvider("C:\\Users\\GeorgeVæra\\Desktop\\Github\\Projects\\Wordle\\Wordle\\wordle_ord.txt");
            this._wordleGame = new WordleGame(provider);

            // Reset UI tiles
            for (int r = 0; r < 6; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    var border = _tiles[r, c];
                    var textBlock = (TextBlock)border.Child!;

                    textBlock.Text = "";
                    border.Background = Brushes.Transparent;
                    border.BorderBrush = Brushes.Gray;
                }
            }

            // Reset state
            _currentRow = 0;
            _currentColumn = 0;
            foreach (var key in _keyboardTiles.Values)
            {
                key.Background = Brushes.LightGray;
                key.BorderBrush = Brushes.DarkGray;
            }

            UpdateTileHighlight();
        }
        private void ClearCurrentRow()
        {
            for (int c = 0; c < 5; c++)
            {
                var textBlock = (TextBlock)_tiles[_currentRow, c].Child!;
                textBlock.Text = "";
            }

            _currentColumn = 0;
        }

        private async void ShowCorrectWord()
        {
            var correctWord = _wordleGame.GetWord().ToUpper();

            var dialog = new Window
            {
                Width = 300,
                Height = 150,
                Content = new TextBlock
                {
                    Text = $"The correct word was: {correctWord}",
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    FontSize = 18
                }
            };

            await dialog.ShowDialog(this);
        }

        private void CreateBoard()
        {
            for (int r = 0; r < 6; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    var text = new TextBlock
                    {
                        FontSize = 24,
                        FontWeight = FontWeight.Bold,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                    };

                    var border = new Border
                    {
                        Width = 55,
                        Height = 55,
                        Margin = new Thickness(5),
                        Background = new SolidColorBrush(Color.Parse("#121213")),
                        BorderBrush = new SolidColorBrush(Color.Parse("#3a3a3c")),
                        BorderThickness = new Thickness(2),
                        CornerRadius = new CornerRadius(4),
                        Child = text
                    };

                    _tiles[r, c] = border;
                    GameGrid.Children.Add(border);
                }
            }
        }

        private void UpdateTileHighlight()
        {
            for (int r = 0; r < 6; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    if (r == _currentRow && c == _currentColumn)
                    {
                        _tiles[r, c].BorderBrush = Brushes.White;
                        _tiles[r, c].BorderThickness = new Thickness(3);
                    }
                    else
                    {
                        _tiles[r, c].BorderBrush =
                            new SolidColorBrush(Color.Parse("#3a3a3c"));
                        _tiles[r, c].BorderThickness = new Thickness(2);
                    }
                }
            }
        }

        private void ApplyColors(GuessResult result)
        {
            for (int c = 0; c < 5; c++)
            {
                switch (result.finalGuessPositions[c])
                {
                    case 2:
                        _tiles[_currentRow, c].Background = Brushes.Green;
                        break;

                    case 1:
                        _tiles[_currentRow, c].Background = Brushes.Goldenrod;
                        break;

                    case 0:
                        _tiles[_currentRow, c].Background = Brushes.Gray;
                        break;
                }
            }
        }
    }
}
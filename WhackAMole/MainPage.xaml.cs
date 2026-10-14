
using Microsoft.Maui.Controls.Shapes;
using WhackAMole.Game;

namespace WhackAMole
{
    public partial class MainPage : ContentPage
    {
        private readonly GameEngine _engine = new();
        private IDispatcherTimer? _gameTimer;

        public MainPage()
        {
            InitializeComponent();

            _engine.Changed += UpdateLabels;
            _engine.GameOver += OnGameOver;

            CreateTimer();
            CreateGameGrid();
            CreateMoleTapGesture();
        }

        private void CreateTimer()
        {
            _gameTimer = Dispatcher.CreateTimer();
            _gameTimer.Interval = TimeSpan.FromSeconds(1);
            _gameTimer.Tick += (s, e) => _engine.Tick();
        }
        private void UpdateLabels()
        {
            ScoreLabel.Text = $"Score: {_engine.Score}";
            TimeLabel.Text = $"Time Remianing: {_engine.TimeRemaining} seconds";
        }
        private void CreateGameGrid()
        {
            for (int i = 0; i < GameConfig.GridSize; i++)
            {
                GameGrid.RowDefinitions.Add(new RowDefinition());
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int row = 0; row < GameConfig.GridSize; row++)
            {
                for (int col = 0; col < GameConfig.GridSize; col++)
                {
                    var cell = new Border
                    {
                        BackgroundColor = Colors.LightGreen,
                        Stroke = Colors.DarkGreen,
                        StrokeThickness = 3,
                        StrokeShape = new RoundRectangle { CornerRadius = 15 },
                        Margin = new Thickness(5)
                    };
                    GameGrid.Add(cell, col, row);
                }
            }
        }
        private void CreateMoleTapGesture()
        {
            var Tap = new TapGestureRecognizer();
            Tap.Tapped += OnMoleTapped;
            Mole.GestureRecognizers.Add(Tap);
        }

        // Event handlers
        private async void StartGameButton(object? sender, EventArgs e)
        {
            StartButton.IsEnabled = false;

            _engine.Start();
            _gameTimer?.Start();

            await ShowMole();

            StartButton.IsEnabled = true;
        }
        private async Task ShowMole()
        {
            while (_engine.IsRunning)
            {
                _engine.MoleNextPosition();
                Grid.SetRow(Mole, _engine.MoleRow);
                Grid.SetColumn(Mole, _engine.MoleCol);

                Mole.InputTransparent = true;
                Mole.Opacity = 0;
                Mole.Scale = 0.5;

                await Task.WhenAll(
                    Mole.FadeToAsync(1, GameConfig.AppearMs),
                    Mole.ScaleToAsync(1, GameConfig.AppearMs));

                if (!_engine.IsRunning) return;

                Mole.InputTransparent = false;
                await Mole.FadeToAsync(0, _engine.FadeOutMs);

                await Task.Yield(); // Let Cancel animation finish first
                Mole.InputTransparent = true;
            }
        }
        private void OnMoleTapped(object? sender, TappedEventArgs e)
        {
            _engine.Hit();
            Mole.InputTransparent = true;
            Mole.CancelAnimations();
        }
        private void OnGameOver()
        {
            _gameTimer?.Stop();
            Mole.CancelAnimations();
            Mole.Opacity = 0;
            Mole.InputTransparent = true;

            TimeLabel.Text = "Game Over!";
            ScoreLabel.Text = $"Score: {_engine.Score}";
        }
    }
}
namespace WhackAMole.Game
{
    public class GameEngine
    {
        private readonly Random _random = new();

        public int Score { get; private set; }
        public int TimeRemaining { get; private set; }
        public bool IsRunning { get; private set; }
        public int MoleRow { get; private set; }
        public int MoleCol { get; private set; }

        // How long the mole stays, shrinks as time runs out
        public uint FadeOutMs =>
            (uint) (GameConfig.BaseFadeMs + TimeRemaining * GameConfig.FadePerSecondMs);

        public event Action? Changed;
        public event Action? GameOver;

        public void Start()
        {
            Score = 0;
            TimeRemaining = GameConfig.GameSeconds;
            IsRunning = true;
            Changed?.Invoke();
        }

        public void Tick()
        {
            if (!IsRunning) return;
            TimeRemaining--;
            Changed?.Invoke();

            if (TimeRemaining <= 0)
            {
                IsRunning = false;
                GameOver?.Invoke();
            }
        }

        public void Hit()
        {
            if (!IsRunning) return;
            Score++;
            Changed?.Invoke();
        }

        public void MoleNextPosition()
        {
            MoleRow = _random.Next(GameConfig.GridSize);
            MoleCol = _random.Next(GameConfig.GridSize);
        }

    }
}

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;

namespace PieEatingNinjas.MineSweepingNinja.MineSweeper
{
    public class Game : INotifyPropertyChanged
    {
        DispatcherTimer timer;
        int dismantledMines;
        int incorrectDismantled;

        public int TotalDismantledMines { get { return dismantledMines + incorrectDismantled; } }
        public int Height { get; private set; }
        public int Width { get; private set; }
        public int TotalMines { get; private set; }
        public Tile[,] Tiles { get; private set; }

        public Tile ExplodedTile { get; private set; }

        int _ElapsedTime;
        public int ElapsedTime
        {
            get { return _ElapsedTime; }
            private set
            {
                _ElapsedTime = value;
                RaisePropertyChanged();
            }
        }

        public event EventHandler OnFinished;
        public event EventHandler OnExploded;

        public Game(int width, int height, int numberOfMines)
        {
            Width = width;
            Height = height;
            TotalMines = numberOfMines;
        }

        public void Init()
        {
            ElapsedTime = 0;
            dismantledMines = 0;
            incorrectDismantled = 0;
            ExplodedTile = null;

            Tiles = new Tile[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tile t = new Tile(this, x, y);
                    t.Exploded += T_Exploded;
                    t.DismantleChanged += T_DismantleChanged;
                    Tiles[x, y] = t;
                }
            }

            int b = 0;
            Random r = new Random();
            while (b < TotalMines)
            {
                int x = r.Next(Width);
                int y = r.Next(Height);

                Tile t = Tiles[x, y];
                if (!t.IsMine)
                {
                    t.IsMine = true;
                    b++;
                }
            }
        }

        private void T_DismantleChanged(object sender, EventArgs e)
        {
            Tile s = (Tile)sender;
            if (s.IsDismantled)
            {
                if (s.IsMine)
                    dismantledMines++;
                else
                    incorrectDismantled++;
            }
            else
            {
                if (s.IsMine)
                    dismantledMines--;
                else
                    incorrectDismantled--;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalDismantledMines)));

            if (dismantledMines == TotalMines)
            {
                timer.Stop();
                OnFinished?.Invoke(this, null);
            }
        }

        private void T_Exploded(object sender, EventArgs e)
        {
            timer.Stop();
            foreach (var item in Tiles)
            {
                item.Reveal();
            }
            ExplodedTile = sender as Tile;
            OnExploded?.Invoke(sender, null);
        }

        public void Start()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += OnTimerTick;
            timer.Start();
        }

        public bool IsBomb(int x, int y)
        {
            if(IsWithinBounds(x,y))
                    return Tiles[x, y].IsMine;
            return false;
        }

        public void OpenTile(int x, int y)
        {
            if (IsWithinBounds(x, y))
                Tiles[x, y].Open();
        }

        private bool IsWithinBounds(int x, int y)
            => x >= 0 && x < Width && y >= 0 && y < Height;

        private void OnTimerTick(object sender, object e) => ElapsedTime++;

        public void RaisePropertyChanged([CallerMemberName]string property = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

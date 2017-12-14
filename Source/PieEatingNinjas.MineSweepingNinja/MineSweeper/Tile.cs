using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using static PieEatingNinjas.MineSweepingNinja.MineSweeper.Icons;

namespace PieEatingNinjas.MineSweepingNinja.MineSweeper
{
    public class Tile : Button
    {
        const int HOLDTIMETRESHOLD = 200;

        bool wasLongPress;
        DateTime? timeStartedHolding;

        TileState state;

        public int X { get; }
        public int Y { get; }

        readonly Point[] neighbors;

        public Game Game { get; }
        public bool IsMine { get; set; }
        public bool IsOpened { get; set; }
        public bool IsDismantled { get; set; }

        public event EventHandler Exploded;
        public event EventHandler DismantleChanged;

        public Tile(Game game, int x, int y)
        {
            Game = game;
            X = x;
            Y = y;

            neighbors = new Point[8]
            {
                new Point(X - 1, Y - 1),
                new Point(X - 0, Y - 1),
                new Point(X + 1, Y - 1),
                new Point(X - 1, Y - 0),
                new Point(X + 1, Y - 0),
                new Point(X - 1, Y + 1),
                new Point(X - 0, Y + 1),
                new Point(X + 1, Y + 1)
            };

            Click += Tile_Click;

            Background = new SolidColorBrush(Colors.Transparent);
            Style = this.Resources["ButtonRevealStyle"] as Style;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            FontSize = 20;
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            timeStartedHolding = DateTime.Now;
            base.OnPointerPressed(e);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            timeStartedHolding = null;
            base.OnPointerExited(e);
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (timeStartedHolding.HasValue)
            {
                var holdTime = (DateTime.Now - timeStartedHolding.Value).TotalMilliseconds;
                if (holdTime >= HOLDTIMETRESHOLD)
                    OnLongPress();
            }
            timeStartedHolding = null;
            base.OnPointerReleased(e);
        }
       
        private void OnLongPress()
        {
            TileState newState = TileState.Nothing;
            bool isDismantled = false;
            switch (state)
            {
                case TileState.Nothing:
                    newState = TileState.Dismantled;
                    isDismantled = true;
                    break;
                case TileState.Dismantled:
                    newState = TileState.Potential;
                    break;
                case TileState.Potential:
                default:
                    break;
            }
            state = newState;
            Render();
            wasLongPress = true;

            if (IsDismantled != isDismantled)
            {
                IsDismantled = isDismantled;
                DismantleChanged?.Invoke(this, null);
            }
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            if (!wasLongPress)
            {
                if (!IsDismantled && IsEnabled)
                {
                    if (IsMine)
                    {
                        state = TileState.Exploded;
                        Render();
                        Exploded?.Invoke(this, null);
                    }
                    else
                    {
                        Open();
                    }
                }
            }
            wasLongPress = false;
        }

        internal void Reveal()
        {
            if (IsDismantled)
            {
                if (IsMine)
                    Background = new SolidColorBrush(Colors.Green);
                else
                    Background = new SolidColorBrush(Colors.Red);
            }
            else if (IsMine)
                Content = BOMB;
        }

        private void Render()
        {
            switch (state)
            {
                case TileState.Potential:
                    Content = DUNNO;
                    break;
                case TileState.Exploded:
                    Background = new SolidColorBrush(Colors.Tomato);
                    Content = BOMB;
                    break;
                case TileState.Dismantled:
                    Content = FLAG;
                    break;
                case TileState.Nothing:
                    Content = "";
                    break;
            }
        }

        public void Open()
        {
            if (!IsOpened && !IsDismantled && IsEnabled)
            {
                IsOpened = true;
                IsEnabled = false;

                int c = neighbors.Count(p => Game.IsBomb((int)p.X, (int)p.Y));

                if (c > 0)
                    Content = c.ToString();
                else
                    neighbors.ToList().ForEach(p => Game.OpenTile((int)p.X, (int)p.Y));
            }
        }
    }
}

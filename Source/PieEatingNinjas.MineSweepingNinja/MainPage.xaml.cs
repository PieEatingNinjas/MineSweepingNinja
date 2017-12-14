using PieEatingNinjas.MineSweepingNinja.MineSweeper;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using static PieEatingNinjas.MineSweepingNinja.MineSweeper.Icons;

namespace PieEatingNinjas.MineSweepingNinja
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private Game _Game;

        public Game Game
        {
            get { return _Game; }
            private set { _Game = value; RaisePropertyChanged(); }
        }

        private Visibility _OverlayVisibility;

        public Visibility OverlayVisibility
        {
            get { return _OverlayVisibility; }
            private set { _OverlayVisibility = value; RaisePropertyChanged(); }
        }


        public MainPage()
        {
            this.InitializeComponent();
            Start();
        }

        public void RaisePropertyChanged([CallerMemberName]string property = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        public event PropertyChangedEventHandler PropertyChanged;

        public void Start()
        {
            int size = 10;
            OverlayVisibility = Visibility.Collapsed;
            MainGrid.Children.Clear();
            Game = new Game(size, size, size);
            Game.OnExploded += Game_OnExploded;
            Game.OnFinished += Game_OnFinished;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Game)));

            Game.Init();

            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    var b = Game.Tiles[c, r];
                    b.SetValue(Grid.ColumnProperty, c);
                    b.SetValue(Grid.RowProperty, r);
                    MainGrid.Children.Add(b);
                }
            }
            Game.Start();
        }

        private void Game_OnFinished(object sender, EventArgs e)
        {
            Cat.Text = WINNER;
            GoToFinishedPage(true, Cat);
        }

        private void Game_OnExploded(object sender, EventArgs e)
        {
            GoToFinishedPage(false, sender as Button);
        }

        private async Task GoToFinishedPage(bool hasWon, UIElement animationSource)
        {
            OverlayVisibility = Visibility.Visible;
            await Task.Delay(200);
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("cat", animationSource);
            this.Frame.Navigate(typeof(FinishedPage), hasWon);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Cat.Text = NINJA;
            ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("cat");
            if (animation != null)
            {
                animation.TryStart(Cat);
            }
        }
    }
}

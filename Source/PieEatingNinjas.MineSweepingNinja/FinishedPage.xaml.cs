using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using static PieEatingNinjas.MineSweepingNinja.MineSweeper.Icons;

namespace PieEatingNinjas.MineSweepingNinja
{
    public sealed partial class FinishedPage : Page, INotifyPropertyChanged
    {
        private string _Title;

        public string Title
        {
            get { return _Title; }
            private set { _Title = value; RaisePropertyChanged(); }
        }

        private string _Subtitle;

        public string Subtitle
        {
            get { return _Subtitle; }
            private set { _Subtitle = value; RaisePropertyChanged(); }
        }

        private string _Image;

        public string Image
        {
            get { return _Image; }
            set { _Image = value; RaisePropertyChanged(); }
        }

        bool HasWon;

        public FinishedPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            HasWon = (e.Parameter as bool? ?? false);
            if (HasWon)
            {
                Title = "I don't always play games";
                Subtitle = "But when I do, I win!";
                Image = WINNER;
            }
            else
            {
                Title = "KABOOM!";
                Subtitle = "You lost!";
                Image = BOMB;
            }

            ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("cat");
            if (animation != null)
            {
                animation.TryStart(txtImage);
            }
        }

        public async void NewGame()
        {
            Image = NINJA;
            await Task.Delay(200);
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("cat", txtImage);
            Frame.Navigate(typeof(MainPage));
        }

        public async void ViewBoard()
        {
            if (HasWon)
            {
                Image = NINJA;
                await Task.Delay(200);
            }
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("cat", txtImage);
            Frame.GoBack();
        }

        public void RaisePropertyChanged([CallerMemberName]string property = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

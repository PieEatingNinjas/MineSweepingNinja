using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using static PieEatingNinjas.MineSweepingNinja.MineSweeper.Icons;

namespace PieEatingNinjas.MineSweepingNinja
{
    public sealed partial class StartPage : Page
    {
        public StartPage()
        {
            this.InitializeComponent();
            Cat.Text = NINJA;
        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("cat", Cat);
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}

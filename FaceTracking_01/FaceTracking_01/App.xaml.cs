using Xamarin.Forms;
using FaceTracking_01.Views;

namespace FaceTracking_01
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new FaceTrackingPage());
        }
    }
}
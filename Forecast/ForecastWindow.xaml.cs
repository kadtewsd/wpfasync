using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Forecast
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ForecastWindow : Window
    {
        public ForecastWindow()
        {
            InitializeComponent();
        }

        // Delegates to be used in placking jobs onto the Dispatcher.
        private delegate void NoArgDelegate();
        private delegate void OneArgDelegate(String arg);

        // Storyboards for the animations.
        private Storyboard showClockFaceStoryboard;
        private Storyboard hideClockFaceStoryboard;
        private Storyboard showWeatherImageStoryboard;
        private Storyboard hideWeatherImageStoryboard;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Load the storyboard resources.
            showClockFaceStoryboard = 
                (Storyboard)this.Resources["ShowClockFaceStoryboard"];
            hideClockFaceStoryboard = 
                (Storyboard)this.Resources["HideClockFaceStoryboard"];
            showWeatherImageStoryboard = 
                (Storyboard)this.Resources["ShowWeatherImageStoryboard"];
            hideWeatherImageStoryboard = 
                (Storyboard)this.Resources["HideWeatherImageStoryboard"];   
        }
        private void ForecastButtonHandler(object sender, RoutedEventArgs e)
        {
            // Change the status image and start the rotation animation.
            fetchButton.IsEnabled = false;
            fetchButton.Content = "Contacting Server";
            weatherText.Text = "";
            hideWeatherImageStoryboard.Begin(this);

            NoArgDelegate timer = new NoArgDelegate(this.SetTimer);

            tomorrowsWeather.Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Normal,
                timer);
            // Start fetching the weather forecast asynchronously.
            NoArgDelegate fetcher = new NoArgDelegate(
                this.FetchWeatherFromServer);

            fetcher.BeginInvoke(null, null);
        }

        private void SetTimer() {
            showClockFaceStoryboard.Begin(this);
            ImageSource img =  (ImageSource)this.Resources["Timer"];
            weatherIndicatorImage.Source = img;
        }

        private void FetchWeatherFromServer()
        {
            // Simulate the delay from network access.
            Thread.Sleep(2000);              
            
            // Tried and true method for weather forecasting - random numbers.
            Random rand = new Random();
            String weather;

            if (rand.Next(2) == 0)
            {
                weather = "rainy";
            }
            else
            {
                weather = "sunny";
            }

            // Schedule the update function in the UI thread.
            tomorrowsWeather.Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Normal,
                new OneArgDelegate(UpdateUserInterface), 
                weather);
        }

        private void HideWatch() {

            //Stop clock animation
            showClockFaceStoryboard.Stop(this);
            hideClockFaceStoryboard.Begin(this);
        }

        private void UpdateUserInterface(String weather)
        {
            //weatherIndicatorImage.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new NoArgDelegate(HideWatch));

            Thread.Sleep(2000);
            //Set the weather image
            if (weather == "sunny")
            {       
                weatherIndicatorImage.Source = (ImageSource)this.Resources[
                    "SunnyImageSource"];
            }
            else if (weather == "rainy")
            {
                weatherIndicatorImage.Source = (ImageSource)this.Resources[
                    "RainingImageSource"];
            }

            HideWatch();
            showWeatherImageStoryboard.Begin(this);
            //Update UI text
            fetchButton.IsEnabled = true;
            fetchButton.Content = "Fetch Forecast";
            weatherText.Text = weather;     
        }

        private void HideClockFaceStoryboard_Completed(object sender,
            EventArgs args)
        {         
            showWeatherImageStoryboard.Begin(this);
        }
        
        private void HideWeatherImageStoryboard_Completed(object sender,
            EventArgs args)
        {           
            showClockFaceStoryboard.Begin(this, true);
        }    

    }
}

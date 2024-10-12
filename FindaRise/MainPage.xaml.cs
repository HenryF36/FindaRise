using System;
using System.Net.Http;
using System.Text.Json;
using System.Timers;
using Microsoft.Maui.Dispatching;
using System.Threading.Tasks;

namespace FindaRise
{
    public partial class MainPage : ContentPage
    {
        private static System.Timers.Timer _timer;
        private const string ApiUrl = "https://api.sunrise-sunset.org/json";

        public MainPage()
        {
            InitializeComponent();
            SetTimer();
            GetSunriseSunsetTimes(); // Fetch sunrise and sunset times when the page loads
        }

        private void SetTimer()
        {
            // Create a timer that triggers every second (1000 ms)
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true; // Make sure the timer resets after each interval
            _timer.Enabled = true;   // Start the timer
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            DateTime now = DateTime.Now;
            string formattedTime = now.ToString("hh:mm:ss tt");

            // Update the label on the main thread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ClockL.Text = "Current Time is " + formattedTime;  // Safely update the UI
            });
        }

        private async void GetSunriseSunsetTimes()
        {
            try
            {
                // Specify your location coordinates
                double latitude = 34.0522; // Los Angeles
                double longitude = -118.2437; // Los Angeles

                // Your WeatherAPI key
                string apiKey = "YOUR_API_KEY_HERE"; // Replace with your actual API key

                // Create an HTTP client
                using HttpClient client = new HttpClient();
                string url = $"https://api.weatherapi.com/v1/current.json?key={apiKey}&q={latitude},{longitude}";
                Console.WriteLine($"Request URL: {url}");

                // Get the response from the API
                string response = await client.GetStringAsync(url);
                Console.WriteLine("API Response: " + response);

                // Deserialize the JSON response
                var weatherResponse = JsonSerializer.Deserialize<WeatherApiResponse>(response);
                if (weatherResponse != null && weatherResponse.Location != null)
                {
                    // Get the sunrise and sunset times
                    string sunrise = weatherResponse.Location.Localtime; // Localtime includes the date and time
                    string sunset = weatherResponse.Location.Localtime; // Localtime includes the date and time

                    // Update UI labels with sunrise and sunset times
                    RiseL.Text = $"The sun will rise at {sunrise}";
                    SetL.Text = $"The sun will set at {sunset}";
                }
                else
                {
                    // Handle null result
                    Console.WriteLine("Deserialization resulted in null.");
                    RiseL.Text = "Error fetching sunrise time.";
                    SetL.Text = "Error fetching sunset time.";
                }
            }
            catch (Exception ex)
            {
                // Display and log the error message
                RiseL.Text = $"Error fetching sunrise time: {ex.Message}";
                SetL.Text = $"Error fetching sunset time: {ex.Message}";
                Console.WriteLine($"Exception Type: {ex.GetType()}");
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }

        // Model for the WeatherAPI response
        public class WeatherApiResponse
        {
            public Location Location { get; set; }
        }

        public class Location
        {
            public string Name { get; set; }
            public string Region { get; set; }
            public string Country { get; set; }
            public string Localtime { get; set; } // Includes both date and time
        }




        private void CordTog(object sender, EventArgs e)
        {
            Console.WriteLine("Coordinates toggled!");
        }

        
        



    }
}


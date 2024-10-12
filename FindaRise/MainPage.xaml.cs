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

                // Create an HTTP client
                using HttpClient client = new HttpClient();
                string url = $"{ApiUrl}?lat={latitude}&lng={longitude}&formatted=0"; // Use formatted=0 for ISO 8601 format

                // Log the constructed URL for debugging
                Console.WriteLine("Constructed URL: " + url);

                // Fetch the response
                string response = await client.GetStringAsync(url);

                // Log the raw response for debugging
                Console.WriteLine("API Response: " + response);
                RiseL.Text = "API Response: " + response; // Show raw response for initial debugging

                // Deserialize the JSON response
                var result = JsonSerializer.Deserialize<SunriseSunsetResponse>(response);

                // Check if the result or results is null
                if (result == null)
                {
                    Console.WriteLine("Deserialization resulted in null. Check JSON structure and class definitions.");
                    RiseL.Text = "Error fetching sunrise time.";
                    SetL.Text = "Error fetching sunset time.";
                    return;
                }

                if (result.Results == null)
                {
                    Console.WriteLine("Results in the response is null.");
                    RiseL.Text = "Error fetching sunrise time.";
                    SetL.Text = "Error fetching sunset time.";
                    return;
                }

                // Convert the UTC time to local time
                DateTime sunriseUtc = DateTime.Parse(result.Results.Sunrise);
                DateTime sunsetUtc = DateTime.Parse(result.Results.Sunset);

                // Update UI labels with sunrise and sunset times
                RiseL.Text = $"The sun will rise at {sunriseUtc.ToLocalTime():hh:mm:ss tt}";
                SetL.Text = $"The sun will set at {sunsetUtc.ToLocalTime():hh:mm:ss tt}";
            }
            catch (HttpRequestException httpEx)
            {
                RiseL.Text = "Network error. Please check your internet connection.";
                SetL.Text = "Network error. Please check your internet connection.";
                Console.WriteLine("HTTP Error: " + httpEx.Message);
            }
            catch (JsonException jsonEx)
            {
                RiseL.Text = "Error parsing data.";
                SetL.Text = "Error parsing data.";
                Console.WriteLine("JSON Parsing Error: " + jsonEx.Message);
            }
            catch (Exception ex)
            {
                // Handle any other exceptions
                RiseL.Text = "Error fetching sunrise time.";
                SetL.Text = "Error fetching sunset time.";
                Console.WriteLine("General Error: " + ex.Message);
            }
        }


        private void CordTog(object sender, EventArgs e)
        {
            Console.WriteLine("Coordinates toggled!");
        }

        public class SunriseSunsetResponse
        {
            public Results Results { get; set; }
            public string Status { get; set; } // Added to check response status
        }

        public class Results
        {
            public string Sunrise { get; set; }
            public string Sunset { get; set; }
            public string SolarNoon { get; set; }
            public string DayLength { get; set; }
            public string CivilTwilightBegin { get; set; }
            public string CivilTwilightEnd { get; set; }
            public string NauticalTwilightBegin { get; set; }
            public string NauticalTwilightEnd { get; set; }
            public string AstronomicalTwilightBegin { get; set; }
            public string AstronomicalTwilightEnd { get; set; }
        }


    }
}


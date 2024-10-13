using System;
using System.Net.Http;
using System.Security.AccessControl;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Maui.Dispatching;
using Newtonsoft.Json;

namespace FindaRise
{
    public partial class MainPage : ContentPage
    {
        private const string ApiUrl = "https://api.sunrise-sunset.org/json";
        private static System.Timers.Timer _timer;
        public Boolean CordShow = false;
        public double latitude = 666666666666;
        public double longitude = 666666666666;
        public MainPage()
        {
            InitializeComponent();
            SetTimer();
            GetSunriseSunsetTimes(); // Fetch sunrise and sunset times when the page loads
        }
        //Timers
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
                await GetCoordinatesAsync();

                // Create an HTTP client
                using HttpClient client = new HttpClient();
                string url = $"{ApiUrl}?lat={latitude}&lng={longitude}&formatted=0"; // Use formatted=0 for ISO 8601 format
                string response = await client.GetStringAsync(url);

                // Deserialize the JSON response using Newtonsoft.Json
                var result = JsonConvert.DeserializeObject<SunriseSunsetResponse>(response);

                if (result != null && result.Results != null)
                {
                    // Convert the UTC time to local time
                    DateTime sunriseUtc = DateTime.Parse(result.Results.Sunrise);
                    DateTime sunsetUtc = DateTime.Parse(result.Results.Sunset);

                    // Update UI labels with sunrise and sunset times
                    RiseL.Text = $"The sun will rise at {sunriseUtc.ToLocalTime():hh:mm:ss tt}";
                    SetL.Text = $"The sun will set at {sunsetUtc.ToLocalTime():hh:mm:ss tt}";
                    if (int.TryParse(result.Results.day_length, out int dayLengthInSeconds))
                    {
                        double dayLengthInHours = dayLengthInSeconds / 60.0/60 ; // Convert seconds to minutes
                        DayL.Text = $"The day length is {dayLengthInHours} hours"; // Update UI to display in minutes
                    }
                }
                else
                {
                    // Handle null result
                    Console.WriteLine("Deserialization resulted in null.");
                    RiseL.Text = "Error fetching sunrise time.";
                    SetL.Text = "Error fetching sunset time.";
                    DayL.Text = "Error fetching day length";
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., network issues, JSON parsing errors)
                RiseL.Text = "Error fetching sunrise time.";
                SetL.Text = "Error fetching sunset time.";
                DayL.Text = "Error fetching day length.";
            }
        }
        //Class for sunrise
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
            public string day_length { get; set; }
            public string CivilTwilightBegin { get; set; }
            public string CivilTwilightEnd { get; set; }
            public string NauticalTwilightBegin { get; set; }
            public string NauticalTwilightEnd { get; set; }
            public string AstronomicalTwilightBegin { get; set; }
            public string AstronomicalTwilightEnd { get; set; }
        }
        //GPS
        private async Task GetCoordinatesAsync()
        {
            try
            {
                // Check if location services are enabled
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location == null)
                {
                    // Request a fresh location if last known location is not available
                    location = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.High));
                }

                if (location != null)
                {
                    // Use location coordinates (latitude and longitude)
                    latitude = location.Latitude;
                    longitude = location.Longitude;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., permission denied, location unavailable)
                Welcome.Text = "Unable to get location: " + ex.Message;
                RiseL.Text = "Error";
                SetL.Text = "Error";
                DayL.Text = "Error";
            }
        }
        private async void SCCtoggle(object sender, EventArgs e)
        {
            if (!CordShow)
            {
                Cord.IsVisible = true;
                cordim.IsVisible = true;
                await GetCoordinatesAsync();
                CordShow = true;
                CordSB.Text = "Hide Current Coordinates";
                Cord.Text = $"Latitude: {latitude}, Longitude: {longitude}";
            }
            else
            {
                CordSB.Text = "Show Current Coordinates";
                Cord.IsVisible = false;
                cordim.IsVisible= false;
                CordShow = false;
            }
        }
    }
}
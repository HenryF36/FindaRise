using System;
using System.Net.Http;
using System.Security.AccessControl;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Dispatching;

namespace FindaRise
{
    public partial class MainPage : ContentPage
    {
        private const string ApiUrl = "https://api.sunrise-sunset.org/json";

        public MainPage()
        {
            InitializeComponent();
            GetSunriseSunsetTimes(); // Fetch sunrise and sunset times when the page loads
        }

        private async void GetSunriseSunsetTimes()
        {
            try
            {
                // Specify your location coordinates
                double latitude = 34.0522; // Los Angeles
                double longitude = -118.2437; // Los Angeles

                using HttpClient client = new HttpClient();
                string url = $"{ApiUrl}?lat={latitude}&lng={longitude}&formatted=0";
                string response = await client.GetStringAsync(url);

                // Log the raw response for debugging
                Console.WriteLine("API Response: " + response);

                // Deserialize the JSON response
                var result = JsonSerializer.Deserialize<SunriseSunsetResponse>(response);
                if (result != null && result.Results != null)
                {
                    // Convert the UTC time to local time
                    DateTime sunriseUtc = DateTime.Parse(result.Results.Sunrise);
                    DateTime sunsetUtc = DateTime.Parse(result.Results.Sunset);

                    // Update UI labels with sunrise and sunset times
                    RiseL.Text = $"The sun will rise at {sunriseUtc.ToLocalTime():hh:mm:ss tt}";
                    SetL.Text = $"The sun will set at {sunsetUtc.ToLocalTime():hh:mm:ss tt}";
                }
                else
                {
                    Console.WriteLine("Deserialization resulted in null.");
                    RiseL.Text = "Error fetching sunrise time.";
                    SetL.Text = "Error fetching sunset time.";
                }
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine("JSON Deserialization Error: " + jsonEx.Message);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                RiseL.Text = "Error fetching sunrise time.";
                SetL.Text = "Error fetching sunset time.";
                Console.WriteLine(ex.Message);
            }
        }
        private void CordTog(object sender, EventArgs e)
        {

        }


        public class SunriseSunsetResponse
        {
            public Results Results { get; set; }
            public string Status { get; set; } // Check response status
        }

        public class Results
        {
            public string Sunrise { get; set; }
            public string Sunset { get; set; }
        }
    }
}

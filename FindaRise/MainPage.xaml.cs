using System;
using System.Net.Http;
using System.Security.AccessControl;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Dispatching;
using Newtonsoft.Json;

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

        private void CordTog(object sender, EventArgs e) { 
        }


        
        private async void GetSunriseSunsetTimes()
        {
            try
            {
                // Specify your location coordinates 42.402830692019236, -83.44996959988292
                double latitude = 42.402830692019236;
                double longitude = -83.44996959988292; 

                // Create an HTTP client
                using HttpClient client = new HttpClient();
                string url = $"{ApiUrl}?lat={latitude}&lng={longitude}&formatted=0"; // Use formatted=0 for ISO 8601 format
                string response = await client.GetStringAsync(url);

                // Log the raw response for debugging
                RiseL.Text = "API Response: " + response;

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
                // Handle exceptions (e.g., network issues, JSON parsing errors)
                RiseL.Text = "Error fetching sunrise time.";
                SetL.Text = "Error fetching sunset time.";
                Console.WriteLine(ex.Message);
            }
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

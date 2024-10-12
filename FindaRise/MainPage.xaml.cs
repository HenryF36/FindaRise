using System;
using System.Timers;
using Microsoft.Maui.Dispatching;

namespace FindaRise
{
    public partial class MainPage : ContentPage
    {
        private static System.Timers.Timer _timer;

        public MainPage()
        {
            InitializeComponent();
            SetTimer();
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
        private void CordTog(object sender, EventArgs e)
        {
            
            Console.WriteLine("Coordinates toggled!");
            
        }
    }
}

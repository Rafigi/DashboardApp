using System;

namespace DashboardApp.Models
{
    public class WeatherForcastDto
    {
        public DateTime Datetime { get; set; }
        public float Temp { get; set; }
        public float CloudCover { get; set; }
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }
    }

}

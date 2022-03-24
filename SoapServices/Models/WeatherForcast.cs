using System;
namespace SoapServices.Models
{
    public class WeatherForcastDto
    {
        public float Temp { get; set; }
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }
        public float Windchill { get; set; }
        public DateTime datetime { get; set; }
    }

}

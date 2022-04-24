using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ServiceReference1;
using SoapServices.Models;

namespace SoapServices.Services
{

    public interface IWeatherForcastService
    {
        Task<List<WeatherForcastDto>> GetWeatherForcastAsync(string location);
    }


    public class WeatherForcastService : IWeatherForcastService
    {
        private readonly IOptions<SoapSettings> _soapSettings;

        public WeatherForcastService(IOptions<SoapSettings> soapSettings)
        {
            _soapSettings = soapSettings;
        }


        public async Task<List<WeatherForcastDto>> GetWeatherForcastAsync(string location)
        {
            var client = new ForecastServiceClient();
            var result = await client.GetForecastAsync(location, _soapSettings.Value.Password);

            Location selectedResult = result.Body.GetForecastResult.location;

            return selectedResult
                .MapToWeatherForcastDto();

        }
    }

    public static class ServiceHelper
    {
        public static List<WeatherForcastDto> MapToWeatherForcastDto(this Location currentLocation)
        {
            return currentLocation.values.Where(p => p.datetimeStr.Date == DateTime.Now.Date).Select(weather => new WeatherForcastDto()
            {
                CloudCover = weather.cloudcover ?? 0,
                Sunrise = currentLocation.currentConditions.sunrise,
                Sunset = currentLocation.currentConditions.sunset,
                Temp = weather.temp ?? 0,
                Datetime = weather.datetimeStr
            })
                .ToList();
        }
    }
}

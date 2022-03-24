using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ServiceReference1;
using SoapServices.Models;

namespace SoapServices.Services
{

    public interface IWeatherForcastService
    {
        Task<WeatherForcastDto> GetWeatherForcastAsync(string location);
    }


    public class WeatherForcastService : IWeatherForcastService
    {
        private readonly IOptions<SoapSettings> _soapSettings;

        public WeatherForcastService(IOptions<SoapSettings> soapSettings )
        {
            _soapSettings = soapSettings;
        }


        public async Task<WeatherForcastDto> GetWeatherForcastAsync(string location)
        {
            var client = new ForecastServiceClient();
            var result = await client.GetForecastAsync(location, _soapSettings.Value.Password);

            var selectedResult = result.Body.GetForecastResult.location.currentConditions;

            return selectedResult
                .MapToWeatherForcastDto();

        }
    }

    public static class ServiceHelper
    {
        public static WeatherForcastDto MapToWeatherForcastDto(this Currentconditions currentConditions)
        {
            return new WeatherForcastDto()
            {
                Sunrise = currentConditions.sunrise,
                Sunset = currentConditions.sunset,
                Temp = currentConditions.temp ?? 0,
                Windchill = currentConditions.windchill ?? 0
            };
        }

        public static string ConvertToJSON(this WeatherForcastDto weatherForcastDto)
            => JsonSerializer.Serialize(weatherForcastDto);


    }
}

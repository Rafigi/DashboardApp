using System.Text.Json;
using System.Threading.Tasks;
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




        public async Task<WeatherForcastDto> GetWeatherForcastAsync(string location)
        {
            var client = new ForecastServiceClient();
            var result = await client.GetForecastAsync(location, _key);

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

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SoapServices.Models;
using SoapServices.Services;

namespace WeatherServiceApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IWeatherForcastService _weatherForcastService;

        public WeatherForecastController(IWeatherForcastService weatherForcastService)
        {
            _weatherForcastService = weatherForcastService;
        }

        [HttpGet("{location}")]
        public async Task<List<WeatherForcastDto>> GetWeatherForcast(string location)
        {
            return await _weatherForcastService.GetWeatherForcastAsync(location);
        }
    }
}

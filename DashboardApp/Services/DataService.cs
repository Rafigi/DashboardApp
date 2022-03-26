using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DashboardApp.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace DashboardApp.Services
{
    public interface IDataService
    {
        Task NotifyDashBoardDataHasChanged();
        Task<List<WeatherForcastDto>> GetWeatherForcast(string location);
    }
    public class DataService : IDataService
    {
        private readonly IHubContext<SignalHub, ISignalHub> _context;
        private readonly HttpClient _client;

        public DataService(IHubContext<SignalHub, ISignalHub> context, HttpClient client)
        {
            _context = context;
            _client = client;
        }

        public async Task NotifyDashBoardDataHasChanged()
        {
            await _context.Clients.All.ReceiveNotification();
        }

        public async Task<List<WeatherForcastDto>> GetWeatherForcast(string location)
        {
            var apiName = "WeatherForecast/" + location;
            var httpResponse = await _client.GetAsync(apiName);
            if (httpResponse.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<List<WeatherForcastDto>>(await httpResponse.Content.ReadAsStringAsync());
            }

            return new List<WeatherForcastDto>();
        }
    }
}

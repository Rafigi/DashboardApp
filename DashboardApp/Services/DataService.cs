using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DashboardApp.Models;
using DatabaseService.Dtos;
using DatabaseService.Services;
using FTPServices.Models;
using FTPServices.Services;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace DashboardApp.Services
{
    public interface IDataService
    {
        Task NotifyDashBoardDataHasChanged();
        Task<List<WeatherForcastDto>> GetWeatherForcast(string location);
        Task<SolarPanel> GetSolarPanelPower();

        Task<List<TemperaturDto>> GetTemperaturFromInside(int take);
    }
    public class DataService : IDataService
    {
        private readonly IHubContext<SignalHub, ISignalHub> _context;
        private readonly HttpClient _client;
        private readonly IFtpServices _ftpServices;
        private readonly IDatabaseServices _databaseServices;

        public DataService(IHubContext<SignalHub, ISignalHub> context, HttpClient client, IFtpServices ftpServices, IDatabaseServices databaseServices)
        {
            _context = context;
            _client = client;
            _ftpServices = ftpServices;
            _databaseServices = databaseServices;
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

        public async Task<SolarPanel> GetSolarPanelPower()
        {
            return await _ftpServices.GetCalculateSolarPower();
        }

        public async Task<List<TemperaturDto>> GetTemperaturFromInside(int take)
        {
            return await _databaseServices.GetTemperatursAsync(take);
        }
    }
}

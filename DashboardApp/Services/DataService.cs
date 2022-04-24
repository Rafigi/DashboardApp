using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Confluent.Kafka;
using DashboardApp.Models;
using DatabaseService.Dtos;
using DatabaseService.Services;
using FTPServices.Models;
using FTPServices.Services;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using PowerPriceKafkaServices.Common;
using PowerPriceKafkaServices.Services;

namespace DashboardApp.Services
{
    public interface IDataService
    {
        Task NotifyFTPDataHasChanged();
        Task NotifyDatabaseDataHasChanged();
        Task<List<WeatherForcastDto>> GetWeatherForcast(string location);
        Task<SolarPanel> GetSolarPanelPower();

        Task<List<TemperaturDto>> GetTemperaturFromInside(int take);
        Task<string> SendKafkaMessageToKafkaPriceService();
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

        public async Task NotifyFTPDataHasChanged()
        {
            await _context.Clients.All.NotificationFtpChange();
        }

        public async Task NotifyDatabaseDataHasChanged()
        {
            await _context.Clients.All.NotificatioDatabaseChange();
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

        public async Task<string> SendKafkaMessageToKafkaPriceService()
        {
            Guid correlationId = Guid.NewGuid();
            Header header = new Header("correlationId", correlationId.ToByteArray());
            KafkaServices requestService = new RequestService();
            await requestService.ProduceMessageWithHeaderAsync(KafkaTopics.requestService, "Ask for new price", header);

            using (var consumer = new ReplyService().CreateConsumerBuilder().Build())
            {
                consumer.Subscribe(KafkaTopics.replyService);
                Console.WriteLine($"On Channel => {KafkaTopics.replyService}");
                ConsumeResult<Ignore, string> consumeResult;
                while (true)
                {
                    consumeResult = consumer.Consume();
                    var guid = consumeResult.Message.Headers.First(p => p.Key == "correlationId").GetValueBytes();
                    if (new Guid(guid) == correlationId)
                    {
                        consumer.Close();
                        consumer.Dispose();
                        break;
                    }
                }
                return consumeResult.Message.Value;
            }
        }
    }
}

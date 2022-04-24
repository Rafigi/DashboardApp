using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PowerPriceKafkaServices.Common;
using PowerPriceKafkaServices.Models;

namespace PowerPriceKafkaServices.Services
{
    public class DownloadServices : KafkaServices
    {
        public DownloadServices(string topic)
        {
            _topic = topic;
        }
        public override async void Consumer()
        {
            using (var consumer = CreateConsumerBuilder().Build())
            {
                consumer.Subscribe(_topic);
                Console.WriteLine($"DownloadService => Ready to listen On Channel => {_topic}");

                while (true)
                {
                    var consumeResult = consumer.Consume();
                        Root file = await DownloadFile();
                        Console.WriteLine("File is downloaded");
                        var result = file.data.Elspotprices;
                        var json = JsonConvert.SerializeObject(result);

                        await ProduceMessageWithHeadersAsync(KafkaTopics.cacheServiceSaveFileToCache, json,
                            consumeResult.Message.Headers);
                }
            }
        }

        private async Task<Root> DownloadFile()
        {
            var files = await GetTodayPricesJsonAsync();

            return JsonConvert.DeserializeObject<Root>(files);
        }

        private async Task<string> GetTodayPricesJsonAsync()
        {
            HttpClient client = new HttpClient();
            const string payload = @"{"" operationName"": ""Dataset"",  ""variables"": {}, ""query"": "" query Dataset { elspotprices (where: {PriceArea: {_eq:\""DK1\""}}, order_by: {HourUTC:desc}, limit:48, offset:0) {HourUTC, HourDK, PriceArea, SpotPriceDKK, SpotPriceEUR}} ""}";

            var response = await client.PostAsync("https://data-api.energidataservice.dk/v1/graphql",
                new StringContent(payload, Encoding.UTF8, "application/json"));

            var result = await response.Content.ReadAsStringAsync();

            return result;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka;
using Newtonsoft.Json;
using PowerPriceKafkaServices.Common;
using PowerPriceKafkaServices.Models;

namespace PowerPriceKafkaServices.Services
{
    public class TransformServices : KafkaServices
    {
        public TransformServices(string topic)
        {
            _topic = topic;
        }

        public override async void Consumer()
        {
            using (var consumer = CreateConsumerBuilder().Build())
            {
                consumer.Subscribe(_topic);
                consumer.Assign(new TopicPartition(_topic, 0));
                Console.WriteLine($"TransformServices => Ready to listen On Channel => {_topic}");

                while (true)
                {
                    var consumeResult = consumer.Consume();
                    string price = TransformData(consumeResult.Message.Value);
                    await ProduceMessageWithHeadersAsync(KafkaTopics.replyService, price, consumeResult.Message.Headers);
                }
            }
        }

        private string TransformData(string message)
        {
            DateTime now = DateTime.Now;
            now = now.AddMinutes(-now.Minute).AddSeconds(-now.Second);
            List<Elspotprice> elSpotPrices =
                JsonConvert.DeserializeObject<List<Elspotprice>>(message) ?? new List<Elspotprice>();

            Elspotprice spotPrice = elSpotPrices.FirstOrDefault(el => el.HourDK.Date == now.Date && el.PriceArea == "DK1");
            if (spotPrice == null)
                return "0,00";

            var price = spotPrice.SpotPriceDKK ?? (spotPrice.SpotPriceEUR.Value * 7.5);

            return (Math.Round(price / 1000, 2)).ToString("F");
        }


    }
}

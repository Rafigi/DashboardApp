using System;
using PowerPriceKafkaServices.Common;

namespace PowerPriceKafkaServices.Services
{
    public class RequestService : KafkaServices
    {
        public RequestService()
        {

        }
        public RequestService(string topic)
        {
            _topic = topic;
        }
        public override async void Consumer()
        {
            using (var consumer = CreateConsumerBuilder().Build())
            {
                consumer.Subscribe(KafkaTopics.requestService);
                Console.WriteLine($"RequestService => Ready to listen On Channel => { KafkaTopics.requestService}");
                while (true)
                {
                    var consumeResult = consumer.Consume();
                    await ProduceMessageWithHeadersAsync(KafkaTopics.cacheServiceIsCached, consumeResult.Message.Value, consumeResult.Message.Headers);
                }
            }
        }
    }
}

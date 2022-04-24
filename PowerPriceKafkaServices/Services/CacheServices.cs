using System;
using System.IO;
using System.Threading.Tasks;
using PowerPriceKafkaServices.Common;

namespace PowerPriceKafkaServices.Services
{
    public class CacheServices : KafkaServices
    {

        public CacheServices(string topic)
        {
            _topic = topic;
        }

        public override async void Consumer()
        {
            using (var consumer = CreateConsumerBuilder().Build())
            {
                consumer.Subscribe(_topic);
                Console.WriteLine($"CacheServices => Ready to listen On Channel => {_topic}");
                if (KafkaTopics.cacheServiceIsCached == _topic)
                {
                    while (true)
                    {


                        var consumeResult = consumer.Consume();

                        if (IsCached)
                        {
                            Console.WriteLine("The filed has been cached");
                            var file = await GetCacheAsync();
                            await ProduceMessageWithHeadersAsync(KafkaTopics.TransformServiceTransformData, file, consumeResult.Message.Headers);
                        }
                        else
                        {
                            Console.WriteLine("The files has not been cached, and needed to be downloaded");
                            await ProduceMessageWithHeadersAsync(KafkaTopics.downloadServiceDownloadFile, "", consumeResult.Message.Headers);
                        }

                    }
                }

                if (KafkaTopics.cacheServiceSaveFileToCache == _topic)
                {
                    while (true)
                    {
                        var consumeResult = consumer.Consume();
                        Console.WriteLine("The file is being saved to the cache");
                        await SaveCacheAsync(consumeResult.Message.Value);
                        await ProduceMessageWithHeadersAsync(KafkaTopics.TransformServiceTransformData, consumeResult.Message.Value, consumeResult.Message.Headers);
                    }

                }

            }
        }

        private string GetFileName()
        {
            var date = DateTime.Now.ToShortDateString().Replace('/', '-');
            return $"prices-{date}.txt";
        }

        private bool IsCached => File.Exists(GetFileName());
        private async Task<string> GetCacheAsync() => await File.ReadAllTextAsync(GetFileName());

        private async Task SaveCacheAsync(string file)
        {
            await File.WriteAllTextAsync(GetFileName(), file);
        }
    }
}

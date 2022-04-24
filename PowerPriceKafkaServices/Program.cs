using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PowerPriceKafkaServices.Common;
using PowerPriceKafkaServices.Services;

namespace PowerPriceKafkaServices
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await SetUpConsumers();
        }

        private static Task SetUpConsumers()
        {
            KafkaConsumerServices().ForEach((c) =>
            {
                c.thread = new Thread(c.Consumer);
                c.thread.Start();
            });
            return Task.CompletedTask;
        }

        private static List<KafkaServices> KafkaConsumerServices() => new()
        {
            new RequestService(KafkaTopics.requestService),
            new CacheServices(KafkaTopics.cacheServiceIsCached),
            new CacheServices(KafkaTopics.cacheServiceSaveFileToCache),
            new DownloadServices(KafkaTopics.downloadServiceDownloadFile),
            new TransformServices(KafkaTopics.TransformServiceTransformData),
        };
    }
}

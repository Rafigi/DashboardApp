using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace PowerPriceKafkaServices.Common
{
    public abstract class KafkaServices
    {
        public Thread thread;
        public string _topic;
        private readonly ConsumerConfig consumerConfig = new()
        {
            BootstrapServers = "localhost:29092",
            GroupId = "Gruppe",
        };

        private readonly ProducerConfig producerConfig = new()
        {
            BootstrapServers = "localhost:29092",
            ClientId = Dns.GetHostName()
        };

        public async Task ProduceMessageWithHeadersAsync(string channel, string message, Headers headers)
        {
            IProducer<Null, string> producer = new ProducerBuilder<Null, string>(producerConfig).Build();
            await producer.ProduceAsync(channel, new Message<Null, string>() { Headers = headers, Timestamp = new Timestamp(DateTime.Now), Value = message });
            Console.WriteLine($"A Message to channel {channel} has been process");
        }

        public async Task ProduceMessageWithHeaderAsync(string channel, string message, Header header)
        {
            var headers = new Headers();
            headers.Add(header.Key, header.GetValueBytes());
            IProducer<Null, string> producer = new ProducerBuilder<Null, string>(producerConfig).Build();
            await producer.ProduceAsync(channel, new Message<Null, string>() { Headers = headers, Timestamp = new Timestamp(DateTime.Now), Value = message });

            Console.WriteLine($"A Message to channel {channel} has been process");
        }

        public ConsumerBuilder<Ignore, string> CreateConsumerBuilder()
        {
            return new ConsumerBuilder<Ignore, string>(consumerConfig);
        }

        public abstract void Consumer();
    }
}

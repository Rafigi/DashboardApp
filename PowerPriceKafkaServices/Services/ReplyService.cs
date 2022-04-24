using PowerPriceKafkaServices.Common;

namespace PowerPriceKafkaServices.Services
{
    public class ReplyService : KafkaServices
    {
        public ReplyService()
        {

        }

        public ReplyService(string topic)
        {
            _topic = topic;
        }

        public override void Consumer()
        {

        }
    }
}

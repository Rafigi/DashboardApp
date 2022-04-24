namespace PowerPriceKafkaServices.Common
{
    public struct KafkaTopics
    {
        public const string TransformServiceTransformData = "transformService-transformData";
        public const string downloadServiceDownloadFile = "downloadService-downloadFile";
        public const string cacheServiceIsCached = "cacheService-IsCached";
        public const string cacheServiceSaveFileToCache = "cacheService-SaveFileToCache";
        public const string requestService = "requestService";
        public const string replyService = "replyService";
    }
}

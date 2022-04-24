 Write-Output  "Create all needed topics" 
 docker exec kafka_kafka_1 kafka-topics --create --bootstrap-server localhost:29092 --partitions 1 --replication-factor 1 --topic transformService-transformData
 docker exec kafka_kafka_1 kafka-topics --create --bootstrap-server localhost:29092 --partitions 1 --replication-factor 1 --topic downloadService-downloadFile
 docker exec kafka_kafka_1 kafka-topics --create --bootstrap-server localhost:29092 --partitions 1 --replication-factor 1 --topic cacheService-IsCached
 docker exec kafka_kafka_1 kafka-topics --create --bootstrap-server localhost:29092 --partitions 1 --replication-factor 1 --topic cacheService-SaveFileToCache
 docker exec kafka_kafka_1 kafka-topics --create --bootstrap-server localhost:29092 --partitions 1 --replication-factor 1 --topic requestService
 docker exec kafka_kafka_1 kafka-topics --create --bootstrap-server localhost:29092 --partitions 1 --replication-factor 1 --topic replyService
using Confluent.Kafka;
using Newtonsoft.Json;
using System;


namespace DwitTech.AccountService.Core.EventsHandler
{
    public class EventPublisher
    {
        private readonly ProducerConfig _producerConfig;

        public EventPublisher(ProducerConfig producerConfig)
        {
            _producerConfig = producerConfig;
        }

        public void PublishEvent<T>(string topic, T eventData)
        {
            using (var producer = new ProducerBuilder<string, string>(_producerConfig).Build())
            {
                try
                {
                    var serializedData = JsonConvert.SerializeObject(eventData);
                    producer.Produce(topic, new Message<string, string>
                    {
                        Value = serializedData
                    });

                    producer.Flush(TimeSpan.FromSeconds(10));
                }
                catch (ProduceException<string, string> ex)
                {
                    throw new Exception($"Failed to send event to Kafka: {ex.Error.Reason}", ex);
                }
            }
        }
    }
}

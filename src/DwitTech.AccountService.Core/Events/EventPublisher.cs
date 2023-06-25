using Confluent.Kafka;
using DwitTech.AccountService.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DwitTech.AccountService.Core.Events
{
    public class EventPublisher:IEventPublisher
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<EventPublisher> _logger;

        public EventPublisher(IProducer<string, string> producer, ILogger<EventPublisher> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task<bool> PublishEmailEventAsync<T>(string topic, T eventData)
        {
            try
            {
                var serializedData = JsonConvert.SerializeObject(eventData);
                await _producer.ProduceAsync(topic, new Message<string, string>
                {
                    Value = serializedData
                });

                _producer.Flush(TimeSpan.FromSeconds(3));
                _logger.LogInformation("Email published successfully");
                return true;
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.LogError(ex, "Error! Unable to publish event!");
                throw;
            }
        }
    }
}

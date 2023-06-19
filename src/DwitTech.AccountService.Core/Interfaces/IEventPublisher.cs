using DwitTech.AccountService.Core.Models;

namespace DwitTech.AccountService.Core.Interfaces
{
    public interface IEventPublisher
    {
        Task<bool> PublishEmailEventAsync<T>(string topic, T eventData);
    }
}

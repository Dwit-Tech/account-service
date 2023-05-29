


namespace DwitTech.AccountService.Core.Models
{
    public abstract class BaseEvent
    {
        public string EventType { get; set; }
        public DateTime TimeStamp { get; set; }
        public string EmailAddress { get; set; }
    }
}

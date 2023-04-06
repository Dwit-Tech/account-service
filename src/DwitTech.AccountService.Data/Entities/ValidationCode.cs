using DwitTech.AccountService.Data.Enum;

namespace DwitTech.AccountService.Data.Entities
{
    public class ValidationCode : BaseEntity
    {
        public int UserId { set; get; }
        public string Code { set; get; }
        public CodeType CodeType { set; get; }
        public NotificationChannel NotificationChannel { set; get; }
    }
}

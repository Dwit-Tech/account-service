using System.ComponentModel.DataAnnotations;

namespace DwitTech.AccountService.Data.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedOnUtc { get; private set; } = DateTime.UtcNow;
        public DateTime? ModifiedOnUtc { get; set; }
    }
}

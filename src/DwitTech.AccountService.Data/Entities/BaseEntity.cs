using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Data.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedOnUtc { get; } = DateTime.UtcNow;
        public DateTime? ModifiedOnUtc { get; set; }
    }
}

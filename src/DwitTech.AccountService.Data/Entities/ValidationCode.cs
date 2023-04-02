using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Data.Entities
{
    public class ValidationCode : BaseEntity
    {
        public int UserId { set; get; }
        public string Code { set; get; }
        public int CodeType { set; get; }
        public int NotificationChannel { set; get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Exceptions
{
    public class EntityNotFoundException:Exception
    {
        public override string Message
        {
            get
            {
                return "The specified entity does not exist in the database";
            }
        }
    }
}

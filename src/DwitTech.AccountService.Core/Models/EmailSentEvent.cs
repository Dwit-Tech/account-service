using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Models
{
    public class EmailSentEvent : BaseEvent
    {
        public EmailSentEvent()
        {
            EventType = "Email-Sent-Event";
            TimeStamp = DateTime.UtcNow;
            //Add ID?
        }
    }
}

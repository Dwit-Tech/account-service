using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DwitTech.AccountService.Core.Models;

namespace DwitTech.AccountService.Core.Dtos
{
    [AutoMap(typeof(Email), ReverseMap = true)]
    public class EmailDto : Profile
    {
        [EmailAddress]
        [Required]
        public string FromEmail { get; set; }

        [EmailAddress]
        [Required]
        public string ToEmail { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        [Required]
        public string Cc { get; set; }

        public string Bcc { get; set; }


    }
}

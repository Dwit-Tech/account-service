using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using DwitTech.AccountService.Core;
using DwitTech.AccountService.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace DwitTech.AccountService.Data.Entities
{
    public class ValidationCode
    {
        [Key]
        public string? UserId { set; get; }
        public string Code { set; get; }
        public ActivationType ActivationType { set; get; }
        public ActivationChannel ActivationChannel { set; get; }


    }
}
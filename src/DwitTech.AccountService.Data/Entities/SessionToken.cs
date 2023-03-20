﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Data.Entities
{
    public class SessionToken:BaseEntity
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string RefreshToken{ get; set; }
    }
}
﻿using DwitTech.AccountService.Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Data.Entities
{
    public class User : BaseEntity
    {
        [Key]
        [Required]
        [MaxLength(25)]
        public int Id { get; set; }

        [Required]
        [MaxLength(25)]
        public string Firstname { get; set; }

        [Required]
        [MaxLength(25)]
        public string Lastname { get; set; }

        [Required]
        [MaxLength(45)]
        public string Email { get; set; }

        [Required]
        [MaxLength(11)]
        public string PhoneNumber { get; set; }

        [Required]
        public string AddressLine1 { get; set; }

        [Required]
        public string AddressLine2 { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string PostalCode { get; set; }

        [Required]
        public string ZipCode { get; set; }

        [Required]
        [Range(8,16)]
        public string Password { get; set; }

        public UserStatus Status { get; set; }

        public Role Roles { get; set; }
        
    }
}

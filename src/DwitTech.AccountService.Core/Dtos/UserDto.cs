using AutoMapper;
using DwitTech.AccountService.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Dtos
{
    public abstract class UserDto: Profile 
    {
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
        public int PhoneNumber { get; set; }
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

        public UserStatus Status { get; set; }

    }

}

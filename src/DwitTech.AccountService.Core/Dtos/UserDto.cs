﻿using AutoMapper;
using DwitTech.AccountService.Core.CustomAnnotations;
using DwitTech.AccountService.Core.Enums;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace DwitTech.AccountService.Core.Dtos
{
    [AutoMap(typeof(User), ReverseMap = true)]
    public class UserDto
    {
        [Required]
        [MaxLength(25)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(25)]
        public string LastName { get; set; }

        [Required]
        [ValidatePassword]
        public string PassWord { get; set; }

        [Required]
        [MaxLength(45)]
        [EmailAddress]
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
        public string CountryCode { get; set; }

        public Enums.Role Roles { get; set; }
    }

}

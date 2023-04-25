using AutoMapper;
using DwitTech.AccountService.Core.CustomAnnotations;
using DwitTech.AccountService.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DwitTech.AccountService.Core.Dtos
{
    [AutoMap(typeof(User), ReverseMap = true)]
    public class EditRequestDto
    {
        [JsonPropertyName("firstName")]
        [Required]
        [MaxLength(25)]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        [Required]
        [MaxLength(25)]
        public string LastName { get; set; }

        [JsonPropertyName("email")]
        [Required]
        [MaxLength(45)]
        [EmailAddress]
        public string Email { get; set; }

        [JsonPropertyName("phoneNumber")]
        [Required]
        [MaxLength(11)]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("addressLine1")]
        [Required]
        public string AddressLine1 { get; set; }

        [JsonPropertyName("addressLine2")]
        [Required]
        public string AddressLine2 { get; set; }

        [JsonPropertyName("country")]
        [Required]
        public string Country { get; set; }

        [JsonPropertyName("state")]
        [Required]
        public string State { get; set; }

        [JsonPropertyName("city")]
        [Required]
        public string City { get; set; }

        [JsonPropertyName("postalCode")]
        [Required]
        public string PostalCode { get; set; }

        [JsonPropertyName("zipCode")]
        [Required]
        public string ZipCode { get; set; }
    }
}

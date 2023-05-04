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
        [MaxLength(25)]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        [MaxLength(25)]
        public string LastName { get; set; }

        [JsonPropertyName("email")]
        [MaxLength(45)]
        [EmailAddress]
        public string Email { get; set; }

        [JsonPropertyName("phoneNumber")]
        [MaxLength(11)]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("addressLine1")]
        public string AddressLine1 { get; set; }

        [JsonPropertyName("addressLine2")]
        public string AddressLine2 { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("postalCode")]
        public string PostalCode { get; set; }

        [JsonPropertyName("zipCode")]
        public string ZipCode { get; set; }
    }
}

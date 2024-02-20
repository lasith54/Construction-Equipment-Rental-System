using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.Xml;

namespace RentalSystem.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [MaxLength(12)]
        public string? NIC { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Username { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [MinLength(8)]
        public String? Password { get; set; }
        public string? Address { get; set; }
        [Phone]
        public string? ContactNo { get; set; }
        public string? Role { get; set; }
        public List<Product>? Rents { get; set; }

        public string FullName
        {
            get
            {
                return $"{Firstname}{Lastname}";
            }
        }
    }
}

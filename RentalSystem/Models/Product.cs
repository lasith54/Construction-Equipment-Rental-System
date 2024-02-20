using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalSystem.Models
{
	public class Product
	{
		public int ProductId { get; set; }

		[NotMapped]
		public IFormFile? ProductImage { get; set; }

		[StringLength(500)]
		public string? ImageUrl { get; set; }

		[Required]
		public string? Productname { get; set; }

		[Required]
		public string? Manufacturer { get; set; }
		public string? Description { get; set; }

		[Required]
		public decimal Rent { get; set; }

		[Required]
		public int Quantity { get; set; }
	}
}

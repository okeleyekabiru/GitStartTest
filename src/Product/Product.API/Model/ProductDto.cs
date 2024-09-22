using System.ComponentModel.DataAnnotations;

namespace Product.API.Model
{
    public class ProductDto
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int StockQuantity { get; set; }
    }

    public class UpdateProductDto
    {
        [Required]
        public Guid? Id { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? StockQuantity { get; set; }
    }
}
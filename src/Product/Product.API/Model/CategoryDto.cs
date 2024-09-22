using System.ComponentModel.DataAnnotations;

namespace Product.API.Model
{
    public class CategoryDto
    {
        [Required]
        public string Name { get; set; } 
        [Required]
        public string? Description { get; set; }
    }

}
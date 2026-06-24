using System.ComponentModel.DataAnnotations;

namespace main.Models
{
    public class PropertyImage
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Image is required.")]
        public string ImageUrl { get; set; } = string.Empty;
        
        public int PropertyId { get; set; }
        public Property Property { get; set; } = null!;
    }
}

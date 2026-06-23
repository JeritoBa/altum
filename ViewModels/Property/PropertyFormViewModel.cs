using System.ComponentModel.DataAnnotations;

namespace main.ViewModels.Property
{
    public class PropertyFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Property Name is required.")]
        [StringLength(100, ErrorMessage = "Property Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required.")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country is required.")]
        [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters.")]
        public string Country { get; set; } = string.Empty;

        [Required(ErrorMessage = "Max Guests is required.")]
        [Range(1, 1000, ErrorMessage = "Max Guests must be between 1 and 1000.")]
        public int MaxGuests { get; set; }

        [Required(ErrorMessage = "Bedrooms is required.")]
        [Range(0, 100, ErrorMessage = "Bedrooms cannot exceed 100.")]
        public int Bedrooms { get; set; }

        [Required(ErrorMessage = "Bathrooms is required.")]
        [Range(0, 100, ErrorMessage = "Bathrooms cannot exceed 100.")]
        public int Bathrooms { get; set; }

        [Required(ErrorMessage = "Main Image is required.")]
        public string MainImageUrl { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;
    }
}

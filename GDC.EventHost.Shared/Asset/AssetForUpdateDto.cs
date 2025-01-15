using System.ComponentModel.DataAnnotations;

namespace GDC.EventHost.Shared.Asset
{
    public class AssetForUpdateDto
    {
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "You should enter a Name.")]
        [MaxLength(150, ErrorMessage = "The Uri should not be longer than 150 characters.")]
        public required string Name { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "You should enter a Description.")]
        [MaxLength(250, ErrorMessage = "The Description should not be longer than 250 characters.")]
        public required string Description { get; set; }

        [Display(Name = "Uri")]
        [MaxLength(500, ErrorMessage = "The Uri should not be longer than 500 characters.")]
        public required string Uri { get; set; }
    }
}

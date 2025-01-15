using System.ComponentModel.DataAnnotations;

namespace GDC.EventHost.Shared.PerformanceType
{
    public class PerformanceTypeForUpdateDto
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "You should enter a Name.")]
        [MaxLength(150, ErrorMessage = "The Performance Type Name should not be longer than 150 characters.")]
        public required string Name { get; set; }

        [Display(Name = "Description")]
        [MaxLength(250, ErrorMessage = "The Performance Type Description should not be longer than 250 characters.")]
        public string? Description { get; set; }
    }
}
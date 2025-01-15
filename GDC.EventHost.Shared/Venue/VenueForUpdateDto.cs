using GDC.EventHost.Shared.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.Shared.Venue
{
    public class VenueForUpdateDto
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "You should enter a name.")]
        [MaxLength(150, ErrorMessage = "The name should not be longer than 150 characters.")]
        public required string Name { get; set; }

        [Display(Name = "Description")]
        [MaxLength(250, ErrorMessage = "The description should not be longer than 250 characters.")]
        public string? Description { get; set; }

        [Display(Name = "Long Description")]
        [MaxLength(1500, ErrorMessage = "The long description should not be longer than 1500 characters.")]
        public string? LongDescription { get; set; }

        [Display(Name = "Status Id")]
        [Required(ErrorMessage = "You should enter a Status Id.")]
        [StatusIsValid]
        public required StatusEnum StatusId { get; set; }
    }
}
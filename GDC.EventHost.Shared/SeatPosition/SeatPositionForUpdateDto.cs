using System.ComponentModel.DataAnnotations;

namespace GDC.EventHost.Shared.SeatPosition
{
    public class SeatPositionForUpdateDto
    {
        public Guid Id { get; set; }

        [Display(Name = "Level")]
        [Required(ErrorMessage = "You should enter a Level for this Seat Position identifier.")]
        [Range(1, 5, ErrorMessage = "Only 5 Seat Position Levels are allowed.")]
        public required int Level { get; set; }

        [Display(Name = "Number")]
        [Required(ErrorMessage = "You should enter a number of Seat Positions.")]
        [Range(1, 25, ErrorMessage = "There should be at least one and no more than 25 Seat Positions of this type.")]
        public required int Number { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "You should enter a Seat Position name.")]
        [MaxLength(10, ErrorMessage = "The Seat Position name should not be longer than 10 characters.")]
        public required string Name { get; set; }
    }
}
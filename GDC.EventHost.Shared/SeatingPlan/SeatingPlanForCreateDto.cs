using GDC.EventHost.Shared.SeatPosition;
using System.ComponentModel.DataAnnotations;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.Shared.SeatingPlan
{
    public class SeatingPlanForCreateDto
    {
        [Display(Name = "Name")]
        [MaxLength(150, ErrorMessage = "Seating plan name should not be longer than 150 characters.")]
        [Required(ErrorMessage = "You should enter a Name.")]
        public required string Name { get; set; }

        [Display(Name = "Status")]
        [Required(ErrorMessage = "You should enter a Status.")]
        public StatusEnum StatusId { get; set; }

        [Display(Name = "Venue")]
        [RegularExpression(@"^[A-Za-z0-9]{8}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{12}$",
            ErrorMessage = "If specified, the Venue Id must be a valid Guid.")]
        public Guid VenueId { get; set; }

        public IEnumerable<SeatPositionsForCreateDto> SeatPositions { get; set; } = [];

        [Display(Name = "Seats")]
        [Range(1,25, ErrorMessage = "Should be at least one and no more than 25 seats per row.")]
        [Required(ErrorMessage = "You should enter the maximum number of seats in each row.")]
        public int NumberOfSeatsPerRow { get; set; }
    }
}
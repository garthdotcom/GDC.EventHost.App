using System.ComponentModel.DataAnnotations;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.Shared.Ticket
{
    public class TicketForUpdateDto
    {
        public Guid Id { get; set; }

        [Display(Name = "Number")]
        [Required(ErrorMessage = "You should enter a Ticket Number.")]
        [MaxLength(35, ErrorMessage = "The Ticket Number should not be longer than 35 characters.")]
        public required string Number { get; set; }

        [Display(Name = "Price")]
        [Required(ErrorMessage = "You should enter a Ticket Price.")]
        [RegularExpression(@"^\d{1,3}(\.\d{1,2})?$", ErrorMessage = "The Price needs to have two decimal places.")]
        public decimal Price { get; set; }

        [Display(Name = "Status")]
        [Required(ErrorMessage = "You should enter a Ticket Status.")]
        public TicketStatusEnum TicketStatusId { get; set; }

        [Display(Name = "Performance")]
        [Required(ErrorMessage = "You should enter a Performance Id.")]
        [RegularExpression(@"^[A-Za-z0-9]{8}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{12}$", 
            ErrorMessage = "The Performance Id must be a valid Guid.")]
        public Guid EventId { get; set; }

        [Display(Name = "Seat")]
        [Required(ErrorMessage = "You should enter a Seat Id.")]
        [RegularExpression(@"^[A-Za-z0-9]{8}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{12}$",
            ErrorMessage = "The Seat Id must be a valid Guid.")]
        public Guid SeatId { get; set; }
    }
}
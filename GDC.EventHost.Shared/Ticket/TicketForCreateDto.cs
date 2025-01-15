using System.ComponentModel.DataAnnotations;

namespace GDC.EventHost.Shared.Ticket
{
    public class TicketForCreateDto
    {
        [Display(Name = "Performance")]
        [Required(ErrorMessage = "You should enter a Performance Id.")]
        [RegularExpression(@"^[A-Za-z0-9]{8}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{12}$",
            ErrorMessage = "The Performance Id must be a valid Guid.")]
        public Guid PerformanceId { get; set; }

        [Display(Name = "Layout")]
        [Required(ErrorMessage = "You should enter a Layout Id.")]
        [RegularExpression(@"^[A-Za-z0-9]{8}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{12}$",
            ErrorMessage = "The Layout Id must be a valid Guid.")]
        public Guid LayoutId { get; set; }

        [Display(Name = "Basic Ticket Price")]
        [Required(ErrorMessage = "You should enter a Basic Ticket Price.")]
        [RegularExpression(@"^\d{1,3}(\.\d{1,2})?$", ErrorMessage = "The Basic Ticket Price must have two decimal places.")]
        public decimal BasicTicketPrice { get; set; }
    }
}
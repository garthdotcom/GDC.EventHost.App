using GDC.EventHost.Shared.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.Shared.Performance
{
    public class PerformanceForUpdateDto
    {
        [Display(Name = "Date")]
        [Required(ErrorMessage = "You should enter a Performance Date.")]
        [DateIsFutureOrNull]
        public required DateTime Date { get; set; }

        [Display(Name = "Title")]
        public string? Title { get; set; }

        [Display(Name = "Performance Type Id")]
        [Required(ErrorMessage = "You should enter a Performance Type Id.")]
        [RegularExpression(@"^[A-Za-z0-9]{8}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{12}$",
            ErrorMessage = "The Performance Type Id must be a valid Guid.")]
        public required Guid PerformanceTypeId { get; set; }

        [Display(Name = "Event Id")]
        [Required(ErrorMessage = "You should enter an Event Id.")]
        [RegularExpression(@"^[A-Za-z0-9]{8}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{12}$",
            ErrorMessage = "The Event Id must be a valid Guid.")]
        public required Guid EventId { get; set; }

        [Display(Name = "Venue Id")]
        [Required(ErrorMessage = "You should enter a Venue Id.")]
        [RegularExpression(@"^[A-Za-z0-9]{8}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{12}$",
            ErrorMessage = "The Venue Id must be a valid Guid.")]
        public required Guid VenueId { get; set; }

        [Display(Name = "Status Id")]
        [Required(ErrorMessage = "You should enter a Status Id.")]
        [StatusIsValid]
        public required StatusEnum StatusId { get; set; }

    }
}
using System.ComponentModel.DataAnnotations;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.Shared.Performance
{
    public class PerformanceDto
    {
        public required Guid Id { get; set; }

        [Display(Name = "Date")]
        public required DateTime Date { get; set; }

        [Display(Name = "Title")]
        public string? Title { get; set; }

        [Display(Name = "Performance Type Id")]
        public required Guid PerformanceTypeId { get; set; }

        [Display(Name = "Event Id")]
        public required Guid EventId { get; set; }

        [Display(Name = "Status Id")]
        public required StatusEnum StatusId { get; set; }

        [Display(Name = "Venue Id")]
        public required Guid VenueId { get; set; }
    }
}
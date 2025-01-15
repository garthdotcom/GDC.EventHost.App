using GDC.EventHost.Shared.Performance;
using GDC.EventHost.Shared.Seat;
using System.ComponentModel.DataAnnotations;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.Shared.SeatingPlan
{
    public class SeatingPlanDetailDto
    {
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public required string Name { get; set; }

        [Display(Name = "Status")]
        public StatusEnum StatusId { get; set; }

        [Display(Name = "Status Value")]
        public string StatusValue { get; set; } = string.Empty;

        [Display(Name = "Venue")]
        public Guid VenueId { get; set; }

        [Display(Name = "Venue Name")]
        public string VenueName { get; set; } = string.Empty;

        public List<SeatDisplayDto> SeatsForDisplay { get; set; } = [];

        public List<PerformanceDetailDto> Performances { get; set; } = [];
    }
}

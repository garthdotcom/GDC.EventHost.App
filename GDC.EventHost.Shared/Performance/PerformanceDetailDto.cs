using GDC.EventHost.Shared.PerformanceAsset;
using System.ComponentModel.DataAnnotations;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.Shared.Performance
{
    public class PerformanceDetailDto
    {
        public required Guid Id { get; set; }

        [Display(Name = "Date")]
        public required DateTime Date { get; set; }

        [Display(Name = "Title")]
        public string? Title { get; set; }

        [Display(Name = "Performance Type Id")]
        public required Guid PerformanceTypeId { get; set; }

        [Display(Name = "Performance Type Name")]
        public string? PerformanceTypeName { get; set; }

        [Display(Name = "Status Id")]
        public required StatusEnum StatusId { get; set; }

        [Display(Name = "Status Value")]
        public string? StatusValue { get; set; }

        [Display(Name = "Event Id")]
        public required Guid EventId { get; set; }

        [Display(Name = "Event Title")]
        public string? EventTitle { get; set; }

        [Display(Name = "Venue Id")]
        public Guid? VenueId { get; set; }

        [Display(Name = "Venue Name")]
        public string? VenueName { get; set; }

        [Display(Name = "SeatingPlan Id")]
        public Guid? SeatingPlanId { get; set; }

        [Display(Name = "SeatingPlan Name")]
        public string? SeatingPlanName { get; set; }

        public PerformanceTicketCount? TicketCount { get; set; }

        public List<PerformanceAssetDto> PerformanceAssets { get; set; } = [];
    }
}
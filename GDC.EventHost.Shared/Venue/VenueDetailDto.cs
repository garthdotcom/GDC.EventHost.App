using GDC.EventHost.Shared.Performance;
using System.ComponentModel.DataAnnotations;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.Shared.Venue
{
    public class VenueDetailDto
    {
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public required string Name { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Long Description")]
        public string? LongDescription { get; set; }

        [Display(Name = "Status Id")]
        public required StatusEnum StatusId { get; set; }

        [Display(Name = "Status Value")]
        public string? StatusValue { get; set; } = string.Empty;

        public List<PerformanceDetailDto> Performances { get; set; } = [];

        //public List<VenueAssetDto> VenueAssets { get; set; } = [];

        //public List<SeatingPlanDto> SeatingPlans { get; set; } = [];
    }
}
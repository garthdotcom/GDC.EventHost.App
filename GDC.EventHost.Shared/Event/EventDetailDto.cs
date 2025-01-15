using GDC.EventHost.Shared.Performance;
using System.ComponentModel.DataAnnotations;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.Shared.Event
{
    public class EventDetailDto
    {
        public Guid Id { get; set; }

        [Display(Name = "Title")]
        public required string Title { get; set; }

        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Long Description")]
        public string? LongDescription { get; set; }

        [Display(Name = "Series Id")]
        public Guid? SeriesId { get; set; }

        [Display(Name = "Series Title")]
        public string? SeriesTitle { get; set; }

        [Display(Name = "Status Id")]
        public required StatusEnum StatusId { get; set; }
        [Display(Name = "Status Value")]
        public string? StatusValue { get; set; }

        public List<PerformanceDetailDto> Performances { get; set; } = [];

        //public List<EventAssetDto> EventAssets { get; set; } = [];
    }
}
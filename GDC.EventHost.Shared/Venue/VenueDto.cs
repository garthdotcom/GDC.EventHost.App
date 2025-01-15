using GDC.EventHost.Shared.Performance;
using System.ComponentModel.DataAnnotations;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.Shared.Venue
{
    public class VenueDto
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
    }
}
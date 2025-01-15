using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.Shared.SeatingPlan
{
    public class SeatingPlanDto
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public StatusEnum StatusId { get; set; }

        public Guid VenueId { get; set; }
    }
}
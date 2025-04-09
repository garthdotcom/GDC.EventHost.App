using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.Shared.Performance
{
    public class PerformanceForCreateDto
    {
        public DateTime? Date { get; set; }

        public string? Title { get; set; }

        public Guid? PerformanceTypeId { get; set; }

        public required Guid EventId { get; set; }

        public Guid? VenueId { get; set; }

        public StatusEnum? StatusId { get; set; }
    }
}
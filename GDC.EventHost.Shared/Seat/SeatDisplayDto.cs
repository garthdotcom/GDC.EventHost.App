using GDC.EventHost.Shared.SeatPosition;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.Shared.Seat
{
    public class SeatDisplayDto 
    {
        public List<SeatPositionDisplayDto> SeatPositions { get; set; } = []; 

        public Guid Id { get; set; }

        public SeatTypeEnum SeatTypeId { get; set; }

        public string SeatTypeName { get; set; } = string.Empty;

        public string DisplayValue { get; set; } = string.Empty;

        public int OrdinalValue { get; set; }

        public Guid ParentId { get; set; }

        public Guid SeatingPlanId { get; set; }

        public Guid VenueId { get; set; }

        public string VenueName { get; set; } = string.Empty;
    }
}

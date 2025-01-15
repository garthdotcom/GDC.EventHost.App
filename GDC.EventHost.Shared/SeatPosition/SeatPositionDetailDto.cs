using GDC.EventHost.Shared.Seat;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.Shared.SeatPosition
{
    public class SeatPositionDetailDto
    {
        public Guid Id { get; set; }

        // section, row
        public SeatPositionTypeEnum SeatPositionTypeId { get; set; }

        public string SeatPositionTypeName{ get; set; } = string.Empty;

        public string DisplayValue { get; set; } = string.Empty;

        public int OrdinalValue { get; set; }

        public int Level { get; set; }

        public IEnumerable<SeatDetailDto> Seats { get; set; } = [];

        public SeatPositionDetailDto? ParentSeatPostion { get; set; }

        public IEnumerable<SeatPositionDetailDto> ChildSeatPositions { get; set; } = [];
    }
}

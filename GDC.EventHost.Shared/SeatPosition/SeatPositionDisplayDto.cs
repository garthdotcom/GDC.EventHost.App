using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.Shared.SeatPosition
{
    public class SeatPositionDisplayDto
    {
        public int Level { get; set; }

        public SeatPositionTypeEnum SeatPositionTypeId { get; set; }

        public string SeatPositionTypeName { get; set; } = string.Empty;

        public int OrdinalValue { get; set; }

        public string DisplayValue { get; set; } = string.Empty;
    }
}

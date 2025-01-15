using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.Shared.SeatPosition
{
    public class SeatPositionDto
    {
        public Guid Id { get; set; }

        // section, row
        public SeatPositionTypeEnum SeatPositionTypeId { get; set; }

        public string DisplayValue { get; set; } = string.Empty;

        public int OrdinalValue { get; set; }

        public int Level { get; set; }
    }
}
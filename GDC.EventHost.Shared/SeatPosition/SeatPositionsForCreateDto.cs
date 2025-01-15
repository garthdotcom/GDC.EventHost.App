using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.Shared.SeatPosition
{
    public class SeatPositionsForCreateDto
    {
        public int Level { get; set; }

        public int Number { get; set; }

        public SeatPositionTypeEnum SeatPositionType { get; set; } 
    }
}

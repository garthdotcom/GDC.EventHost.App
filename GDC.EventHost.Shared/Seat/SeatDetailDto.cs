using GDC.EventHost.Shared.SeatPosition;
using GDC.EventHost.Shared.Ticket;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.Shared.Seat
{
    public class SeatDetailDto
    {
        public Guid Id { get; set; } 

        public SeatTypeEnum SeatTypeId { get; set; }

        public string SeatTypeName { get; set; } = string.Empty;

        public string DisplayValue { get; set; } = string.Empty;

        public int OrdinalValue { get; set; }

        public Guid ParentId { get; set; } 

        public SeatPositionDetailDto? SeatPositionParent { get; set; }

        public IEnumerable<TicketDto> Tickets { get; set; } = [];
    }
}
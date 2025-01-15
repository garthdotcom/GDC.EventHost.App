using GDC.EventHost.Shared.Ticket;

namespace GDC.EventHost.Shared.Order
{
    public class OrderItemDto
    {
        public required Guid Id { get; set; }

        public required TicketDetailDto Ticket { get; set; }

        public required Guid OrderId { get; set; }
    }
}
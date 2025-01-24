using GDC.EventHost.Shared.Ticket;

namespace GDC.EventHost.Shared.Order
{
    public class OrderItemForUpdateDto
    {
        public Guid Id { get; set; }

        public TicketDetailDto Ticket { get; set; }

        public Guid OrderId { get; set; }
    }
}

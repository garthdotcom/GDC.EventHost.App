using GDC.EventHost.Shared.Ticket;

namespace GDC.EventHost.Shared.ShoppingCart
{
    public class ShoppingCartItemDto
    {
        public required Guid Id { get; set; }

        public required TicketDetailDto Ticket { get; set; }

        public required Guid ShoppingCartId { get; set; }
    }
}
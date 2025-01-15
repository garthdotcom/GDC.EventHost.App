using GDC.EventHost.Shared.Ticket;

namespace GDC.EventHost.Shared.ShoppingCart
{
    public class ShoppingCartItemForUpdateDto
    {
        public required Guid Id { get; set; }

        public required TicketDto Ticket { get; set; }

        public required Guid ShoppingCartId { get; set; }
    }
}

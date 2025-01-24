using GDC.EventHost.Shared.Ticket;

namespace GDC.EventHost.Shared.ShoppingCart
{
    public class ShoppingCartItemForUpdateDto
    {
        public Guid Id { get; set; }

        public TicketDto Ticket { get; set; }

        public Guid ShoppingCartId { get; set; }
    }
}

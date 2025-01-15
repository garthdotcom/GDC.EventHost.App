using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.Shared.Order
{
    public class OrderForUpdateDto
    {
        public Guid Id { get; set; }

        public Guid MemberId { get; set; }

        public OrderStatusEnum OrderStatusId { get; set; }

        public DateTime Date { get; set; }

        public List<OrderItemForUpdateDto> OrderItems { get; set; } = [];
    }
}

using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.Shared.Order
{
    public class OrderForPatchDto
    {
        public Guid Id { get; set; }

        public Guid MemberId { get; set; }

        public OrderStatusEnum OrderStatusId { get; set; }

        public DateTime Date { get; set; }
    }
}

using GDC.EventHost.Shared.Order;

namespace GDC.EventHost.App.ViewModels.Orders
{
    public class OrderDetailVM
    {
        public OrderDto Order { get; set; }

        public string ReturnUrl { get; set; }

        public decimal OrderTotal { get; set; }
    }
}

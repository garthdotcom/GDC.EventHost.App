using GDC.EventHost.Shared.Order;
using GDC.EventHost.App.Utilities;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class OrderListVM
    { 
        public string SearchQuery { get; set; }
        public PaginatedList<OrderListItemVM> OrderListItems { get; set; }
    }

    public class OrderListItemVM
    {
        public OrderDto Order { get; set; }
        public string MemberFullName { get; set; }
        public string MemberUserName { get; set; }
    }
} 
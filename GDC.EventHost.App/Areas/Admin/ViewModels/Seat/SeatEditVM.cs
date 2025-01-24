using GDC.EventHost.Shared.Seat;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class SeatEditVM
    {
        public SeatForUpdateDto Seat { get; set; }

        public List<SelectListItem> SeatTypeList { get; set; }

        public string ReturnUrl { get; set; }
    }
}
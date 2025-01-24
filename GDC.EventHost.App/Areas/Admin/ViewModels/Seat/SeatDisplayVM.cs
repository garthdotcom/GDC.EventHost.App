using GDC.EventHost.Shared.Seat;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class SeatDisplayVM
    {
        public SeatDisplayDto SeatDetail { get; set; }

        public string ReturnUrl { get; set; }
    }
}
using GDC.EventHost.Shared.SeatPosition;
using Microsoft.AspNetCore.Mvc.Rendering;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class SeatPositionEditVM
    {
        public List<SeatPositionsForCreateDto> Positions { get; set; } 

        public int Level { get; set; }

        public int Number { get; set; } 

        public SeatPositionTypeEnum SeatPositionTypeId { get; set; } 

        public List<SelectListItem> SeatPositionTypeList { get; set; }

        public Guid? PerformanceId { get; set; }
    }
}

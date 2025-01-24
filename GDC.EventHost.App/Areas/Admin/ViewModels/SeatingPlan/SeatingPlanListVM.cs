using GDC.EventHost.Shared.SeatingPlan;
using GDC.EventHost.App.Utilities;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class SeatingPlanListVM
    {
        public string SearchQuery { get; set; }
        public PaginatedList<SeatingPlanDetailDto> SeatingPlanList { get; set; }
    }
}

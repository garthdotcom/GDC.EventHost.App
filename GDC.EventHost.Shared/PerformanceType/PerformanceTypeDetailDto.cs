using GDC.EventHost.Shared.Performance;
using System.ComponentModel.DataAnnotations;

namespace GDC.EventHost.Shared.PerformanceType
{
    public class PerformanceTypeDetailDto
    {
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public required string Name { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        public List<PerformanceDetailDto> Performances { get; set; } = [];
    }
}
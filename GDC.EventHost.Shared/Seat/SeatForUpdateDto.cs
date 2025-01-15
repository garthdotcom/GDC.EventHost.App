using System.ComponentModel.DataAnnotations;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.Shared.Seat
{
    public class SeatForUpdateDto
    {
        public Guid Id { get; set; }

        [Display(Name = "Display Value")]
        [Required(ErrorMessage = "You should enter a Display Value.")]
        [MaxLength(10, ErrorMessage = "The Display Value should not be longer than 10 characters.")]
        public required string DisplayValue { get; set; }

        [Display(Name = "Ordinal Value")]
        [Required(ErrorMessage = "You should enter an Ordinal Value.")]
        public required int OrdinalValue { get; set; }

        [Display(Name = "Seat Type")]
        [Required(ErrorMessage = "You should enter a Seat Type.")]
        public required SeatTypeEnum SeatTypeId { get; set; }

        [Display(Name = "Parent")]
        [Required(ErrorMessage = "You should enter a Parent Id.")]
        [RegularExpression(@"^[A-Za-z0-9]{8}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{12}$",
            ErrorMessage = "The Parent Id must be a valid Guid.")]
        public Guid ParentId { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.Shared.SeriesAsset
{
    public class SeriesAssetForUpdateDto
    {
        public Guid Id { get; set; }

        [Display(Name = "Series")]
        [Required(ErrorMessage = "You should enter a Series Id.")]
        [RegularExpression(@"^[A-Za-z0-9]{8}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{12}$",
            ErrorMessage = "The Series Id must be a valid Guid.")]
        public Guid SeriesId { get; set; }

        [Display(Name = "Asset")]
        [Required(ErrorMessage = "You should enter an Asset Id.")]
        [RegularExpression(@"^[A-Za-z0-9]{8}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{12}$",
            ErrorMessage = "The Asset Id must be a valid Guid.")]
        public Guid AssetId { get; set; }

        [Display(Name = "Asset Type")]
        [Required(ErrorMessage = "You should enter an Asset Type.")]
        public AssetTypeEnum AssetTypeId { get; set; }

        [Display(Name = "Ordinal Value")]
        [Required(ErrorMessage = "You should enter an Ordinal Value.")]
        [Range(1, 10, ErrorMessage = "The Ordinal Value must be within the range of 1 and 10.")]
        public int OrdinalValue { get; set; }
    }
}

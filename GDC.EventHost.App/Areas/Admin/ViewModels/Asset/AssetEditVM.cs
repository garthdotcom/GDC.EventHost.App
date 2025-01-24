using GDC.EventHost.Shared.Asset;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class AssetEditVM
    {
        public AssetForUpdateDto Asset { get; set; } 

        public List<SelectListItem> AssetTypeList { get; set; }

        [Display(Name = "File to Upload")]
        public IFormFile FileUpload { get; set; }
    }
}

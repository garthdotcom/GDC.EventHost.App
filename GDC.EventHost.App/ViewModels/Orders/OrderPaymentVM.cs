using System.ComponentModel.DataAnnotations;

namespace GDC.EventHost.App.ViewModels.Orders
{
    public class OrderPaymentVM
    {
        [Required]
        [RegularExpression(@"^[A-Za-z0-9]{8}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{12}$",
            ErrorMessage = "The Order Id must be a valid Guid.")]
        public Guid OrderId { get; set; } 

        [Required]
        [Display(Name = "Card Number")]
        [RegularExpression(@"^((4\d{3})|(5[1-5]\d{2})|(6011))-?\d{4}-?\d{4}-?\d{4}|3[4,7]\d{13}$",
            ErrorMessage = "You should enter a valid card number.")]
        public string CreditCardNumber { get; set; }

        [Required]
        [Display(Name = "Name On Card")]
        [MaxLength(50, ErrorMessage = "You should enter a name that is less than 50 characters.")]
        public string NameOnCard { get; set; }

        [Required]
        [Display(Name = "Expiry Date")]
        [RegularExpression(@"^(([0-9])|(0[0-9])|(1[0-2]))/202[0-9]$",
            ErrorMessage = "You should enter a valid expiry date in format MM/YYYY.")]
        public string CardExpiryMonthYear { get; set; }

        [Required]
        [Display(Name = "CV2")]
        [RegularExpression(@"^\d{3}$",
            ErrorMessage = "You should enter a valid expiry date in format MM/YYYY.")]
        public string CardCV2 { get; set; }
    }
}

using GDC.EventHost.App.Models;

namespace GDC.EventHost.App.ViewModels.ShoppingCarts
{
    public class ShoppingCartVM
    {
        public ShoppingCart ShoppingCart { get; set; }

        public decimal ShoppingCartTotal { get; set; }

        public string ReturnUrl { get; set; }
    }
}
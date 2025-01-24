using GDC.EventHost.App;
using GDC.EventHost.App.ApiServices;
using GDC.EventHost.Shared.ShoppingCart;
using GDC.EventHost.Shared.Ticket;
using GDC.EventHost.App.Models;
using GDC.EventHost.App.ViewModels.ShoppingCarts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GDC.EventHost.App.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    public class ShoppingCartController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<ShoppingCartController> _logger;
        private readonly IEventHostService _eventHostService;
        private readonly ShoppingCart _shoppingCart;

        public ShoppingCartController(IConfiguration configuration,
            ILogger<ShoppingCartController> logger,
            IEventHostService eventHostService,
            ShoppingCart shoppingCart)
        {
            _config = configuration;
            _logger = logger;
            _eventHostService = eventHostService;
            _shoppingCart = shoppingCart;
        }

        public ActionResult Index()
        {
            var items = _shoppingCart.GetShoppingCartItems(); 

            _shoppingCart.ShoppingCartItems = items; 

            var shoppingCartViewModel = new ShoppingCartVM
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal(),
                ReturnUrl = Request.Headers["Referer"].ToString()
            };

            return View(shoppingCartViewModel);
        }

        public ActionResult AddToShoppingCart(Guid ticketId)
        {
            var selectedTicketDto = _eventHostService
                .GetOne<TicketDto>($"/tickets/{ticketId}").Result;

            if (selectedTicketDto != null)
            {
                _shoppingCart.AddToCart(new ShoppingCartItemForUpdateDto()
                {
                    Ticket = selectedTicketDto,
                    ShoppingCartId = Guid.Parse(_shoppingCart.ShoppingCartId)
                });
            }

            return RedirectToAction("Index");
        }

        public ActionResult RemoveItem(Guid shoppingCartItemId)
        {
            var cartItems = _shoppingCart.GetShoppingCartItems();

            if (cartItems.Count > 0)
            {
                var selectedItem = cartItems.FirstOrDefault(i => i.Id == shoppingCartItemId);

                if (selectedItem != null)
                {
                    _shoppingCart.RemoveFromCart(selectedItem);
                }
            }
             
            return RedirectToAction("Index");
        }
    }
}
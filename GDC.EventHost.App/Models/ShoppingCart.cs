using GDC.EventHost.App.ApiServices;
using GDC.EventHost.App.Auth;
using GDC.EventHost.App.Services;
using GDC.EventHost.Shared.Member;
using GDC.EventHost.Shared.ShoppingCart;
using GDC.EventHost.Shared.Ticket;
using GDC.EventHost.App.ApiServices;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.App.Models
{
    public class ShoppingCart
    {
        private readonly IEventHostService _eventHostService;

        private ShoppingCart(IEventHostService eventHostService)
        {
            _eventHostService = eventHostService;
        }

        public static ShoppingCart GetCart(IServiceProvider services)
        {
            // this line is needed because we dont have access to session in a regular class 

            ISession session = services.GetRequiredService<IHttpContextAccessor>()?
                .HttpContext.Session;

            var context = services.GetService<EventHostService>();

            var cartIdInSession = session.GetString("CartId");
            
            if (cartIdInSession == null)
            {
                var claimsPrincipal = services.GetRequiredService<IHttpContextAccessor>().HttpContext.User;

                var userManager = services.GetRequiredService<UserManager<EventHostUser>>();

                var user = userManager.GetUserAsync(claimsPrincipal).Result;

                if (user != null)
                {
                    var memberInDb = context.GetOne<MemberDto>($"/users/{user.Id}").Result;

                    if (memberInDb == null || memberInDb.Id == Guid.Empty)
                    {
                        return null;
                        // TODO: log the error
                    }

                    // no cart id in session. is there one in the db for this user?

                    var shoppingCartsInDb = context.GetMany<ShoppingCartDto>($"/shoppingcarts").Result;

                    var cartForUserInDb = shoppingCartsInDb.FirstOrDefault(c => c.MemberId == memberInDb.Id);

                    if (cartForUserInDb != null)
                    {
                        // yes, update the cart id in session to match the database cart key

                        cartIdInSession = cartForUserInDb.Id.ToString();

                        session.SetString("CartId", cartIdInSession);
                    }
                    else
                    {
                        // no, there is no cart in session or in the database

                        string stringData = JsonConvert.SerializeObject(new ShoppingCartForUpdateDto()
                        {
                            MemberId = memberInDb.Id,
                            ShoppingCartItems = new List<ShoppingCartItemForUpdateDto>()
                        });

                        // create a new cart in the db for the user

                        var newCartForUserInDb = context.PostOne<ShoppingCartDto>("/shoppingcarts", stringData).Result;

                        if (!context.Error)
                        {
                            cartIdInSession = newCartForUserInDb.Id.ToString();

                            // add the cart id to the session if db create was successful

                            session.SetString("CartId", cartIdInSession);
                        }

                        // todo: handle error
                    }
                }
            }

            //TODO: add some checking here to verify the session cart belongs to the current user?

            // return the shopping cart with the id from the session in the Id property

            return new ShoppingCart(context) { ShoppingCartId = cartIdInSession };
        }

        public void AddToCart(ShoppingCartItemForUpdateDto shoppingCartItemForUpdateDto)
        {
            var shoppingCart = _eventHostService
                .GetOne<ShoppingCartForUpdateDto>($"/shoppingcarts/{ShoppingCartId}").Result;

            if (shoppingCart != null)
            {
                // cart exists

                if (!shoppingCart.ShoppingCartItems.Any(s => s.Ticket.Id == shoppingCartItemForUpdateDto.Ticket.Id))
                {
                    // ticket is not already in the cart

                    if (!TicketIsAvailable(shoppingCartItemForUpdateDto.Ticket.Id))
                    {
                        // ticket has become unavailable

                        return;
                    }

                    // add the ticket to the cart

                    string stringData = JsonConvert.SerializeObject(shoppingCartItemForUpdateDto);

                    _ = _eventHostService.PostOne<ShoppingCartItemDto>("/shoppingcartitems", stringData).Result;
                    //await _eventHostService.PostOne<ShoppingCartItemDto>("/shoppingcartitems", stringData);

                    if (!_eventHostService.Error)
                    {
                        // ticket added to the cart. update the status so others do not attempt to purchase it

                        UpdateTicketStatus(shoppingCartItemForUpdateDto.Ticket.Id, TicketStatusEnum.SalePending);
                    }
                    else
                    {
                        var theErrors = _eventHostService.Messages;
                    }
                }
            }
        }

        public List<ShoppingCartItemDto> GetShoppingCartItems()
        {
            // todo: use the property

            var items = new List<ShoppingCartItemDto>();

            if (string.IsNullOrEmpty(ShoppingCartId))
            {
                return items;
            }

            var shoppingCart = _eventHostService
                .GetOne<ShoppingCartDto>($"/shoppingcarts/{ShoppingCartId}").Result;

            if (shoppingCart != null)
            {
                items = shoppingCart.ShoppingCartItems;
            }

            return items;
        }

        public void RemoveFromCart(ShoppingCartItemDto shoppingCartItemDto)
        {
            var itemsInCart = GetShoppingCartItems();

            var cartItem = itemsInCart
                .FirstOrDefault(s => s.Ticket.Id == shoppingCartItemDto.Ticket.Id);

            if (cartItem != null)
            {
                // ticket is in the cart; remove it

                RemoveCartItem(cartItem);

                UpdateTicketStatus(cartItem.Ticket.Id, TicketStatusEnum.UnSold);
            }
        }

        public void ClearCart() 
        {
            var itemsInCart = GetShoppingCartItems();

            foreach (var item in itemsInCart)
            {
                // delete the shopping cart item and update the ticket to unsold

                RemoveCartItem(item);

                UpdateTicketStatus(item.Ticket.Id, TicketStatusEnum.UnSold);
            }
        }

        private async void RemoveCartItem(ShoppingCartItemDto cartItem)
        {
            await _eventHostService.DeleteOne($"/shoppingcartitems/{cartItem.Id}");
        }

        public decimal GetShoppingCartTotal()
        {
            var total = 0M;

            if (string.IsNullOrEmpty(ShoppingCartId))
            {
                return total;
            }

            return GetShoppingCartItems().Select(s => s.Ticket.Price).Sum();
        }

        private bool TicketIsAvailable(Guid ticketId)
        {
            var ticket = _eventHostService.GetOne<TicketDto>($"/tickets/{ticketId}").Result;

            if (ticket == null)
            {
                return false;
            }

            return ticket.TicketStatusId == TicketStatusEnum.UnSold;
        }

        private void UpdateTicketStatus(Guid ticketId, TicketStatusEnum ticketStatus)
        {
            var patchDocList = new List<PatchDocument>()
            {
                new PatchDocument()
                {
                    op = "replace",
                    path = "/ticketStatusId",
                    value = ticketStatus.ToString()
                }
            };

            string stringData = JsonConvert.SerializeObject(patchDocList);

            Task.Run(async () => 
                {
                    await _eventHostService.PatchOne($"/tickets/{ticketId}", stringData); 
                }
            );
        }

        public void CompleteSale()
        {
            var itemsInCart = GetShoppingCartItems();

            foreach (var shoppingCartItem in itemsInCart)
            {
                UpdateTicketStatus(shoppingCartItem.Ticket.Id, TicketStatusEnum.Sold);

                RemoveCartItem(shoppingCartItem);
            }
        }

        public string ShoppingCartId { get; set; }

        public List<ShoppingCartItemDto> ShoppingCartItems { get; set; }
    }
}


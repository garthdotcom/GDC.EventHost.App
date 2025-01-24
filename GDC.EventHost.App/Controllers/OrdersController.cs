using GDC.EventHost.App;
using GDC.EventHost.App.ApiServices;
using GDC.EventHost.App.Services;
using GDC.EventHost.Shared.Member;
using GDC.EventHost.Shared.Order;
using GDC.EventHost.Shared.ShoppingCart;
using GDC.EventHost.App.Models;
using GDC.EventHost.App.Services;
using GDC.EventHost.App.ViewModels.Orders;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.App.Controllers
{
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    public class OrdersController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<OrdersController> _logger;
        private readonly IEventHostService _eventHostService;
        private readonly IWebHostEnvironment _environment;
        private readonly ShoppingCart _shoppingCart;
        private readonly IEmailSender _emailSender;

        public OrdersController(IConfiguration configuration,
            ILogger<OrdersController> logger,
            IEventHostService eventHostService,
            IEmailSender emailSender,
            IWebHostEnvironment environment,
            ShoppingCart shoppingCart)
        {
            _config = configuration;
            _logger = logger;
            _eventHostService = eventHostService;
            _shoppingCart = shoppingCart;
            _emailSender = emailSender;
            _environment = environment;
        }

        public async Task<ActionResult> Checkout()
        {
            // fill an order form with the shopping cart values

            var cartId = _shoppingCart.ShoppingCartId;

            var shoppingCart = await _eventHostService
                .GetOne<ShoppingCartDto>($"/shoppingcarts/{cartId}");

            if (shoppingCart != null)
            {
                var orderForUpdate = new OrderForUpdateDto
                {
                    MemberId = shoppingCart.MemberId,
                    Date = DateTime.Now,
                    OrderStatusId = OrderStatusEnum.Incomplete
                };

                foreach (var cartItem in shoppingCart.ShoppingCartItems)
                {
                    orderForUpdate.OrderItems.Add(new OrderItemForUpdateDto
                    {
                        Ticket = cartItem.Ticket
                    });     
                }

                var orderEditViewModel = new OrderEditVM
                {
                    Order = orderForUpdate,
                    OrderTotal = orderForUpdate.OrderItems.Select(o => o.Ticket.Price).Sum(),
                    ReturnUrl = Request.Headers["Referer"].ToString()
                };

                return View(orderEditViewModel);
            }

            return RedirectToAction("Index", "ShoppingCart");
        }

        [HttpPost]
        public async Task<ActionResult> Checkout(OrderEditVM orderEditViewModel)
        {
            //*********************************************************************************************
            // if an order has already been created for a ticket in the cart, we don't want to create
            // another order - skip ahead to the payment page if the order is still in progress or direct
            // the user to the shopping cart if not

            var cartId = _shoppingCart.ShoppingCartId;

            var shoppingCart = await _eventHostService
                .GetOne<ShoppingCartDto>($"/shoppingcarts/{cartId}");

            var orders = await _eventHostService
                .GetMany<OrderDto>($"/members/{shoppingCart.MemberId}/orders");

            foreach (var cartItem in shoppingCart.ShoppingCartItems)
            {
                foreach (var order in orders)
                {
                    var ticketsInOrder = order.OrderItems.Select(s => s.Ticket);

                    if (ticketsInOrder.Any(t => t.Id == cartItem.Ticket.Id))
                    {
                        // an order has already been created that contains a ticket in the cart

                        // if the item is in progress, redirect to payment page without creating another order
                        if (order.OrderStatusId == OrderStatusEnum.Incomplete)
                        {
                            return RedirectToAction("Payment", new { orderId = order.Id });
                        }

                        // if the order status is not in progress, redirect to shopping cart
                        return RedirectToAction("Index", "ShoppingCart");
                    }
                }
            }

            //*********************************************************************************************

            // create the order

            string stringData = JsonConvert.SerializeObject(orderEditViewModel.Order);

            var newOrder = await _eventHostService.PostOne<OrderDto>("/orders", stringData);

            if (_eventHostService.Error)
            {
                ModelState.AddModelError("", _eventHostService.Messages);
            }

            if (ModelState.IsValid)
            {
                return RedirectToAction("Payment", new { orderId = newOrder.Id });
            }

            ViewBag.Message = _eventHostService.Messages;

            return View(orderEditViewModel);
        }

        public ActionResult Payment(Guid orderId) 
        {
            // step 2 - show the enter payment page with cancel and complete order buttons

            var paymentViewModel = new OrderPaymentVM
            {
                OrderId = orderId
            };

            return View(paymentViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Payment(OrderPaymentVM paymentViewModel)
        {
            //*********************************************************************************************
            // check whether the order is complete. if so we don't want to pay again - redirect to the
            // orders list page for the member

            var order = await _eventHostService
                .GetOne<OrderDto>($"/orders/{paymentViewModel.OrderId}");

            if (order.OrderStatusId == OrderStatusEnum.Complete ||
                order.OrderStatusId == OrderStatusEnum.Rejected ||
                order.OrderStatusId == OrderStatusEnum.Cancelled)
            {
                return RedirectToAction("OrderList", "Members", 
                    new { id = order.MemberId });
            }

            //*********************************************************************************************

            if (ModelState.IsValid)
            {
                // todo: simulate payment process here; assume valid for now

                // update status of the tickets to sold and delete the items in the cart

                _shoppingCart.CompleteSale();

                // update status of the order to complete

                UpdateOrderStatus(paymentViewModel.OrderId, OrderStatusEnum.Complete);

                // send email confirmation with tickets

                var member = await _eventHostService
                    .GetOne<MemberDetailDto>($"/members/{order.MemberId}");

                if (member != null)
                {
                    var emailAddress = member.EmailAddress;

                    var subject = "EventHost Order Confirmation";

                    var builder = new StringBuilder();

                    var filePath = Path.Combine(_environment.WebRootPath, "content", "emailOrderConfirmation.html");

                    var templateFile = new FileInfo(filePath);

                    using (var reader = templateFile.OpenText())
                    {
                        builder.Append(reader.ReadToEnd());
                    }

                    builder.Replace("{{orderNumber}}", order.Id.ToString().Replace("-","").ToUpper());

                    builder.Replace("{{memberName}}", member.FullName);

                    var messageTextBuilder = new StringBuilder();

                    messageTextBuilder.Append("<div><table>");
                    messageTextBuilder.Append("<tr>");
                    messageTextBuilder.Append("<th style=\"width:400px;text-align:left;border-bottom:1px solid gray\">Ticket</th>");
                    messageTextBuilder.Append("<th style=\"width:50px;text-align:right;border-bottom:1px solid gray\">Price</th>");
                    messageTextBuilder.Append("</tr>");

                    foreach (var orderItem in order.OrderItems)
                    {
                        var orderListBuilder = new StringBuilder();

                        orderListBuilder.Append($"{orderItem.Ticket.PerformanceTitle}<br />");
                        orderListBuilder.Append($"{orderItem.Ticket.PerformanceDate.ToString("dd MMMM yyyy HH:mm")}<br />");
                        orderListBuilder.Append($"{orderItem.Ticket.VenueName}<br />");

                        foreach(var seatPosition in orderItem.Ticket.Seat.SeatPositions)
                        {
                            orderListBuilder.Append($"{seatPosition.SeatPositionTypeName} {seatPosition.DisplayValue}, ");
                        }

                        orderListBuilder.Append($" Seat {orderItem.Ticket.Seat.DisplayValue}");

                        messageTextBuilder.Append("<tr>");
                        messageTextBuilder.Append($"<td style=\"text-align:left;border-bottom:1px solid gray\">{orderListBuilder.ToString()}</td>");
                        messageTextBuilder.Append($"<td style=\"text-align:right;border-bottom:1px solid gray\">{orderItem.Ticket.Price.ToString("c")}</td>");
                        messageTextBuilder.Append("</tr>");
                    }

                    var orderTotal = order.OrderItems.Select(o => o.Ticket.Price).Sum();

                    messageTextBuilder.Append("<tr>");
                    messageTextBuilder.Append($"<td style=\"text-align:right\">Total:</td>");
                    messageTextBuilder.Append($"<td style=\"text-align:right\">{orderTotal.ToString("c")}</td>");
                    messageTextBuilder.Append("</tr>");
                    messageTextBuilder.Append("</table></div>");

                    builder.Replace("{{messageText}}", messageTextBuilder.ToString());

                    var message = builder.ToString();

                    await _emailSender.SendEmailAsync(emailAddress, subject, message);
                }

                // direct the user to a detail view with a thank you message

                return RedirectToAction("Detail", new { orderId = paymentViewModel.OrderId });
            }

            return View(paymentViewModel);
        }

        public async Task<ActionResult> Detail(Guid orderId)
        {
            var referringPage = Request.Headers["Referer"].ToString();

            if (!referringPage.Contains("OrderList"))
            {
                ViewBag.Message = "Thank you for your order. We hope you enjoy the show!"; 
            }

            var orderDto = await _eventHostService.GetOne<OrderDto>($"/orders/{orderId}");

            var orderDetailViewModel = new OrderDetailVM
            {
                Order = orderDto,
                OrderTotal = orderDto.OrderItems.Select(o => o.Ticket.Price).Sum(),
                ReturnUrl = referringPage.Contains("OrderList") ? referringPage : string.Empty
            };

            return View(orderDetailViewModel);
        }

        public async Task<ActionResult> CancelOrder(Guid orderId)
        {
            // delete the order

            await _eventHostService.DeleteOne($"/orders/{orderId}");

            // direct the user back to the shopping cart

            return RedirectToAction("Index", "ShoppingCart");
        }

        private void UpdateOrderStatus(Guid orderId, OrderStatusEnum orderStatus)
        {
            var patchDocList = new List<PatchDocument>()
            {
                new PatchDocument()
                {
                    op = "replace",
                    path = "/orderStatusId",
                    value = orderStatus.ToString()
                }
            };

            string stringData = JsonConvert.SerializeObject(patchDocList);

            Task.Run(async () =>
            {
                await _eventHostService.PatchOne($"/orders/{orderId}", stringData);
            }
            );
        }
    }
}
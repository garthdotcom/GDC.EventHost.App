using GDC.EventHost.App;
using GDC.EventHost.App.ApiServices;
using GDC.EventHost.App.Services;
using GDC.EventHost.Shared.Member;
using GDC.EventHost.Shared.Order;
using GDC.EventHost.App.Areas.Admin.ViewModels;
using GDC.EventHost.App.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.App.Areas.Admin.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    [Area("Admin")]
    public class OrderAdminController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<OrderAdminController> _logger;
        private readonly IEventHostService _eventHostService;

        public OrderAdminController(IConfiguration configuration,
            ILogger<OrderAdminController> logger,
            IEventHostService eventHostService)
        {
            _config = configuration;
            _logger = logger;
            _eventHostService = eventHostService;
        }

        public async Task<ActionResult> Index(string searchQuery, string sortOrder, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchQuery;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["UserNameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "user_name_desc" : "";
            ViewData["OrderDateSortParm"] = sortOrder == "OrderDate" ? "order_date_desc" : "OrderDate";
            ViewData["OrderStatusSortParm"] = sortOrder == "OrderStatus" ? "order_status_desc" : "OrderStatus";

            var uriBuilder = new StringBuilder();

            uriBuilder.Append("/orders");

            if (!string.IsNullOrEmpty(searchQuery))
            {
                uriBuilder.Append("?searchQuery=");
                uriBuilder.Append(WebUtility.UrlEncode(searchQuery));
            }

            var orderList = new List<OrderListItemVM>();

            var orders = await _eventHostService.GetMany<OrderDto>(uriBuilder.ToString());

            foreach (var order in orders)
            {
                var member = await _eventHostService.GetOne<MemberDto>($"/members/{order.MemberId}");

                orderList.Add(
                    new OrderListItemVM
                    {
                        Order = order,
                        MemberFullName = member.FullName,
                        MemberUserName = member.Username
                    });
            }

            switch (sortOrder)
            {
                case "user_name_desc":
                    orderList = orderList.OrderByDescending(s => s.MemberFullName).ToList();
                    break;
                case "OrderDate":
                    orderList = orderList.OrderBy(s => s.Order.Date).ToList();
                    break;
                case "order_date_desc":
                    orderList = orderList.OrderByDescending(s => s.Order.Date).ToList();
                    break;
                case "OrderStatus":
                    orderList = orderList.OrderBy(s => s.Order.OrderStatusValue).ToList();
                    break;
                case "order_status_desc":
                    orderList = orderList.OrderByDescending(s => s.Order.OrderStatusValue).ToList();
                    break;
                default:
                    orderList = orderList.OrderBy(s => s.MemberFullName).ToList();
                    break;
            }

            int pageSize = 10;

            var orderListViewModel = new OrderListVM
            {
                OrderListItems = new PaginatedList<OrderListItemVM>(orderList.AsQueryable(),
                    pageNumber ?? 1, pageSize)
            };

            return View(orderListViewModel);
        }

        public async Task<ActionResult> Detail(Guid id)
        {
            var orderDetail = await _eventHostService.GetOne<OrderDto>($"/orders/{id}");

            // verify object returned contains values
            if (!TryValidateModel(orderDetail, nameof(orderDetail)))
            {
                return NotFound();  // todo - create a friendly not found page
            }

            var orderDetailViewModel = new OrderDetailVM
            {
                Order = orderDetail,
                ReturnUrl = Request.Headers["Referer"].ToString(),
                OrderTotal = orderDetail.OrderItems.Select(o => o.Ticket.Price).Sum()
            };

            return View(orderDetailViewModel);
        }


        public async Task<ActionResult> Edit(Guid id)
        {
            var orderForUpdate = await _eventHostService.GetOne<OrderForUpdateDto>($"/orders/{id}");

            var orderEditViewModel = new OrderEditVM
            {
                Order = orderForUpdate,
                ReturnUrl = Request.Headers["Referer"].ToString(),
                OrderTotal = orderForUpdate.OrderItems.Select(o => o.Ticket.Price).Sum()
            };

            return View(orderEditViewModel);
        }


        [HttpPost]
        public async Task<ActionResult> Edit(OrderForUpdateDto order)
        {
            string stringData = JsonConvert.SerializeObject(order);

            if (order.Id == Guid.Empty)
            {
                var newOrder = await _eventHostService.PostOne<OrderDto>("/orders", stringData);

                if (_eventHostService.Error)
                {
                    ModelState.AddModelError("", _eventHostService.Messages);
                }

                if (ModelState.IsValid)
                {
                    return RedirectToAction("Detail", new { id = newOrder.Id });
                }
            }
            else
            {
                await _eventHostService.PutOne($"/orders/{order.Id.ToString()}", stringData);

                if (_eventHostService.Error)
                {
                    ModelState.AddModelError("", _eventHostService.Messages);
                }

                if (ModelState.IsValid)
                {
                    return RedirectToAction("Detail", new { id = order.Id });
                }
            }

            var orderEditViewModel = new OrderEditVM
            {
                Order = order
            };

            return View(orderEditViewModel);
        }

        public async Task<ActionResult> Delete(Guid id)
        {
            var orderToDelete = await _eventHostService.GetOne<OrderDto>($"/orders/{id}");

            if (orderToDelete != null)
            {
                foreach (var orderItem in orderToDelete.OrderItems)
                {
                    UpdateTicketStatus(orderItem.Ticket.Id, TicketStatusEnum.UnSold);
                }

                await _eventHostService.DeleteOne($"/orders/{id}");
            }

            return RedirectToAction("Index", new { searchQuery = "" });
            // todo preserve original search if any
        }

        public async Task<ActionResult> DeleteOrderItem(Guid orderId, Guid orderItemId)
        {
            var order = await _eventHostService.GetOne<OrderDto>($"/orders/{orderId}");

            if (order == null)
            {
                return NotFound();
            }

            // delete the order item

            var itemToDelete = order.OrderItems.FirstOrDefault(o => o.Id == orderItemId);

            if (itemToDelete == null)
            {
                return NotFound();
            }

            await _eventHostService.DeleteOne($"/orderitems/{orderItemId}");

            // update the status of the ticket to unsold

            UpdateTicketStatus(itemToDelete.Ticket.Id, TicketStatusEnum.UnSold);

            // if the item we are deleting is the only one in the order, hard-delete the order

            if (order.OrderItems.Count == 0)
            {
                await _eventHostService.DeleteOne($"/orders/{orderId}");

                return RedirectToAction("Index", new { searchQuery = "" });  
                // todo preserve original search if any

            }
            
            return RedirectToAction("Detail", new { id = orderId });
        }

        // todo refactor this into a new class
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
            });
        }

    }
}
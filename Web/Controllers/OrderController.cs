using Domain;
using Domain.DomainModels;
using Domain.Identity;
using GemBox.Document;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Controllers
{
        public class OrderController : Controller
        {

            private readonly IOrderService _orderService;
            private readonly UserManager<TicketingUser> _userManager;

            public OrderController(IOrderService orderService, UserManager<TicketingUser> userManager)
            {
                _orderService = orderService;
                _userManager = userManager;
                ComponentInfo.SetLicense("FREE-LIMITED-KEY");
            }

            public IActionResult GetOrders()
            {
                return View(this._orderService.getAllOrders());
            }

            public IActionResult GetUserOrders()
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                return View(this._orderService.getAllUserOrders(userId));
            }


            public IActionResult GetDetailsForOrder(BaseEntity model)
            {
                var result = this._orderService.getOrderDetails(model);
                return View(result);
            }

            public IActionResult GeneratePDF(BaseEntity model)
            {
                Order order = this._orderService.getOrderDetails(model);

                var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Invoice.docx");

                var document = DocumentModel.Load(templatePath);

                document.Content.Replace("{{OrderNumber}}", order.Id.ToString());
                document.Content.Replace("{{CustomerEmail}}", order.User.Email);
                document.Content.Replace("{{CustomerInfo}}", (order.User.FirstName + " " + order.User.LastName));

                StringBuilder sb = new StringBuilder();

                double total = 0.0;

                foreach (var item in order.TicketsInOrders)
                {
                    sb.AppendLine(item.Ticket.Title + " with quantity of " + item.Quantity + " and price of: $" + item.Ticket.TicketPrice);
                    total += (item.Ticket.TicketPrice * item.Quantity);
                }

                document.Content.Replace("{{AllTickets}}", sb.ToString());
                document.Content.Replace("{{TotalPrice }}", "$" + total.ToString());

                var stream = new MemoryStream();

                document.Save(stream, new PdfSaveOptions());

                return File(stream.ToArray(), new PdfSaveOptions().ContentType, order.Id + ".pdf");
            }

        }
    }

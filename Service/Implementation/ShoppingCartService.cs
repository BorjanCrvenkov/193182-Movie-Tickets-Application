using Domain.DomainModels;
using Domain.DTO;
using Domain.Relations;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Implementation
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCart> _shoppingCartRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<TicketsInOrder> _ticketsInOrderRepository;
        private readonly IRepository<EmailMessage> _mailRepository;

        public ShoppingCartService(IRepository<ShoppingCart> shoppingCartRepository, IUserRepository userRepository,
            IRepository<Order> orderRepository,
            IRepository<TicketsInOrder> ticketsInOrderRepository,
            IRepository<EmailMessage> mailRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _ticketsInOrderRepository = ticketsInOrderRepository;
            _mailRepository = mailRepository;
        }


        public bool deleteTicketFromSoppingCart(string userId, Guid ticketId)
        {
            if (!string.IsNullOrEmpty(userId) && ticketId != null)
            {
                var loggedInUser = this._userRepository.Get(userId);

                var userShoppingCart = loggedInUser.UserCart;

                var itemToDelete = userShoppingCart.TicketsInShoppingCart.Where(z => z.TicketId.Equals(ticketId)).FirstOrDefault();

                userShoppingCart.TicketsInShoppingCart.Remove(itemToDelete);

                this._shoppingCartRepository.Update(userShoppingCart);

                return true;
            }
            return false;
        }

        public ShoppingCartDto getShoppingCartInfo(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var loggedInUser = this._userRepository.Get(userId);

                var userCart = loggedInUser.UserCart;

                var allTickets = userCart.TicketsInShoppingCart.ToList();

                var allTicketPrices = allTickets.Select(z => new
                {
                    TicketPrice = z.CurrentTicket.Price,
                    Quantity = z.Quantity
                }).ToList();

                double totalPrice = 0.0;

                foreach (var item in allTicketPrices)
                {
                    totalPrice += item.Quantity * item.TicketPrice;
                }

                var reuslt = new ShoppingCartDto
                {
                    Tickets = allTickets,
                    TotalPrice = totalPrice
                };

                return reuslt;
            }
            return new ShoppingCartDto();
        }

        public bool order(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var loggedInUser = this._userRepository.Get(userId);
                var userCart = loggedInUser.UserCart;

                EmailMessage mail = new EmailMessage();
                mail.MailTo = loggedInUser.Email;
                mail.Subject = "Successfully created order!";
                mail.Status = false;

                Order order = new Order
                {
                    Id = Guid.NewGuid(),
                    User = loggedInUser,
                    UserId = userId
                };

                this._orderRepository.Insert(order);

                List<TicketsInOrder> ticketsInOrder = new List<TicketsInOrder>();

                var result = userCart.TicketsInShoppingCart.Select(z => new TicketsInOrder
                {
                    Id = Guid.NewGuid(),
                    TicketId = z.CurrentTicket.Id,
                    Ticket = z.CurrentTicket,
                    OrderId = order.Id,
                    Order = order,
                    Quantity = z.Quantity
                }).ToList();

                StringBuilder sb = new StringBuilder();

                var totalPrice = 0.0;

                sb.AppendLine("Your order is completed. The order contains: ");

                for(int i = 1; i <= result.Count; i++)
                {
                    var currentItem = result[i-1];
                    totalPrice += (currentItem.Quantity * currentItem.Ticket.Price);
                    sb.AppendLine(currentItem.Ticket.Title + " with quantity of " + currentItem.Quantity + " and price of: $" + currentItem.Ticket.Price);
                }

                sb.AppendLine("Total price for your order: " + totalPrice.ToString());

                mail.Content = sb.ToString();

                ticketsInOrder.AddRange(result);

                foreach (var item in ticketsInOrder)
                {
                    this._ticketsInOrderRepository.Insert(item);
                }

                loggedInUser.UserCart.TicketsInShoppingCart.Clear();

                this._userRepository.Update(loggedInUser);
                this._mailRepository.Insert(mail);

                return true;
            }

            return false;
        }
    }
}

using Domain.Identity;
using Domain.Relations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DomainModels
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; }
        public TicketingUser User { get; set; }

        public virtual ICollection<TicketsInOrder> TicketsInOrders { get; set; }
    }
}
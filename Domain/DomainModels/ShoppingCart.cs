using Domain.Identity;
using Domain.Relations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DomainModels
{
    public class ShoppingCart : BaseEntity
    {
        public string OwnerId { get; set; }
        public virtual TicketingUser Owner { get; set; }

        public virtual ICollection<TicketsInShoppingCart> TicketsInShoppingCart { get; set; }
    }
}
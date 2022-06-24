using Domain.Relations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTO
{
    public class ShoppingCartDto
    {
        public List<TicketsInShoppingCart> Tickets { get; set; }

        public double TotalPrice { get; set; }
    }
}
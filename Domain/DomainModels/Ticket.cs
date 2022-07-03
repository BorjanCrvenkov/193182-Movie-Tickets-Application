using Domain.Relations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.DomainModels
{
    public class Ticket : BaseEntity
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public double Rating { get; set; }
        [Required]
        public string Image { get; set; }

        public virtual ICollection<TicketsInShoppingCart> TicketsInShoppingCart { get; set; }
        public virtual ICollection<TicketsInOrder> TicketsInOrders { get; set; }

        public virtual ICollection<TicketsTypeGenres> TicketsTypeGenres{ get; set; }
    }
}

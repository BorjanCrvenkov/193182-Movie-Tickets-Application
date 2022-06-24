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
        public DateTime ReleasedDate { get; set; }
        [Required]
        public string TicketDescription { get; set; }
        [Required]
        public double TicketPrice { get; set; }
        [Required]
        public double TicketRating { get; set; }
        [Required]
        public string TicketImage { get; set; }
        //public List<Genre> TicketGenres { get; set; }

        public virtual ICollection<TicketsInShoppingCart> TicketsInShoppingCart { get; set; }
        public virtual ICollection<TicketsInOrder> TicketsInOrders { get; set; }

        public virtual ICollection<TicketsTypeGenres> TicketsTypeGenres{ get; set; }
    }
}

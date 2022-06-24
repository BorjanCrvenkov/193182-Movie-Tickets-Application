using Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Relations
{
    public class TicketsTypeGenres : BaseEntity
    {
        public Guid TicketId { get; set; }

        public Ticket Ticket { get; set; }

        public Guid GenreId { get; set; }

        public Genre Genre{ get; set; }
    }
}

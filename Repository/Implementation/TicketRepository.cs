using Domain;
using Domain.DomainModels;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository.Implementation
{
    public class TicketRepository : ITicketRepository
    {

        private readonly ApplicationDbContext context;
        private DbSet<Ticket> entities;

        public TicketRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<Ticket>();
        }

        public List<Ticket> getAllTickets()
        {
            return entities
                .Include(z => z.TicketsTypeGenres)
                .Include("TicketsTypeGenres.Genre")
                .ToListAsync().Result;
        }

        public Ticket getTicketDetails(Guid? id)
        {
            return entities
                .Include(z => z.TicketsTypeGenres)
                .SingleOrDefaultAsync(z => z.Id == id).Result;
        }

        public void Insert(Ticket ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException("ticket");
            }
            entities.Add(ticket);
            context.SaveChanges();
        }

        public void Update(Ticket ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException("ticket");
            }
            var genresToRemove = context.TicketsGenres.Where(z => z.TicketId == ticket.Id);
            context.RemoveRange(genresToRemove);
            entities.Update(ticket);
            context.SaveChanges();
        }

        public void Delete(Ticket ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException("ticket");
            }
            entities.Remove(ticket);
            context.SaveChanges();
        }
    }
}

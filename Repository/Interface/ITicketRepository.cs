using Domain;
using Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Interface
{
    public interface ITicketRepository
    {

        public List<Ticket> getAllTickets();
        public Ticket getTicketDetails(Guid? Id);

        public void Insert(Ticket ticket);

        public void Update(Ticket ticket);

        public void Delete(Ticket ticket);

    }
}

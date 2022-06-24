using Domain.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Interface
{
    public interface IUserRepository
    {
        IEnumerable<TicketingUser> GetAll();
        TicketingUser Get(string id);
        void Insert(TicketingUser entity);
        void Delete(TicketingUser entity);
        void Update(TicketingUser entity);
    }
}

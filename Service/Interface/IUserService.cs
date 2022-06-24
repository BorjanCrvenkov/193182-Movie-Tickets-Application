using Domain.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Interface
{
    public interface IUserService
    {
        IEnumerable<TicketingUser> GetAllUsers();
        TicketingUser GetDetailsForUser(String id);
        void Update(TicketingUser entity);
    }
}
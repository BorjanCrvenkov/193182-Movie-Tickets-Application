using Domain.DomainModels;
using Domain.Relations;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Identity
{
    public class TicketingUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }

        public string Image { get; set; }

        public virtual ShoppingCart UserCart { get; set; }

    }
}


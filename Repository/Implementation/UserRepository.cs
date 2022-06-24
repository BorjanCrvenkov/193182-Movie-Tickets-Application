using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<TicketingUser> entities;
        private readonly UserManager<TicketingUser> _userManager;
        string errorMessage = string.Empty;

        public UserRepository(ApplicationDbContext context, UserManager<TicketingUser> userManager)
        {
            this.context = context;
            entities = context.Set<TicketingUser>();
            _userManager = userManager;
        }
        public IEnumerable<TicketingUser> GetAll()
        {
            return entities.AsEnumerable();
        }

        public TicketingUser Get(string id)
        {
            return entities
               .Include(z => z.UserCart)
               .Include("UserCart.TicketsInShoppingCart")
               .Include("UserCart.TicketsInShoppingCart.CurrentTicket")
               .SingleOrDefault(s => s.Id == id);
        }
        public void Insert(TicketingUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Add(entity);
            context.SaveChanges();
        }

        public void Delete(TicketingUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Remove(entity);
            context.SaveChanges();
        }

        public void Update(TicketingUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Update(entity);
            context.SaveChanges();
        }

    }
}
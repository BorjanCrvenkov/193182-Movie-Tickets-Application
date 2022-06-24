using Domain.DomainModels;
using Domain.Identity;
using Domain.Relations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    public class ApplicationDbContext : IdentityDbContext<TicketingUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<TicketsInShoppingCart> TicketsInShoppingCarts { get; set; }
        public virtual DbSet<TicketingUser> TicketingUsers { get; set; }

        public virtual DbSet<Genre> Genres { get; set; }

        public virtual DbSet<TicketsTypeGenres> TicketsGenres { get; set; }

        public virtual DbSet<EmailMessage> EmailMessages { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TicketingUser>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<Ticket>()
                .Property(r => r.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<ShoppingCart>()
               .Property(r => r.Id)
               .ValueGeneratedOnAdd();

            builder.Entity<TicketsInShoppingCart>()
                .Property(r => r.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<TicketsInShoppingCart>()
                .HasOne(r => r.CurrentTicket)
                .WithMany(r => r.TicketsInShoppingCart)
                .HasForeignKey(r => r.TicketId);

            builder.Entity<TicketsInShoppingCart>()
                .HasOne(r => r.UserCart)
                .WithMany(r => r.TicketsInShoppingCart)
                .HasForeignKey(r => r.ShoppingCartId);

            builder.Entity<ShoppingCart>()
                .HasOne<TicketingUser>(z => z.Owner)
                .WithOne(r => r.UserCart)
                .HasForeignKey<ShoppingCart>(z => z.OwnerId);

            builder.Entity<TicketsInOrder>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<TicketsInOrder>()
                .HasOne(z => z.Ticket)
                .WithMany(z => z.TicketsInOrders)
                .HasForeignKey(z => z.TicketId);

            builder.Entity<TicketsInOrder>()
                .HasOne(z => z.Order)
                .WithMany(z => z.TicketsInOrders)
                .HasForeignKey(z => z.OrderId);

            builder.Entity<TicketsTypeGenres>()
                .HasOne(z => z.Ticket)
                .WithMany(z => z.TicketsTypeGenres)
                .HasForeignKey(z => z.TicketId);

            builder.Entity<TicketsTypeGenres>()
                .HasOne(z => z.Genre)
                .WithMany(z => z.TicketsTypeGenres)
                .HasForeignKey(z => z.GenreId);

        }

    }
}

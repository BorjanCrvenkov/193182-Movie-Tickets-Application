using Domain.DomainModels;
using Domain.DTO;
using Domain.Relations;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Implementation
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IRepository<TicketsInShoppingCart> _ticketsInShoppingCartRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Genre> _genreRepository;
        private readonly IRepository<TicketsTypeGenres> _ticketsTypeGenresRepository ;

        public TicketService(ITicketRepository ticketRepository, 
            IRepository<TicketsInShoppingCart> ticketsInShoppingCartRepository, 
            IUserRepository userRepository, IRepository<Genre> genreRepository,
            IRepository<TicketsTypeGenres> ticketsTypeGenresRepository)
        {
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _ticketsInShoppingCartRepository = ticketsInShoppingCartRepository;
            _genreRepository = genreRepository;
            _ticketsTypeGenresRepository = ticketsTypeGenresRepository;
        }

        public bool AddGenreToTicket(Guid TicketId, List<Guid> genres)
        {
            bool flag = false;

            if (TicketId != null && genres != null)
            {
                Ticket ticket = this.GetDetailsForTicket(TicketId);


                foreach(var genreId in genres)
                {

                    Genre genre = this._genreRepository.Get(genreId);

                    if (ticket != null)
                    {
                        //ticket.TicketGenres.Add(genre);
                        TicketsTypeGenres type = new TicketsTypeGenres
                        {
                            Id = Guid.NewGuid(),
                            Ticket = ticket,
                            TicketId = ticket.Id,
                            Genre = genre,
                            GenreId = genre.Id
                        };

                        
                        this._ticketsTypeGenresRepository.Insert(type);
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                }

            }
            return flag;
        }

        public bool AddToShoppingCart(AddToShoppingCartDto item, string userID)
        {
            var user = this._userRepository.Get(userID);

            var userShoppingCart = user.UserCart;

            if (item.SelectedTicketId != null && userShoppingCart != null)
            {
                var ticket = this.GetDetailsForTicket(item.SelectedTicketId);

                if (ticket != null)
                {
                    TicketsInShoppingCart itemToAdd = new TicketsInShoppingCart
                    {
                        Id = Guid.NewGuid(),
                        CurrentTicket = ticket,
                        TicketId = ticket.Id,
                        UserCart = userShoppingCart,
                        ShoppingCartId = userShoppingCart.Id,
                        Quantity = item.Quantity
                    };

                    var existing = userShoppingCart.TicketsInShoppingCart.Where(z => z.ShoppingCartId == userShoppingCart.Id && z.TicketId == itemToAdd.TicketId).FirstOrDefault();

                    if (existing != null)
                    {
                        existing.Quantity += itemToAdd.Quantity;
                        this._ticketsInShoppingCartRepository.Update(existing);

                    }
                    else
                    {
                        this._ticketsInShoppingCartRepository.Insert(itemToAdd);
                    }
                    return true;
                }
                return false;
            }
            return false;
        }

        public void CreateNewTicket(Ticket t)
        {
            this._ticketRepository.Insert(t);
        }

        public void DeleteTicket(Guid? id)
        {
            var ticket = this.GetDetailsForTicket(id);
            this._ticketRepository.Delete(ticket);
        }

        public List<Ticket> filterByDateAndGenre(DateTime? start, Guid? genreId)
        {
            List<Ticket> tickets = new List<Ticket>();

            if (start != null && genreId != null)
            {
                DateTime date = (DateTime)start;
                tickets = this.GetAllTicketsByGenre(genreId).Where(t => (date.Year == t.StartTime.Year) && (date.Month == t.StartTime.Month) && (date.Day == t.StartTime.Day)).ToList();
            }
            else if (start != null)
            {
                DateTime date = (DateTime)start;
                tickets = this.GetAllTickets().Where(t => (date.Year == t.StartTime.Year) && (date.Month == t.StartTime.Month) && (date.Day == t.StartTime.Day)).ToList();
            }
            else if (genreId != null)
            {
                tickets = this.GetAllTicketsByGenre(genreId);
            }
            else
            {
                tickets = this.GetAllTickets();
            }

            return tickets;
        }

        public List<Ticket> GetAllTickets()
        {
            return this._ticketRepository.getAllTickets();
        }

        public List<Ticket> GetAllTicketsByGenre(Guid? genreId)
        {
            Genre genre = this._genreRepository.Get(genreId);

            List<Ticket> tickets = this.GetAllTickets();

            List<Ticket> result = new List<Ticket>();

            foreach(var item in tickets)
            {
                foreach(var i in item.TicketsTypeGenres)
                {
                    if(i.Genre == genre)
                    {
                        result.Add(item);
                    }
                }
            }

            return result;
        }

        public Ticket GetDetailsForTicket(Guid? id)
        {
            return this._ticketRepository.getTicketDetails(id);
        }

        public AddToShoppingCartDto GetShoppingCartInfo(Guid? id)
        {
            var ticket = this.GetDetailsForTicket(id);
            AddToShoppingCartDto model = new AddToShoppingCartDto
            {
                SelectedTicket = ticket,
                SelectedTicketId = ticket.Id,
                Quantity = 1
            };

            return model;
        }

        public void UpdeteExistingTicket(Ticket t)
        {
            this._ticketRepository.Update(t);
        }



    }
}


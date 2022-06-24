using Domain.DomainModels;
using Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Interface
{
    public interface ITicketService
    {
        List<Ticket> GetAllTickets();
        Ticket GetDetailsForTicket(Guid? id);
        void CreateNewTicket(Ticket t);
        void UpdeteExistingTicket(Ticket t);
        AddToShoppingCartDto GetShoppingCartInfo(Guid? id);
        void DeleteTicket(Guid? id);
        bool AddToShoppingCart(AddToShoppingCartDto item, string userID);
        List<Ticket> filterByDateAndGenre(DateTime? start, Guid? genreId);

        bool AddGenreToTicket(Guid TicketId, List<Guid> genres);

        List<Ticket> GetAllTicketsByGenre(Guid? genreId);

    }
}
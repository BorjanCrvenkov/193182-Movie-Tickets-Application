using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domain.DomainModels;
using Repository;
using Domain.DTO;
using System.Security.Claims;
using Service.Interface;
using ClosedXML.Excel;
using System.Text;
using System.IO;

namespace Web.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly IGenreService _genreService;

        public TicketsController(ITicketService ticketService, IGenreService genreService)
        {
            _ticketService = ticketService;
            _genreService = genreService;
        }

        // GET: Tickets
        public IActionResult Index(DateTime? date,Guid? genreId)
        {
            ViewBag.Genres = this._genreService.getAllGenres();
            return View(this._ticketService.filterByDateAndGenre(date,genreId));
        }

        // GET: Tickets/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = this._ticketService.GetDetailsForTicket(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewBag.Genres = this._genreService.getAllGenres();
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Title,StartTime,ReleasedDate,TicketDescription,TicketPrice,TicketRating,TicketImage")] Ticket ticket,
            List<Guid> genres)
        {
            if (ModelState.IsValid)
            {
                ticket.Id = Guid.NewGuid();
                //ticket.TicketGenres = new List<Genre>();
                this._ticketService.CreateNewTicket(ticket);
                this._ticketService.AddGenreToTicket(ticket.Id, genres);
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = this._ticketService.GetDetailsForTicket(id);
            if (ticket == null)
            {
                return NotFound();
            }

            ViewBag.Genres = this._genreService.getAllGenres();

            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Id,Title,StartTime,ReleasedDate,TicketDescription,TicketPrice,TicketRating,TicketImage")] Ticket ticket,
            List<Guid> genres)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    this._ticketService.UpdeteExistingTicket(ticket);
                    this._ticketService.AddGenreToTicket(ticket.Id, genres);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = this._ticketService.GetDetailsForTicket(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            this._ticketService.DeleteTicket(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult AddTicketToCart(Guid id)
        {
            var result = this._ticketService.GetShoppingCartInfo(id);

            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTicketToCart(AddToShoppingCartDto model)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = this._ticketService.AddToShoppingCart(model, userId);

            if (result)
            {
                return RedirectToAction("Index", "Tickets");
            }
            return View(model);

        }

        public IActionResult ExportTickets()
        {
            List<Genre> genres = this._genreService.getAllGenres();

            return View(genres);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public FileContentResult ExportTickets(Guid? genreId)
        {

            List<Ticket> tickets = this._ticketService.GetAllTicketsByGenre(genreId);

            Genre genre = this._genreService.getGenreDetails(genreId);

            string fileName = genre.GenreName + " Tickets.xlsx";

            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            using(var workBook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workBook.Worksheets.Add(genre.GenreName + " Tickets");

                worksheet.Cell(1, 1).Value = "Ticket Name";
                worksheet.Cell(1, 2).Value = "Ticket Date";
                worksheet.Cell(1, 3).Value = "Ticket Rating";
                worksheet.Cell(1, 4).Value = "Ticket Price";
                worksheet.Cell(1, 5).Value = "Ticket Description";
                worksheet.Cell(1, 6).Value = "Movie Released Date";
                worksheet.Cell(1, 7).Value = "Ticket Genres";

                StringBuilder sb = new StringBuilder();

                for (int i = 1; i <= tickets.Count(); i++)
                {
                    var ticket = tickets[i - 1];

                    worksheet.Cell(i + 1, 1).Value = ticket.Title;
                    worksheet.Cell(i + 1, 2).Value = ticket.StartTime;
                    worksheet.Cell(i + 1, 3).Value = ticket.TicketRating;
                    worksheet.Cell(i + 1, 4).Value = ticket.TicketPrice;
                    worksheet.Cell(i + 1, 5).Value = ticket.TicketDescription;
                    worksheet.Cell(i + 1, 6).Value = ticket.ReleasedDate;

                    foreach (var item in ticket.TicketsTypeGenres)
                    {
                        sb.Append(item.Genre.GenreName + " ");
                    }

                    worksheet.Cell(i + 1, 7).Value = sb.ToString();

                }

                using (var stream = new MemoryStream())
                {
                    workBook.SaveAs(stream);

                    var content = stream.ToArray();

                    return File(content, contentType, fileName);
                }

            }

        }


        private bool TicketExists(Guid id)
        {
            return this._ticketService.GetDetailsForTicket(id) != null;
        }

    }
}


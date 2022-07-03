using Domain;
using Domain.DomainModels;
using Domain.Enumerations;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<TicketingUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IGenreService genreService;
        public HomeController(ILogger<HomeController> logger, UserManager<TicketingUser> userManager,
            RoleManager<IdentityRole> roleManager, IGenreService genreService)
        {
            _logger = logger;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.genreService = genreService;
        }

        public async Task <IActionResult> Index()
        {
            var userId = userManager.GetUserId(HttpContext.User);
            TicketingUser user = this.userManager.FindByIdAsync(userId).Result;
            
            await CreateRoles();
            await CreateAdmin();
            
            CreateGenres();

            return View(user);
        }

        private async Task CreateAdmin()
        {
            var userCheck = await userManager.FindByEmailAsync("admin@mail.com");
            if (userCheck == null)
            {
                var user = new TicketingUser
                {
                    FirstName = "Admin",
                    LastName = "Account",
                    UserName = "admin@mail.com",
                    NormalizedUserName = "admin@mail.com",
                    Email = "admin@mail.com",
                    EmailConfirmed = true,
                    Image = "https://icon-library.com/images/unknown-person-icon/unknown-person-icon-4.jpg",
                    UserCart = new ShoppingCart()
                };
                var result = await userManager.CreateAsync(user, "Admin123!");

                if (result.Succeeded)
                {
                    var result1 = await userManager.AddToRoleAsync(user, Roles.ADMINISTRATOR.ToString());
                }
            }

                
        }

        private async Task CreateRoles()
        {
            var roles = Enum.GetValues(typeof(Roles)).Cast<Roles>().ToList();

            foreach(var item in roles)
            {
                var flag = await roleManager.RoleExistsAsync(item.ToString());
                if (!flag)
                {
                    await roleManager.CreateAsync(new IdentityRole(item.ToString()));
                }
            }
        }

        private void CreateGenres() {
            var enumGenres = Enum.GetValues(typeof(Genres)).Cast<Genres>().ToList();

            var databaseGenres = genreService.getAllGenres();

            foreach (var item in enumGenres)
            {
                if (databaseGenres.FindIndex(g => g.Name.Equals(item.ToString())) == -1)
                {
                    Genre g = new Genre
                    {
                        Id = new Guid(),
                        Name = item.ToString()
                    };

                    genreService.createNewGenre(g);
                }

            }

        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}


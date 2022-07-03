using Domain.DomainModels;
using Domain.Enumerations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Service.Implementation;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web
{
    public static class DataInitializer
    {
        public static async Task CreateRoles(IServiceProvider serviceProvider)
        {

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if(await roleManager.RoleExistsAsync("ADMINISTRATOR"))
            {
                await roleManager.CreateAsync(new IdentityRole("ADMINISTRATOR"));
            }

            if (await roleManager.RoleExistsAsync("STANDARD"))
            {
                await roleManager.CreateAsync(new IdentityRole("STANDARD"));
            }

        }

        public static void CreateGenres(IServiceProvider serviceProvider)
        {
            var enumGenres = Enum.GetValues(typeof(Genres)).Cast<Genres>().ToList();

            var genreService = serviceProvider.GetRequiredService<IGenreService>();

            var databaseGenres = genreService.getAllGenres();

            foreach (var item in enumGenres)
            {
               if(databaseGenres.FindIndex(g => g.Name.Equals(item)) == -1) 
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

    }
}

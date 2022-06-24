using Domain.DomainModels;
using Domain.DTO;
using Domain.Identity;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<TicketingUser> userManager;
        private readonly SignInManager<TicketingUser> signInManager;
        private readonly IUserService userService;

        public AccountController(UserManager<TicketingUser> userManager,
            SignInManager<TicketingUser> signInManager, IUserService userService
            )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.userService = userService;
        }

        public IActionResult Register()
        {
            UserRegistrationDto model = new UserRegistrationDto();
            return View(model);
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register(UserRegistrationDto request)
        {
            if (ModelState.IsValid)
            {
                var userCheck = await userManager.FindByEmailAsync(request.Email);
                if (userCheck == null)
                {
                    var user = new TicketingUser
                    {
                        FirstName = request.LastName,
                        LastName = request.LastName,
                        UserName = request.Email,
                        NormalizedUserName = request.Email,
                        Email = request.Email,
                        EmailConfirmed = true,
                        UserType = Domain.Enumerations.UserType.STANDARD,
                        UserImage = "https://icon-library.com/images/unknown-person-icon/unknown-person-icon-4.jpg",
                        UserCart = new ShoppingCart()
                    };
                    var result = await userManager.CreateAsync(user, request.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        if (result.Errors.Count() > 0)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("message", error.Description);
                            }
                        }
                        return View(request);
                    }
                }
                else
                {
                    ModelState.AddModelError("message", "Email already exists.");
                    return View(request);
                }
            }
            return View(request);

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            UserLoginDto model = new UserLoginDto();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError("message", "Email not confirmed yet");
                    return View(model);

                }
                if (await userManager.CheckPasswordAsync(user, model.Password) == false)
                {
                    ModelState.AddModelError("message", "Invalid credentials");
                    return View(model);

                }

                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);

                if (result.Succeeded)
                {
                    await userManager.AddClaimAsync(user, new Claim("UserRole", "Admin"));
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("message", "Invalid login attempt");
                    return View(model);
                }
            }
            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult ManageUsers()
        {
            return View(this.userService.GetAllUsers());
        }

        public IActionResult Details(String id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = this.userService.GetDetailsForUser(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public IActionResult Edit(String id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = this.userService.GetDetailsForUser(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(String id, [Bind("Id,FirstName,LastName,Email,Address,UserType,UserImage")] TicketingUser user)
        {

            if (id != user.Id)
            {
                return Redirect("/error");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user1 = await this.userManager.FindByIdAsync(user.Id);
                    user1.FirstName = user.FirstName;
                    user1.LastName = user.LastName;
                    user1.Email = user.Email;
                    user1.Address = user.Address;
                    user1.UserType = user.UserType;
                    user1.UserImage = user.UserImage;
                    await this.userManager.UpdateAsync(user1);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (this.userService.GetDetailsForUser(id) != null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect("/Account/Details/" + id);
            }

            return Redirect("/error");
        }

        public IActionResult ImportUsers()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportUsers(IFormFile file)
        {
            string pathToUpload = $"{Directory.GetCurrentDirectory()}\\files\\{file.FileName}";

            using (FileStream fileStream = System.IO.File.Create(pathToUpload))
            {
                file.CopyTo(fileStream);

                fileStream.Flush();
            }

            await getUsersFromExcelFile(file.FileName);

            return Redirect("/Account/ManageUsers");
        }

        private async Task getUsersFromExcelFile(string fileName)
        {
            string pathToFile = $"{Directory.GetCurrentDirectory()}\\files\\{fileName}";

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            List<TicketingUser> userList = new List<TicketingUser>();

            using (var stream = System.IO.File.Open(pathToFile, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        var userCheck = userManager.FindByEmailAsync(reader.GetValue(2).ToString()).Result;

                        bool passwordsMatch = reader.GetValue(3).ToString().Equals(reader.GetValue(4).ToString());
                        if (userCheck == null && passwordsMatch){

                            TicketingUser user = new TicketingUser
                            {
                                FirstName = reader.GetValue(0).ToString(),
                                LastName = reader.GetValue(1).ToString(),
                                UserName = reader.GetValue(2).ToString(),
                                NormalizedUserName = reader.GetValue(2).ToString(),
                                Email = reader.GetValue(2).ToString(),
                                EmailConfirmed = true,
                                UserType = Domain.Enumerations.UserType.ADMINISTRATOR.ToString().Equals(reader.GetValue(5).ToString()) ? Domain.Enumerations.UserType.ADMINISTRATOR : Domain.Enumerations.UserType.STANDARD,
                                UserImage = "https://icon-library.com/images/unknown-person-icon/unknown-person-icon-4.jpg",
                                UserCart = new ShoppingCart()
                            };
                            var result = await userManager.CreateAsync(user, reader.GetValue(4).ToString());
                        }
                        else
                        {
                            continue;
                        }

                    }
                }
            }
        }

        /*[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(String id)
        {
            this._userService.DeleteUser(id);
            return RedirectToAction(nameof(Index));
    }

        // GET: Tickets/Edit/5
        public IActionResult Edit(String id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = this._userService.GetDetailsForUser(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(String id, [Bind("Id,UserName,Email,FirstName,LastName,UserType")] TicketingUser user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                this._userService.GetDetailsForUser(id);
                }
                catch (DbUpdateConcurrencyException)
                {

                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }
        */


    }
}


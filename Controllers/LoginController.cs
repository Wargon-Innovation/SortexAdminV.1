using Microsoft.AspNetCore.Mvc;
using SortexAdminV._1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace SortexAdminV._1.Controllers
{
    public class LoginController : Controller
    {
        private readonly SortexDBContext _context;

        public LoginController(SortexDBContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var userList = _context.Users.ToList();
            if(userList.Count == 0 || userList == null)
            {
                User newUser = new User();
                newUser.UserName = "Admin";
                newUser.Password = "sorTEXHVAdmin";
                CreateUser(newUser);
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> IndexAsync(User user)
        {
           

            bool confirmedLogin = CheckUser(user);

            if (confirmedLogin == true)
            {
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                return RedirectToAction("index", "Home");

            }
            return View();
        }

        private bool CheckUser(User userLogin) 
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userLogin.Password);

            var user = _context.Users.SingleOrDefault(x => x.UserName.ToLower() == userLogin.UserName.ToLower());
            
            if(user == null)
            {
                //ANVÄNDARE FINNS INTE
                return false;
            }
            bool isValidPassword = BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password);
            if (isValidPassword)
            {
                return true;
            }
            //LÖSENORD STÄMMER INTE
            return false;
        }

        private void CreateUser(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _context.Add(user);
            _context.SaveChanges();
        }
    }
}


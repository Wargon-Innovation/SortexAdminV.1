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

        private bool CheckUser(User user) 
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);

            var result = (from users in _context.Users
                          where users.Password == passwordHash && users.UserName == user.UserName
                          select users).FirstOrDefault();
            if (result != null)
            {
                return true;
            }            
           
            return false;
        }


    }
}


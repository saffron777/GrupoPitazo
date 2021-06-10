using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GP.Core.Modules.Account.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace GP.BackOffice.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountServices _accountServices;
        public AccountController(IAccountServices accountServices)
        {
            _accountServices = accountServices;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string userName, string password)
        {
            if (!string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(password))
            {
                return RedirectToAction("Login");
            }

            //Check the user name and password  
            //Here can be implemented checking logic from the database  
            ClaimsIdentity identity = null;
            bool isAuthenticated = false;

            if (userName == "Admin" && password == "6PP1t@Z0")
            {

                //Create the identity for the user  
                //var identity = new ClaimsIdentity(new[] {
                //    new Claim(ClaimTypes.Name, userName)
                //}, CookieAuthenticationDefaults.AuthenticationScheme);

                //var principal = new ClaimsPrincipal(identity);

                //var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);


                //Create the identity for the user  
                identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.Role, "Admin")
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                isAuthenticated = true;

                return RedirectToAction("Index", "Home");
            }



            var login = _accountServices.GetUserByLogin(userName, password);// _accountServices.GetUser(userName, password);
            if (login != null)
            {
                //Create the identity for the user  
                identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.Role, "User")
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                isAuthenticated = true;
            }


            if (isAuthenticated)
            {
                var principal = new ClaimsPrincipal(identity);

                var login2 = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        //[HttpPost]
        public IActionResult Logout()
        {
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
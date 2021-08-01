using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SM.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SM.Controllers
{
    public class HomeController : Controller
    {
        #region Fields

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public HomeController(
                   UserManager<User> userManager,
                   SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #endregion Fields

        #region Methods (public)

        // GET - Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST - Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User appUser)
        {
            if (!ModelState.IsValid)
            {
                return View(appUser);
            }

            // Read user by username
            var user = await _userManager.FindByNameAsync(appUser.UserName);

            if (user != null && await _userManager.CheckPasswordAsync(user, appUser.Password))
            {
                // Log in
                var result = await _signInManager.PasswordSignInAsync(user, user.Password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    return View();
                }    
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }
        }

        // GET - Register
        public IActionResult Register()
        {
            return View();
        }

        // POST - Register
        [HttpPost]
        public async Task<IActionResult> Register(User appUser)
        {
            var user = new User
            {
                UserName = appUser.UserName,
                Name = appUser.Name,
                Password = appUser.Password
            };

            // Create a new user
            var result = await _userManager.CreateAsync(user, user.Password);

            if (result.Succeeded)
            {
                // Login
                var loginResult = await _signInManager.PasswordSignInAsync(user, user.Password, false, false);

                if (loginResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ModelState.AddModelError("", "user cannot be created");
                return View();
            }

            return View();
        }

        public IActionResult Index()
        {
            return View();
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

        // Logout
        public async Task<IActionResult> LogOut(string username, string password)
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }

        #endregion Methods (public)
    }
}

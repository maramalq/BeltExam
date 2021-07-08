using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BeltExam.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace BeltExam.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _db;
        private int? uid
        {
            get { return HttpContext.Session.GetInt32("UserId"); }
        }
        private bool isLoggedIn
        {
            get { return uid != null; }
        }
        public HomeController(MyContext context)
        {
            _db = context;
        }

        [HttpGet("")]
        public IActionResult Redirect()
        {
            return Redirect("/sginin");
        }

        [HttpGet("sginin")]
        public IActionResult Index()
        {
            if( !isLoggedIn )
            {
                return View();
            }
            return RedirectToAction("Dashboard" , "Belt");
        }

        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            // Check initial ModelState => if there are no errors
            if (ModelState.IsValid)
            {
                // If a User exists with provided email
                if ( _db.Users.Any(u => u.Email == user.Email) )
                {
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError("Email", "Email already in use!");
                    // You may consider returning to the View at this point
                    return View("Index");
                }
                // if we reach here it confirms this is new user
                // add them to db... after we hash the password
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                // after hashing password add the user to db
                _db.Users.Add(user);
                _db.SaveChanges();
                // save their id to session
                HttpContext.Session.SetInt32("UserId", user.UserId);
                // redirect to Dashboard
                return RedirectToAction("Dashboard" , "Belt");
            }
            // other code
            return View("Index");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser user)
        {
            if (ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = _db.Users.FirstOrDefault(u => u.Email == user.LoginEmail);
                // If no user exists with provided email
                if (userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
                    return View("Index");
                }
                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();
                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(user, userInDb.Password, user.LoginPassword);
                // result can be compared to 0 for failure
                if (result == 0)
                {
                    // handle failure (this should be similar to how "existing email" is handled)
                    ModelState.AddModelError("LoginPassword", "Invalid Email/Password");
                    return View("Index");
                }
                // if result is not 0, then it is valid
                // Store user id into session
                HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                return RedirectToAction("Dashboard" , "Belt");
            }
            return View("Index");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
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
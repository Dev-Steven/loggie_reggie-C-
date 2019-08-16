using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using loggie_reggie.Models;
using Microsoft.EntityFrameworkCore;
using loggie_reggie.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;    // For session


namespace loggie_reggie.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User NewUser)
        {
            if(ModelState.IsValid)
            {
                        // If a User exists with provided email
                if(dbContext.Users.Any(u => u.Email == NewUser.Email))
                {
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError("Email", "Email already in use!");
                    
                    // You may consider returning to the View at this point
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    NewUser.Password = Hasher.HashPassword(NewUser, NewUser.Password);
                    dbContext.Add(NewUser);
                    dbContext.SaveChanges();
                    NewUser.CreatedAt = DateTime.Now;
                    NewUser.UpdatedAt = DateTime.Now;
                    dbContext.SaveChanges();
                    
                    HttpContext.Session.SetInt32("id",dbContext.Users.Last().Id);

                    return RedirectToAction("Success");
                }   
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost("logging")]
        public IActionResult Logging(LoginUser userSubmission)
        {
            if(ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
                // If no user exists with provided email
                if(userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Login");
                }
                
                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();
                
                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
                
                // result can be compared to 0 for failure
                if(result == 0)
                {
                    // handle failure (this should be similar to how "existing email" is handled)
                    return View("Login");
                }

                HttpContext.Session.SetInt32("id",dbContext.Users.Last().Id);

                return RedirectToAction("Success");
            }
            else
            {
                return View("Login");
            }
        }

        [HttpGet("success")]
        public IActionResult Success()
        {
            int? UserId = HttpContext.Session.GetInt32("id");
            if(UserId == null)
            {
                return View("Login");
            }
            return View("Success");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MeetUp.Models;


namespace MeetUp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MeetUpContext _context;

        public HomeController(ILogger<HomeController> logger, MeetUpContext context)
        {
            _logger = logger;
            _context = context;
        }

        // ====================
        // ====================== Home Route - Display Homepage
        // ====================
        [HttpGet("/")]
        public IActionResult Index()
        {
            return View();
        }

        // ====================
        // ====================== Registration form loader
        // ====================
        [HttpGet("/Register")]
        public IActionResult RegistrationForm()
        {
            return View();
        }
        // ====================
        // ====================== Register - manage registration and admit on success
        // ====================
        [HttpPost("Register")]
        public IActionResult Register(User user)
        {
            if(ModelState.IsValid)
            {
                if(_context.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    user.Password = Hasher.HashPassword(user, user.Password);
                    _context.Add(user);
                    _context.SaveChanges();
                    User u = _context.Users.FirstOrDefault(us => us.Email == user.Email); 
                    HttpContext.Session.SetInt32("UserId", u.UserId);
                    HttpContext.Session.SetString("FirstName", u.FirstName);
                    return RedirectToAction("Success");
                }
            }
            else
            {
                return View("Index");
            }
        }

        // ====================
        // ====================== Login Form loader
        // ====================
        [HttpGet("/Login")]
        public IActionResult LoginForm()
        {
            return View();
        }

        // ====================
        // ====================== Login route - Check credential and admit or display errors
        // ====================
        [HttpPost("Login")]
        public IActionResult Login(LoginView formUser)
        {
            if(ModelState.IsValid)
            {
                var dbUser = _context.Users.SingleOrDefault(u => u.Email == formUser.Email);
                if(dbUser == null)
                {
                    ModelState.AddModelError("Email", "Invalid Login");
                    ModelState.AddModelError("Password", "Invalid Login");
                    return View("Index");
                }
                var Hasher = new PasswordHasher<LoginView>();
                if (Hasher.VerifyHashedPassword(formUser,dbUser.Password,formUser.Password) == 0)
                {
                    ModelState.AddModelError("Email", "Invalid Login");
                    ModelState.AddModelError("Password", "Invalid Login");
                    return View("Index");
                }
                else
                {
                    SetUserInSession(dbUser);
                    return RedirectToAction("Success");
                }

            }
            else
            {
                return View("Index");
            }
        }

        // ====================
        // ====================== Login/Reg Success
        // ====================
        [HttpGet("Success")]
        public IActionResult Success()
        {
            if(HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Dashboard", "Meetup");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // ====================
        // ====================== Logout - Clear Session and Redir to Homepage
        // ====================
        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        // ====================
        // ====================== Unauthorized Access to Route
        // ====================
        [HttpGet("/UnauthorizedAccess")]
        public IActionResult UnauthorizedAccess()
        {
            return View();
        }

        [Route("/{**BadRoute}")]
        public IActionResult BadRoute()
        {
            return View("UnauthorizedAccess");
        }

        // ====================
        // ====================== Error Catch
        // ====================

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // ====================== User Management and Validation Methods
        private void SetUserInSession(User user)
        {
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("FirstName", user.FirstName);
        }
        
        private bool LoggedIn()
        { 
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

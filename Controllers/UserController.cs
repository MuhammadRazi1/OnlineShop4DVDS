using Microsoft.AspNetCore.Mvc;
using OnlineShop4DVDS.Models;
using OnlineShop4DVDS.SqlDbContext;
using System.Reflection.Metadata.Ecma335;

namespace OnlineShop4DVDS.Controllers
{
    public class UserController : Controller
    {
        SqlContext sqlContext;

        public UserController(SqlContext sqlContext)
        {
            this.sqlContext = sqlContext;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model state is invalid.");
                return View(user);
            }

            if (sqlContext.Users.Any(u => u.UserEmail == user.UserEmail))
            {
                ModelState.AddModelError("UserEmail", "Email is already in use.");
                return View(user);
            }

            user.UserPassword = HashPassword(user.UserPassword);
            user.UserRole = "Member";

            sqlContext.Users.Add(user);
            sqlContext.SaveChanges();

            ModelState.Clear();
            HttpContext.Session.SetString("UserRole", "Guest");

            Console.WriteLine($"User registered: {user.UserEmail}, Session Role: {HttpContext.Session.GetString("UserRole")}");

            return RedirectToAction("Login");
        }


        private string HashPassword(string password)
        {
            using(var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return System.Convert.ToBase64String(bytes);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User loginUser)
        {
            var user = sqlContext.Users.FirstOrDefault(u => u.UserEmail == loginUser.UserEmail);

            if(user == null)
            {
                ModelState.AddModelError("UserEmail", "User does not exist");
                return View(loginUser);
            }

            if(user.UserPassword != HashPassword(loginUser.UserPassword))
            {
                ModelState.AddModelError("UserPassword", "Invalid password.");
                return View(loginUser);
            }

            HttpContext.Session.SetString("UserRole", user.UserRole);
            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("UserEmail", user.UserEmail);

            Console.WriteLine($"User {user.UserEmail} logged in successfully with role {user.UserRole}");

            if(user.UserRole == "Admin")
            {
                return RedirectToAction("Admin", "Index");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("UserEmail");

            return RedirectToAction("Login", "User");
        }

        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("UserName");

            ViewBag.UserName = username;
            return View();
        }
        public IActionResult Profile()
        {
            var username = HttpContext.Session.GetString("UserName");
            var useremail = HttpContext.Session.GetString("UserEmail");

            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(useremail)){
                return RedirectToAction("Login");
            }

            ViewBag.UserName = username;
            ViewBag.UserEmail = useremail;

            return View();
        }
    }
}

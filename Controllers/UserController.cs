using Microsoft.AspNetCore.Mvc;
using OnlineShop4DVDS.Models;
using OnlineShop4DVDS.SqlDbContext;

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
                return View(user);
            }

            if(sqlContext.Users.Any(u => u.UserEmail == user.UserEmail))
            {
                ModelState.AddModelError("UserEmail", "Email is already in use.");
                return View(user);
            }

            user.UserPassword = HashPassword(user.UserPassword);
            user.UserRole = "Member";

            sqlContext.Users.Add(user);
            sqlContext.SaveChanges();

            HttpContext.Session.SetString("UserRole", "Guest");

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
    }
}

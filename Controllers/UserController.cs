using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        //User Register

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
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return System.Convert.ToBase64String(bytes);
            }
        }

        //User Login

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User loginUser)
        {
            var user = sqlContext.Users.FirstOrDefault(u => u.UserEmail == loginUser.UserEmail);

            if (user == null)
            {
                ModelState.AddModelError("UserEmail", "User does not exist");
                return View(loginUser);
            }

            if (user.UserPassword != HashPassword(loginUser.UserPassword))
            {
                ModelState.AddModelError("UserPassword", "Invalid password.");
                return View(loginUser);
            }

            HttpContext.Session.SetString("UserRole", user.UserRole);
            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("UserEmail", user.UserEmail);
            HttpContext.Session.SetInt32("UserId", user.UserId);

            Console.WriteLine($"User {user.UserEmail} logged in successfully with role {user.UserRole}");

            if (user.UserRole == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            return RedirectToAction("Index");
        }

        //User Logout

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("UserEmail");
            HttpContext.Session.SetString("UserRole", "Guest");

            Console.WriteLine($"User role changed to Session Role: {HttpContext.Session.GetString("UserRole")}");

            return RedirectToAction("Login");
        }

        //Index View

        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("UserName");

            ViewBag.UserName = username;
            return View();
        }

        //Profile View

        public IActionResult Profile()
        {
            var username = HttpContext.Session.GetString("UserName");
            var useremail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(useremail))
            {
                return RedirectToAction("Login");
            }

            ViewBag.UserName = username;
            ViewBag.UserEmail = useremail;

            return View();
        }

        //Profile Update

        [HttpGet]
        public IActionResult ProfileUpdate()
        {
            string? useremail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(useremail))
            {
                return RedirectToAction("Login");
            }

            var user = sqlContext.Users.FirstOrDefault(u => u.UserEmail == useremail);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var userName = HttpContext.Session.GetString("UserName");

            if (!string.IsNullOrEmpty(userName))
            {
                ViewBag.UserName = userName;
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult ProfileUpdate(User updateUser)
        {
            string? useremail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(useremail))
            {
                return RedirectToAction("Login");
            }

            var user = sqlContext.Users.FirstOrDefault(u => u.UserEmail == useremail);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            if (sqlContext.Users.Any(u => u.UserEmail == updateUser.UserEmail && u.UserEmail != useremail))
            {
                ModelState.AddModelError("UserEmail", "This email is already in use.");
                return View(updateUser);
            }

            user.UserEmail = updateUser.UserEmail;
            user.UserName = updateUser.UserName;

            sqlContext.SaveChanges();

            HttpContext.Session.SetString("UserEmail", user.UserEmail);
            HttpContext.Session.SetString("UserName", user.UserName);

            Console.WriteLine($"User email updated to: {HttpContext.Session.GetString("UserEmail")}");
            Console.WriteLine($"User name updated to: {HttpContext.Session.GetString("UserName")}");

            return RedirectToAction("Profile");
        }

        //Account Delete

        [HttpPost]
        public IActionResult DeleteAccount()
        {
            string? useremail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(useremail))
            {
                return RedirectToAction("Login");
            }

            var user = sqlContext.Users.FirstOrDefault(u => u.UserEmail == useremail);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            sqlContext.Users.Remove(user);
            sqlContext.SaveChanges();

            HttpContext.Session.Clear();

            return RedirectToAction("Register");
        }

        //Album Page

        public IActionResult AlbumPage()
        {
            var album = sqlContext.Albums.ToList();
            return View(album);
        }

        public IActionResult SingleAlbum(int id)
        {
            var albums = sqlContext.Albums.Include(a => a.Artist).FirstOrDefault(a => a.AlbumId == id);
            if (albums == null)
            {
                return NotFound();
            };

            var songs = sqlContext.Songs.Where(s => s.AlbumId == id).ToList();
            ViewBag.Songs = songs;

            return View(albums);
        }

        //Game Page

        public IActionResult GamePage()
        {
            var game = sqlContext.Games.ToList();
            return View(game);
        }

        public IActionResult SingleGame(int id)
        {
            var games = sqlContext.Games
                .Include(d => d.Developer)
                .Include(gp => gp.GamePlatforms)
                    .ThenInclude(p => p.Platform)
                .Include(gg => gg.GameGenres)
                    .ThenInclude(g => g.Genre)
                .FirstOrDefault(g => g.GameId == id);

            return View(games);
        }

        public IActionResult GameTrailer(int id)
        {
            var game = sqlContext.Games
                .FirstOrDefault(g => g.GameId == id);
            return View(game);
        }

        //Movie Page

        public IActionResult MoviePage()
        {
            var movie = sqlContext.Movies
                 .Include(mg => mg.MovieGenres)
                     .ThenInclude(g => g.Genre)
                 .ToList();
            return View(movie);
        }

        public IActionResult SingleMovie(int id)
        {
            var movie = sqlContext.Movies
                .Include(mg => mg.MovieGenres)
                    .ThenInclude(g => g.Genre)
                .FirstOrDefault(m => m.MovieId == id);

            return View(movie);
        }

        public IActionResult MovieTrailer(int id)
        {
            var movie = sqlContext.Movies
                .FirstOrDefault(m => m.MovieId == id);
            return View(movie);
        }

        //News Page

        public IActionResult NewsPage()
        {
            var news = sqlContext.News.ToList();
            return View(news);
        }

        //Feedback Page

        public IActionResult FeedbackPage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult FeedbackInsert(Feedback feedback)
        {
            if (!ModelState.IsValid)
            {
                return View(feedback);
            }

            var userId = HttpContext.Session.GetInt32("UserId");

            if(userId == null)
            {
                return RedirectToAction("Login");
            }

            feedback.UserId = userId.Value;
            sqlContext.Feedbacks.Add(feedback);
            sqlContext.SaveChanges();
            ModelState.Clear();
            ViewBag.FeedbackSuccess = "Feedback added successfully!";
            return RedirectToAction("FeedbackPage");
        }
    }
}

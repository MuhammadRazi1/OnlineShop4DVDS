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
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var album = sqlContext.Albums
                .Include(a => a.Artist)
                .FirstOrDefault(a => a.AlbumId == id);

            if (album == null)
            {
                return NotFound();
            }

            var songs = sqlContext.Songs
                .Where(s => s.AlbumId == id)
                .Include(s => s.Category)
                .ToList();

            var userCollection = new List<int>();
            if (userId.HasValue)
            {
                userCollection = sqlContext.UserSongs
                    .Where(us => us.UserId == userId.Value && songs.Select(s => s.SongId).Contains(us.SongId))
                    .Select(us => us.SongId)
                    .ToList();
            }

            ViewBag.Songs = songs;
            ViewBag.UserCollection = userCollection;

            return View(album);
        }

        //Song Page

        public IActionResult SongPage()
        {
            var song = sqlContext.Songs
                .Include(c => c.Category)
                .Include(a => a.Album)
                .ToList();
            return View(song);
        }

        public IActionResult SingleSong(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            //if(userId == null)
            //{
            //    return RedirectToAction("Login");
            //}

            var song = sqlContext.Songs
                .Include(c => c.Category)
                .Include(a => a.Album)
                .FirstOrDefault(s => s.SongId == id);

            if (song == null)
            {
                return NotFound(); // Handle case where the song doesn't exist
            }

            var isInCollection = sqlContext.UserSongs
                .Any(us => us.UserId == userId && us.SongId == id);

            ViewBag.IsInCollection = isInCollection; // Pass the collection status to the view

            return View(song);
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

            if (userId == null)
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

        //Collection Page

        [HttpPost]
        public IActionResult AddToCollection(int id, int albumId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var existingCollection = sqlContext.UserSongs.FirstOrDefault(us => us.UserId == userId && us.SongId == id);

            if(existingCollection == null)
            {
                var userSong = new UserSong
                {
                    UserId = userId.Value,
                    SongId = id,
                    AddedOn = System.DateTime.Now
                };

                sqlContext.UserSongs.Add(userSong);
                sqlContext.SaveChanges();

                TempData["CollectionMessage"] = "Song added to collection!";
                TempData["AddedSongId"] = id;
            }
            else
            {
                TempData["CollectionMessage"] = "Song already in collection!";
            }

            return RedirectToAction("SingleAlbum", new { id = albumId });
        }

        [HttpPost]
        public IActionResult RemoveFromCollection(int id) // id is the SongId
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            // Find the song in the user's collection
            var userSong = sqlContext.UserSongs
                .FirstOrDefault(us => us.UserId == userId && us.SongId == id);

            if (userSong != null)
            {
                // Remove the song from the collection
                sqlContext.UserSongs.Remove(userSong);
                sqlContext.SaveChanges();

                TempData["CollectionMessage"] = "Song removed from collection!";
            }
            else
            {
                TempData["CollectionMessage"] = "Song is not in your collection!";
            }

            return RedirectToAction("SingleSong", new { id }); // Redirect back to the song details page
        }

        public IActionResult CollectionView()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var collections = sqlContext.UserSongs
                .Include(s => s.Song)
                .ThenInclude(a => a.Album)
                .Include(c => c.Song.Category)
                .Where(us => us.UserId == userId)
                .ToList();

            if (TempData["CollectionMessage"] != null)
            {
                ViewBag.CollectionMessage = TempData["CollectionMessage"];
            }

            return View(collections);
        }

        //Review

        [HttpPost]
        public IActionResult AddReview(Review review)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                review.ReviewDate = DateTime.UtcNow;
                sqlContext.Reviews.Add(review);
                sqlContext.SaveChanges();

                TempData["ReviewMessage"] = "Review added successfully!";
            }
            else
            {
                TempData["ReviewMessage"] = "Failed to add review. Please check your input.";
            }

            if (review.AlbumId != null)
            {
                return RedirectToAction("SingleAlbum", "User", new { id = review.AlbumId });
            }
            else if (review.GameId != null)
            {
                return RedirectToAction("SingleGame", "User", new { id = review.GameId });
            }
            else if (review.MovieId != null)
            {
                return RedirectToAction("SingleMovie", "User", new { id = review.MovieId });
            }

            return RedirectToAction("Index");
        }
    }
}

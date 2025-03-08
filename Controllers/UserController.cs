using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Model.Tree;
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
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
                    }
                }
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

            var album = sqlContext.Albums
                .Include(a => a.Artist)
                .Include(r => r.Reviews)
                    .ThenInclude(u => u.User)
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
            ViewBag.AlbumId = album.AlbumId;
            ViewBag.Reviews = album.Reviews;

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

            var song = sqlContext.Songs
                .Include(c => c.Category)
                .Include(a => a.Album)
                .FirstOrDefault(s => s.SongId == id);

            if (song == null)
            {
                return NotFound();
            }

            var isInCollection = sqlContext.UserSongs
                .Any(us => us.UserId == userId && us.SongId == id);

            ViewBag.IsInCollection = isInCollection; 

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
                .Include(r => r.Reviews)
                    .ThenInclude(u => u.User)
                .FirstOrDefault(g => g.GameId == id);

            ViewBag.GameId = games.GameId;
            ViewBag.Reviews = games.Reviews;

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
                 .Include(r => r.Reviews)
                    .ThenInclude(u => u.User)
                .FirstOrDefault(m => m.MovieId == id);

            ViewBag.MovieId = movie.MovieId;
            ViewBag.Reviews = movie.Reviews;

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

            return RedirectToAction("SingleSong", new { id });
        }

        [HttpPost]
        public IActionResult RemoveFromCollection(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var userSong = sqlContext.UserSongs
                .FirstOrDefault(us => us.UserId == userId && us.SongId == id);

            if (userSong != null)
            {
                sqlContext.UserSongs.Remove(userSong);
                sqlContext.SaveChanges();

                TempData["CollectionMessage"] = "Song removed from collection!";
            }
            else
            {
                TempData["CollectionMessage"] = "Song is not in your collection!";
            }

            return RedirectToAction("SingleSong", new { id });
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

        //Cart

        public IActionResult CartView()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            // Retrieve the user's cart with items
            var cart = sqlContext.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId);

            // If the cart is null, create an empty list of cart items
            var cartItems = cart?.CartItems ?? new List<CartItem>();

            // Fetch item details (name and image) for each cart item
            var cartItemDetails = new List<CartItemDetailViewModel>();
            foreach (var item in cartItems)
            {
                var itemName = GetItemName(item.ItemType, item.ItemId);
                var itemImage = GetItemImage(item.ItemType, item.ItemId);
                var itemPlatform = GetPlatformName(item.ItemType, item.PlatformId);

                // Debugging: Print item details to the console
                Console.WriteLine($"ItemType: {item.ItemType}, ItemId: {item.ItemId}, ItemName: {itemName}, ItemImage: {itemImage}");

                var itemDetail = new CartItemDetailViewModel
                {
                    CartItem = item,
                    ItemName = itemName,
                    ItemImage = itemImage,
                    PlatformName = itemPlatform
                };
                cartItemDetails.Add(itemDetail);
            }

            // Pass the cart item details to the view
            return View(cartItemDetails);
        }

        // Helper method to get item name
        private string GetItemName(string itemType, int itemId)
        {
            switch (itemType)
            {
                case "Game":
                    return sqlContext.Games.FirstOrDefault(g => g.GameId == itemId)?.GameName;
                case "Movie":
                    return sqlContext.Movies.FirstOrDefault(m => m.MovieId == itemId)?.MovieTitle;
                case "Album":
                    return sqlContext.Albums.FirstOrDefault(a => a.AlbumId == itemId)?.AlbumTitle;
                default:
                    return "Unknown Item";
            }
        }

        // Helper method to get item image
        private string GetItemImage(string itemType, int itemId)
        {
            string imageName = null;

            switch (itemType)
            {
                case "Game":
                    imageName = sqlContext.Games.FirstOrDefault(g => g.GameId == itemId)?.GameImage;
                    break;
                case "Movie":
                    imageName = sqlContext.Movies.FirstOrDefault(m => m.MovieId == itemId)?.MovieImage;
                    break;
                case "Album":
                    imageName = sqlContext.Albums.FirstOrDefault(a => a.AlbumId == itemId)?.AlbumImage;
                    break;
            }

            // If the image name is null or empty, use the default image
            if (string.IsNullOrEmpty(imageName))
            {
                return "default-image.jpg"; // Default image in the root images folder
            }

            // Include the subfolder name based on the item type
            return $"{itemType.ToLower()}s/{imageName}";
        }

        private string GetPlatformName(string itemType, int? platformId)
        {
            if (itemType != "Game" || !platformId.HasValue)
            {
                return "N/A"; // Only games have platforms
            }

            var platform = sqlContext.Platforms.FirstOrDefault(p => p.PlatformId == platformId.Value);
            return platform?.PlatformName ?? "N/A";
        }

        [HttpPost]
        public IActionResult AddToCart(string itemType, int itemId, int? platformId)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                return RedirectToAction("Login");
            }

            var userId = HttpContext.Session.GetInt32("UserId");

            // Retrieve or create the user's cart
            var cart = sqlContext.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId.Value };
                sqlContext.Carts.Add(cart);
            }

            // Retrieve the item price based on the item type
            decimal price = 0;
            switch (itemType)
            {
                case "Game":
                    var game = sqlContext.Games.Find(itemId);
                    price = game?.GamePrice ?? 0;
                    break;
                case "Movie":
                    var movie = sqlContext.Movies.Find(itemId);
                    price = movie?.MoviePrice ?? 0;
                    break;
                case "Album":
                    var album = sqlContext.Albums.Find(itemId);
                    price = album?.AlbumPrice ?? 0;
                    break;
            }

            if (price == 0)
            {
                TempData["ErrorMessage"] = "Invalid item selected.";
                return RedirectToAction("Index", "Home");
            }

            // Check if the item is already in the cart
            var existingItem = cart.CartItems
                .FirstOrDefault(item => item.ItemType == itemType && item.ItemId == itemId && item.PlatformId == platformId);

            if (existingItem != null)
            {
                // If the item is already in the cart, increase the quantity
                existingItem.Quantity++;
            }
            else
            {
                // If the item is not in the cart, add it
                cart.CartItems.Add(new CartItem
                {
                    ItemType = itemType,
                    ItemId = itemId,
                    PlatformId = platformId,
                    Quantity = 1,
                    Price = price
                });
            }

            // Save changes to the database
            sqlContext.SaveChanges();

            // Redirect to the cart view
            return RedirectToAction("CartView");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int cartItemId)
        {
            // Retrieve the cart item from the database
            var cartItem = sqlContext.CartItems.FirstOrDefault(ci => ci.CartItemId == cartItemId);

            if (cartItem != null)
            {
                // Remove the cart item from the database
                sqlContext.CartItems.Remove(cartItem);
                sqlContext.SaveChanges();
            }

            // Redirect back to the cart view
            return RedirectToAction("CartView");
        }
    }
}

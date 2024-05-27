using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgriEnergyPOE.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace AgriEnergyPOE.Controllers
{
    public class UsersController : Controller
    {
        private readonly Agri_Energy_DBContext _context;

        // Constructor to initialize the database context
        public UsersController(Agri_Energy_DBContext context)
        {
            _context = context;
        }

        // GET: Users
        // Action to display the list of users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
        // Action to display the details of a specific user
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        // Action to display the create user form
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // Action to handle the creation of a new user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Username,PasswordHash,Email,FullName")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        // Action to display the edit user form
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // Action to handle the updating of an existing user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,PasswordHash,Email,FullName")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        // Action to display the delete user confirmation page
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        // Action to handle the deletion of a user
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Method to check if a user exists by ID
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        // GET: Users/Login
        // Action to display the login form
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Users/Login
        // Action to handle the user login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string Username, string PasswordHash)
        {
            var user = await _context.Users
                .Where(u => u.Username == Username && u.PasswordHash == PasswordHash)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Incorrect Details";
                return View();
            }
        }

        // GET: Users/Register
        // Action to display the registration form
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Users/Register
        // Action to handle the user registration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Username,PasswordHash,Email,FullName,ConfirmPassword")] User user)
        {
            if (ModelState.IsValid)
            {
                var existingUserByUsername = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
                var existingUserByEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

                if (existingUserByUsername != null)
                {
                    ModelState.AddModelError("Username", "Username is already taken.");
                    return View(user);
                }

                if (existingUserByEmail != null)
                {
                    ModelState.AddModelError("Email", "Email is already in use.");
                    return View(user);
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login", "Users");
            }

            return View(user);
        }

    }
}

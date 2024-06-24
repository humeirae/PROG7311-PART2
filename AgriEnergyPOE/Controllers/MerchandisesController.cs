using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgriEnergyPOE.Models;

namespace AgriEnergyPOE.Controllers
{
    [Authorize]
    public class MerchandisesController : Controller
    {
        private readonly Agri_Energy_DBContext _context;

        public MerchandisesController(Agri_Energy_DBContext context)
        {
            _context = context;
        }

        // GET: Merchandises
        // Action to display the list of merchandises
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole == "Employee")
            {
                // Get all products
                return View(await _context.Merchandise.ToListAsync());
            }
            else if (userRole == "Farmer")
            {
                // Get products for the logged-in farmer
                return View(await _context.Merchandise.Where(m => m.FarmerId == userId).ToListAsync());
            }

            return Unauthorized(); // Return unauthorized if user's role is not recognized
        }

        // GET: Merchandises/Details/5
        // Action to display the details of a specific merchandise
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var merchandise = await _context.Merchandise
                .FirstOrDefaultAsync(m => m.MerchandiseId == id);
            if (merchandise == null)
            {
                return NotFound();
            }

            return View(merchandise);
        }

        // GET: Merchandises/Create
        // Action to display the create merchandise form
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Merchandises/Create
        // Action to handle the creation of a new merchandise
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MerchandiseId,MerchandiseName,MerchandiseDescription,ManufactureDate,MerchandiseCost,ContactNumber")] Merchandise merchandise)
        {
            if (ModelState.IsValid)
            {
                _context.Add(merchandise);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(merchandise);
        }

        // GET: Merchandises/Edit/5
        // Action to display the edit merchandise form
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var merchandise = await _context.Merchandise.FindAsync(id);
            if (merchandise == null)
            {
                return NotFound();
            }
            return View(merchandise);
        }

        // POST: Merchandises/Edit/5
        // Action to handle the updating of an existing merchandise
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("MerchandiseId,MerchandiseName,MerchandiseDescription,ManufactureDate,MerchandiseCost,ContactNumber")] Merchandise merchandise)
        {
            if (id != merchandise.MerchandiseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(merchandise);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MerchandiseExists(merchandise.MerchandiseId))
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
            return View(merchandise);
        }

        // GET: Merchandises/Delete/5
        // Action to display the delete merchandise confirmation page
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var merchandise = await _context.Merchandise
                .FirstOrDefaultAsync(m => m.MerchandiseId == id);
            if (merchandise == null)
            {
                return NotFound();
            }

            return View(merchandise);
        }

        // POST: Merchandises/Delete/5
        // Action to handle the deletion of a merchandise
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var merchandise = await _context.Merchandise.FindAsync(id);
            if (merchandise != null)
            {
                _context.Merchandise.Remove(merchandise);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Method to check if a merchandise exists by ID
        private bool MerchandiseExists(int id)
        {
            return _context.Merchandise.Any(e => e.MerchandiseId == id);
        }

        // Search functionality
        // Action to handle the search for merchandises
        public async Task<IActionResult> Search(string SearchText)
        {
            return View("Index", await _context.Merchandise.Where(i => i.MerchandiseDescription.Contains(SearchText) || i.MerchandiseName.Contains(SearchText)).ToListAsync());
        }
    }
}

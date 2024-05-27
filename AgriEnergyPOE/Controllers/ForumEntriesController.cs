using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AgriEnergyPOE.Models;
using Microsoft.AspNetCore.Authorization;

namespace AgriEnergyPOE.Controllers
{
    [Authorize] // Ensures that only authorized users can access this controller
    public class ForumEntriesController : Controller
    {
        private readonly Agri_Energy_DBContext _context;

        // Constructor to initialize the database context
        public ForumEntriesController(Agri_Energy_DBContext context)
        {
            _context = context;
        }

        // GET: ForumEntries
        // Retrieves and displays a list of all forum entries
        public async Task<IActionResult> Index()
        {
            return View(await _context.ForumEntry.ToListAsync());
        }

        // Search Function
        // Searches forum entries based on the provided search text
        public async Task<IActionResult> Search(string SearchText)
        {
            return View("Index", await _context.ForumEntry
                .Where(i => i.EntryContent.Contains(SearchText) || i.EntrySubject.Contains(SearchText))
                .ToListAsync());
        }

        // GET: ForumEntries/Details/5
        // Displays the details of a specific forum entry based on its ID
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumEntry = await _context.ForumEntry
                .FirstOrDefaultAsync(m => m.EntryId == id);
            if (forumEntry == null)
            {
                return NotFound();
            }

            return View(forumEntry);
        }

        [Authorize]
        // GET: ForumEntry/Create
        // Displays the form for creating a new forum entry
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        // POST: ForumEntries/Create
        // Handles the form submission for creating a new forum entry
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EntryId,EntrySubject,EntryContent,PublishedDate")] ForumEntry forumEntry)
        {
            if (ModelState.IsValid)
            {
                _context.Add(forumEntry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(forumEntry);
        }

        // GET: ForumEntries/Edit/5
        // Displays the form for editing an existing forum entry
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumEntry = await _context.ForumEntry.FindAsync(id);
            if (forumEntry == null)
            {
                return NotFound();
            }
            return View(forumEntry);
        }

        [Authorize]
        // POST: ForumEntries/Edit/5
        // Handles the form submission for editing an existing forum entry
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EntryId,EntrySubject,EntryContent,PublishedDate")] ForumEntry forumEntry)
        {
            if (id != forumEntry.EntryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(forumEntry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ForumEntryExists(forumEntry.EntryId))
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
            return View(forumEntry);
        }

        // GET: ForumEntries/Delete/5
        // Displays the confirmation page for deleting a forum entry
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumEntry = await _context.ForumEntry
                .FirstOrDefaultAsync(m => m.EntryId == id);
            if (forumEntry == null)
            {
                return NotFound();
            }

            return View(forumEntry);
        }

        [Authorize]
        // POST: ForumEntries/Delete/5
        // Handles the form submission for deleting a forum entry
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var forumEntry = await _context.ForumEntry.FindAsync(id);
            if (forumEntry != null)
            {
                _context.ForumEntry.Remove(forumEntry);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Checks if a forum entry exists based on its ID
        private bool ForumEntryExists(int id)
        {
            return _context.ForumEntry.Any(e => e.EntryId == id);
        }
    }
}

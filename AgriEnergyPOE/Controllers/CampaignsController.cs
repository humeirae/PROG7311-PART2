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
    public class CampaignsController : Controller
    {
        private readonly Agri_Energy_DBContext _context;

        // Constructor to initialize the database context
        public CampaignsController(Agri_Energy_DBContext context)
        {
            _context = context;
        }

        // GET: Campaigns
        // Retrieves and displays a list of all campaigns
        public async Task<IActionResult> Index()
        {
            return View(await _context.Campaign.ToListAsync());
        }

        // Search Function
        // Searches for campaigns based on the provided search text
        public async Task<IActionResult> Search(string searchText)
        {
            return View("Index", await _context.Campaign
                .Where(c => c.CampaignSummary.Contains(searchText) || c.CampaignTitle.Contains(searchText))
                .ToListAsync());
        }

        // GET: Campaigns/Details/5
        // Displays the details of a specific campaign based on its ID
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var campaign = await _context.Campaign
                .FirstOrDefaultAsync(m => m.CampaignId == id);
            if (campaign == null)
            {
                return NotFound();
            }

            return View(campaign);
        }

        // GET: Campaigns/Create
        // Displays the form for creating a new campaign
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Campaigns/Create
        // Handles the form submission for creating a new campaign
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CampaignId,CampaignTitle,CampaignSummary,CoordinatorContact")] Campaign campaign)
        {
            if (ModelState.IsValid)
            {
                _context.Add(campaign);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(campaign);
        }

        // GET: Campaigns/Edit/5
        // Displays the form for editing an existing campaign
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var campaign = await _context.Campaign.FindAsync(id);
            if (campaign == null)
            {
                return NotFound();
            }
            return View(campaign);
        }

        // POST: Campaigns/Edit/5
        // Handles the form submission for editing an existing campaign
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CampaignId,CampaignTitle,CampaignSummary,CoordinatorContact")] Campaign campaign)
        {
            if (id != campaign.CampaignId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(campaign);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CampaignExists(campaign.CampaignId))
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
            return View(campaign);
        }

        // GET: Campaigns/Delete/5
        // Displays the confirmation page for deleting a campaign
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var campaign = await _context.Campaign
                .FirstOrDefaultAsync(m => m.CampaignId == id);
            if (campaign == null)
            {
                return NotFound();
            }

            return View(campaign);
        }

        // POST: Campaigns/Delete/5
        // Handles the form submission for deleting a campaign
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var campaign = await _context.Campaign.FindAsync(id);
            if (campaign != null)
            {
                _context.Campaign.Remove(campaign);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Checks if a campaign exists based on its ID
        private bool CampaignExists(int id)
        {
            return _context.Campaign.Any(e => e.CampaignId == id);
        }
    }
}

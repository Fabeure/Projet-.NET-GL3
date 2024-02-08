using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Expense_Tracker.Models;

namespace Expense_Tracker.Controllers
{
    public class MissionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MissionController(ApplicationDbContext context, UserManager<ApplicationUser> manager)
        {
            _context = context;
            _userManager = manager;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            bool isLoggedIn = (HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated;

            if (!isLoggedIn)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            var applicationDbContext = _context.Missions;
            var transactionCountPairs = new Dictionary<string, object>();
            var missions = _context.Missions;
            foreach ( var mission in missions)
            {
                transactionCountPairs[$"{mission.MissionId}"] = mission.Transactions.Count();
            }
            ViewBag.TransactionCountDict = transactionCountPairs;
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> getMission(int id)
        {
            bool isLoggedIn = (HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated;

            if (!isLoggedIn)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            var applicationDbContext = _context.Missions;
            var mission = _context.Missions.Find(id);
            return View(mission);
        }

        // GET: Transaction/AddOrEdit
        public IActionResult AddOrEdit(int id = 0)
        {
            bool isLoggedIn = (HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated;

            if (!isLoggedIn)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            PopulateMissions();
            if (id == 0)
                return View(new Mission());
            else
                return View(_context.Missions.Find(id));
        }

        // POST: Transaction/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("MissionId,Date,Name")] Mission mission)
        {
            bool isLoggedIn = (HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated;

            if (!isLoggedIn)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            if (ModelState.IsValid)
            {
                if (mission.MissionId == 0)
                {
                    mission.Owner = _userManager.GetUserAsync(User).Result;
                    _context.Add(mission);
                }
                else
                    _context.Update(mission);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateMissions();
            return View(mission);
        }

        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool isLoggedIn = (HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated;

            if (!isLoggedIn)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            if (_context.Transactions == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Transactions'  is null.");
            }
            var mission = await _context.Missions.FindAsync(id);
            if (mission != null)
            {
                _context.Missions.Remove(mission);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [NonAction]
        public void PopulateMissions()
        {
            var MissionCollection = _context.Missions.ToList();
            Mission DefaultMission = new Mission() { MissionId = 0, Name = "Choose a Mission" };
            MissionCollection.Insert(0, DefaultMission);
            ViewBag.Missions = MissionCollection;
        }
    }
}
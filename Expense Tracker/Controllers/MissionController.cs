using Microsoft.AspNetCore.Mvc;
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

        // GET: Mission
        public async Task<IActionResult> Index()
        {
            bool isLoggedIn = (HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated;

            if (!isLoggedIn)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            var currentUser = _userManager.GetUserAsync(User).Result;
            var applicationDbContext = _context.Missions.Where(m => m.Owner.Id == currentUser.Id);

            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> MissionDetails(int id)
        {
            bool isLoggedIn = (HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated;

            if (!isLoggedIn)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            Mission currentMission = _context.Missions.Find(id);

            List<Transaction> missionTransactions = _context.Transactions.Where(t => t.MissionId == currentMission.MissionId).ToList();
            ViewBag.transactions = missionTransactions;

            return View(currentMission);
        }

        // GET: Mission/AddOrEdit
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

        // POST: Mission/AddOrEdit
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

        // POST: Mission/Delete
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
            Mission mission = await _context.Missions.FindAsync(id);
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
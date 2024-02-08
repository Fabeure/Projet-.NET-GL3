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
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TransactionController(ApplicationDbContext context, UserManager<ApplicationUser> manager)
        {
            _context = context;
            _userManager = manager;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            bool isLoggedIn= (HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated;

            if (!isLoggedIn){
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            var applicationDbContext = _context.Transactions.Include(t => t.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Transaction/AddOrEdit
        public IActionResult AddOrEdit(int id = 0)
        {
            bool isLoggedIn= (HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated;

            if (!isLoggedIn){
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            PopulateCategories();
            PopulateMissions();
            if (id == 0)
                return View(new Transaction());
            else
                return View(_context.Transactions.Find(id));
        }

        // POST: Transaction/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("TransactionId,CategoryId,Amount,Note,Date,MissionId")] Transaction transaction)
        {
            bool isLoggedIn= (HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated;

            if (!isLoggedIn){
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            if (transaction.TransactionId == 0)
            {
                var currentUser = _userManager.GetUserAsync(User).Result;
                transaction.ownerId = currentUser.Id;
                var transactionMission = _context.Missions.Find(transaction.MissionId);
                if (transactionMission != null)
                {
                    transaction.MissionId = transactionMission.MissionId;
                }
                _context.Add(transaction);
            }
            else
                _context.Update(transaction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool isLoggedIn= (HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated;

            if (!isLoggedIn){
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            if (_context.Transactions == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Transactions'  is null.");
            }
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [NonAction]
        public void PopulateCategories()
        {

            var CategoryCollection = _context.Categories.ToList();
            Category DefaultCategory = new Category() { CategoryId = 0, Title = "Choose a Category" };
            CategoryCollection.Insert(0, DefaultCategory);
            ViewBag.Categories = CategoryCollection;
        }
        public void PopulateMissions()
        {
            var MissionCollection = _context.Missions.ToList();
            Mission DefaultMission = new Mission() { MissionId = 0, Name = "Choose a Mission" };
            MissionCollection.Insert(0, DefaultMission);
            ViewBag.Missions = MissionCollection;
        }
    }
}

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

        public TransactionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            bool isLoggedIn= (HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated;

            if (!isLoggedIn){
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            var userId = _userManager.GetUserId(User);
            var userTransactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.ownerId == userId)
                .ToListAsync();

            return View(userTransactions);
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
            {
                var transaction = _context.Transactions.FirstOrDefault(t => t.TransactionId == id && t.ownerId == _userManager.GetUserId(User));
                if (transaction == null)
                {
                    return NotFound();
                }
                return View(_context.Transactions.Find(id));
            }
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
            var Userid = _userManager.GetUserId(User);
            var budget = await _context.Budgets.FirstOrDefaultAsync(b =>
        b.CategoryId == transaction.CategoryId && b.UserId == Userid);

            var category = await _context.Categories.FindAsync(transaction.CategoryId);

            if (transaction.TransactionId == 0)
            {   
                transaction.ownerId = Userid;

                if (category.Type == "Expense")
                {
                    // Deduct the transaction amount from the budget
                    budget.Amount -= transaction.Amount;

                    // Update the budget in the database
                    _context.Budgets.Update(budget);
                }
                if (budget.Amount <= 0)
                {
                    if ((_context.UserBudgetAlerts.FirstOrDefaultAsync(b =>
        b.CategoryTitle == category.Title) == null))
                    {
                        var budgetAlert = new UserBudgetAlert
                        {
                            CategoryTitle = category.Title,
                            UserId = budget.UserId
                        };

                        _context.UserBudgetAlerts.Add(budgetAlert);
                    }
                }   
                var transactionMission = _context.Missions.Find(transaction.MissionId);
                if (transactionMission != null)
                {
                    transaction.MissionId = transactionMission.MissionId;
                }
                _context.Add(transaction);
            }
            else
            {
                if (transaction.Amount != null)
                {
                    var oldtransaction = await _context.Transactions.FindAsync(transaction.TransactionId);
                    budget.Amount += oldtransaction.Amount;
                    budget.Amount -= transaction.Amount;

                    _context.Budgets.Update(budget);
                }
                _context.Update(transaction);

                if (budget.Amount <= 0)
                {
                    if ((_context.UserBudgetAlerts.FirstOrDefaultAsync(b =>
        b.CategoryTitle == category.Title) == null))
                    {
                        var budgetAlert = new UserBudgetAlert
                        {
                            CategoryTitle = category.Title,
                            UserId = budget.UserId
                        };

                        _context.UserBudgetAlerts.Add(budgetAlert);
                    }
                }

            }
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

        [HttpGet]
        public IActionResult FetchBudgetAlerts()
        {
            // Get the current user's ID
            var userId = _userManager.GetUserId(User);

            // Fetch the user's budget alerts from the database
            var userBudgetAlerts = _context.UserBudgetAlerts
                .Where(uba => uba.UserId == userId)
                .ToList();

            // Project the budget alerts to a simpler model for JSON serialization
            var alerts = userBudgetAlerts.Select(uba => new
            {
                CategoryTitle = uba.CategoryTitle

            }).ToList();

            return Json(alerts);
        }


        [NonAction]
        public void PopulateCategories()
        {

            var CategoryCollection = _context.Categories.ToList();
            Category DefaultCategory = new Category() { CategoryId = 0, Title = "Choose a Category" };
            CategoryCollection.Insert(0, DefaultCategory);
            ViewBag.Categories = CategoryCollection;
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

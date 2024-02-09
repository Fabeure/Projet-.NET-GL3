using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Expense_Tracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Expense_Tracker.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CategoryController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
    

    // GET: Category
        public async Task<IActionResult> Index()
        {
            bool isLoggedIn= (HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated;

            if (!isLoggedIn){
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            var categories = _context.Categories.ToList();
            var categorywithbudget = new List<CategoryWithBudget>();

            foreach (var category in categories)
            {
                var categoryViewModel = new CategoryWithBudget
                {
                    CategoryId = category.CategoryId,
                    Title = category.Title,
                    Icon = category.Icon,
                    Type = category.Type,
                    // Populate data from other model
                    RemainingBudget = (await _context.Budgets.FirstOrDefaultAsync(b =>
    b.CategoryId == category.CategoryId && b.UserId == _userManager.GetUserId(User)))?.Amount ?? 0
            };

                categorywithbudget.Add(categoryViewModel);
            }

            return View(categorywithbudget);
        }


        // GET: Category/AddOrEdit
        public IActionResult AddOrEdit(int id = 0)
        {
            bool isLoggedIn= (HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated;

            if (!isLoggedIn){
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            if (id == 0)
                return View(new Category());
            else
                return View(_context.Categories.Find(id));

        }

        // POST: Category/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("CategoryId,Title,Icon,Type")] Category category, Decimal budgetamount)
        {
            bool isLoggedIn= (HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated;

            if (!isLoggedIn){
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            if (ModelState.IsValid)
            {

                var employees = _userManager.Users.ToList();

                if (category.CategoryId == 0)
                {
                    _context.Add(category);
                    await _context.SaveChangesAsync();

                    if (category.Type == "Expense" && budgetamount > 0)
                    {


                        int generatedCategoryId = category.CategoryId;

                        foreach (var employee in employees)
                        {
                            var budget = new Budget
                            {
                                CategoryId = generatedCategoryId,
                                Amount = budgetamount,
                                UserId = employee.Id // Associate the budget with the employee
                            };


                            _context.Budgets.Add(budget);
                        }
                    }
                }
                else
                {
                    if (budgetamount > 0)
                    {
                        var budgets = _context.Budgets.Where(b => b.CategoryId == category.CategoryId);
                        foreach (var budget in budgets)
                        {
                            budget.Amount = budgetamount;
                            _context.Update(budget);
                        }
                    }

                    _context.Update(category);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool isLoggedIn= (HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated;

            if (!isLoggedIn){
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}

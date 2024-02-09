using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using System.Runtime.CompilerServices;

namespace Expense_Tracker.Models
{
    public class ApplicationUser: IdentityUser
    {
        public byte[]? profilePicture { get; set; }

        public List<UserBudgetAlert> BudgetAlerts { get; set; }
        public List<Transaction>? transactions { get; set; }
    }
}

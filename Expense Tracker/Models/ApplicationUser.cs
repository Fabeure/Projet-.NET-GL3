using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;

namespace Expense_Tracker.Models
{
    public class ApplicationUser: IdentityUser
    {
        public byte[]? profilePicture { get; set; }
        public List<Transaction>? transactions { get; set; }
    }
}

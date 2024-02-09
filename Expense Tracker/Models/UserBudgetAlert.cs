namespace Expense_Tracker.Models
{
    public class UserBudgetAlert
    {
        public int Id { get; set; } // Primary key
        public string UserId { get; set; } // Foreign key to ApplicationUser
        public string CategoryTitle { get; set; } // The category title for the budget alert

        public ApplicationUser User { get; set; } // Navigation property to ApplicationUser
    }
}

using System.ComponentModel.DataAnnotations;

namespace Expense_Tracker.Models
{
    public class Budget
    {

        [Key]
        public int BudgetId { get; set; }

        [Required]
        public string UserId { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }

        /*public virtual ApplicationUser User { get; set; }
        public virtual Category Category { get; set; }*/
    }
}

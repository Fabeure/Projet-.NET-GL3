using System.ComponentModel.DataAnnotations;

namespace Expense_Tracker.Models
{
    public class Mission
    {
        [Key]
        public int MissionId { get; set; }

        public int UserId { get; set; }
        public ApplicationUser Owner { get; set; }
        public ICollection<Transaction> Transactions { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

    }
}

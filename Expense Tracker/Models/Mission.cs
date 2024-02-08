using System.ComponentModel.DataAnnotations;

namespace Expense_Tracker.Models
{
    public class Mission
    {
        [Key]
        public int MissionId { get; set; }

        public ApplicationUser? Owner { get; set; } = null;
        public ICollection<Transaction>? Transactions { get; set; } = new List<Transaction>();
        public DateTime Date { get; set; } = DateTime.Now;

        public string Name { get; set; }

    }
}

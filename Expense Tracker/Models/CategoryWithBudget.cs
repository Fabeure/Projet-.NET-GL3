using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Expense_Tracker.Models
{
    public class CategoryWithBudget
    {
        
        public int CategoryId { get; set; }

        public string Title { get; set; }

       
        public string Icon { get; set; } = "";

      
        public string Type { get; set; } = "Expense";

        public decimal RemainingBudget { get; set; } 

        public string? TitleWithIcon
        {
            get
            {
                return this.Icon + " " + this.Title;
            }
        }

    }
}


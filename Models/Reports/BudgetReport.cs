using MoMoney.Interfaces;

namespace MoMoney.Models
{
    /// <summary>
    /// Represents a budget report for a category, including utilization and limits.
    /// </summary>
    public class BudgetReport : IBudgetLike
    {
        /// <summary>
        /// Gets or sets the category name for this budget.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the progress as a fractional value between 0.0 and 1.0.
        /// </summary>
        public double Progress { get; set; }

        /// <summary>
        /// Gets or sets the amount spent in this category.
        /// </summary>
        public decimal Spent { get; set; }

        /// <summary>
        /// Gets or sets the spending limit for this category.
        /// </summary>
        public decimal Limit { get; set; }

        /// <summary>
        /// Gets a string showing the spent amount versus the limit in currency format.
        /// </summary>
        public string SpentOfLimit => $"{Spent:C} of {Limit:C}";

        /// <summary>
        /// Gets the remaining budget as a formatted string. Shows positive balance if under budget, or overage if exceeded.
        /// </summary>
        public string Remaining => Spent <= Limit ? $"{(Limit - Spent):C} left" : $"{(Spent - Limit):C} over";

        /// <summary>
        /// Gets a value indicating whether this budget has been exceeded.
        /// </summary>
        public bool IsOverBudget => Spent > Limit;
    }
}

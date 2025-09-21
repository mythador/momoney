using SQLite;

namespace MoMoney.Models
{
    /// <summary>
    /// Represents a budget entry including category, amount, and metadata.
    /// </summary>
    public class Budget
    {
        /// <summary>
        /// Gets or sets the unique identifier for the budget.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the category name associated with this budget.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the budgeted amount for the category.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the time period of the budget (default is "Monthly").
        /// </summary>
        public string Period { get; set; } = "Monthly";

        /// <summary>
        /// Gets or sets the timestamp when the budget was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

namespace MoMoney.Models
{
    /// <summary>
    /// Represents a monthly financial summary of income, expenses, and net total.
    /// </summary>
    public class MonthlySummary
    {
        /// <summary>
        /// Gets or sets the month name and year (e.g., "January 2025").
        /// </summary>
        public string Month { get; set; }

        /// <summary>
        /// Gets or sets the total income for the month.
        /// </summary>
        public decimal Income { get; set; }

        /// <summary>
        /// Gets or sets the total expenses for the month.
        /// </summary>
        public decimal Expenses { get; set; }

        /// <summary>
        /// Gets the net balance for the month, calculated as Income minus Expenses.
        /// </summary>
        public decimal Net => Income - Expenses;
    }
}
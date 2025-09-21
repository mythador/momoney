using SQLite;

namespace MoMoney.Models
{
    /// <summary>
    /// Represents a financial transaction, such as an income or expense entry.
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Gets or sets the unique identifier for the transaction.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the amount of the transaction.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the category associated with the transaction.
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date when the transaction occurred.
        /// </summary>
        public DateTime Date { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets any notes or description related to the transaction.
        /// </summary>
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this transaction is income.
        /// True indicates income; false indicates expense.
        /// </summary>
        public bool IsIncome { get; set; } // true = income, false = expense
    }
}

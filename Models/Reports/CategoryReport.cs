namespace MoMoney.Models
{
    /// <summary>
    /// Represents a spending report for a specific category.
    /// </summary>
    public class CategoryReport
    {
        /// <summary>
        /// Gets or sets the category name (e.g., "Food", "Rent").
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the total amount spent in this category.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the color associated with this category, represented as a hex string.
        /// </summary>
        public string ColorHex { get; set; }
    }
}

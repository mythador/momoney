using SQLite;

namespace MoMoney.Models
{
    /// <summary>
    /// Represents a category used to organize budgets or transactions.
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Gets or sets the unique identifier for the category.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the category.
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}

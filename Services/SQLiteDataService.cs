using SQLite;
using MoMoney.Models;

namespace MoMoney.Services
{
    /// <summary>
    /// Provides a SQLite-based implementation of the <see cref="IDataService"/> interface.
    /// Handles CRUD operations for transactions, budgets, and categories.
    /// </summary>
    public class SQLiteDataService : IDataService
    {
        private readonly SQLiteAsyncConnection _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteDataService"/> class.
        /// Ensures required tables are created.
        /// </summary>
        /// <param name="dbPath">The file path of the SQLite database.</param>
        public SQLiteDataService(string dbPath)
        {
            _db = new SQLiteAsyncConnection(dbPath);

            // Ensure tables are created up front
            _db.CreateTableAsync<Transaction>().Wait();
            _db.CreateTableAsync<Budget>().Wait();
            _db.CreateTableAsync<Category>().Wait();
        }


        // ==================================================================
        // Transactions
        // ==================================================================

        /// <summary>
        /// Gets all transactions asynchronously, ordered by date descending.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The task result contains a list of transactions.</returns>
        public Task<List<Transaction>> GetTransactionsAsync() =>
            _db.Table<Transaction>()
               .OrderByDescending(t => t.Date)
               .ToListAsync();

        /// <summary>
        /// Adds a new transaction asynchronously.
        /// </summary>
        /// <param name="t">The transaction to add.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the number of rows affected.</returns>
        public Task<int> AddTransactionAsync(Transaction t) =>
            _db.InsertAsync(t);

        /// <summary>
        /// Updates an existing transaction asynchronously.
        /// </summary>
        /// <param name="t">The transaction to update.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the number of rows affected.</returns>
        public Task<int> UpdateTransactionAsync(Transaction t) =>
            _db.UpdateAsync(t);

        /// <summary>
        /// Deletes a transaction asynchronously.
        /// </summary>
        /// <param name="t">The transaction to delete.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the number of rows affected.</returns>
        public Task<int> DeleteTransactionAsync(Transaction t) =>
            _db.DeleteAsync(t);


        // ==================================================================
        // Budgets
        // ==================================================================

        /// <summary>
        /// Gets all budgets asynchronously, ordered by category.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The task result contains a list of budgets.</returns>
        public Task<List<Budget>> GetBudgetsAsync() =>
            _db.Table<Budget>()
               .OrderBy(b => b.Category)
               .ToListAsync();

        /// <summary>
        /// Adds a new budget asynchronously.
        /// </summary>
        /// <param name="b">The budget to add.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the number of rows affected.</returns>
        public Task<int> AddBudgetAsync(Budget b) =>
            _db.InsertAsync(b);

        /// <summary>
        /// Updates an existing budget asynchronously.
        /// </summary>
        /// <param name="b">The budget to update.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the number of rows affected.</returns>
        public Task<int> UpdateBudgetAsync(Budget b) =>
            _db.UpdateAsync(b);

        /// <summary>
        /// Deletes a budget asynchronously.
        /// </summary>
        /// <param name="b">The budget to delete.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the number of rows affected.</returns>
        public Task<int> DeleteBudgetAsync(Budget b) =>
            _db.DeleteAsync(b);

        /// <summary>
        /// Seeds default budgets if none exist in the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SeedDefaultBudgetsAsync()
        {
            var existing = await GetBudgetsAsync();
            if (existing.Count == 0)
            {
                var defaults = new[]
                {
                    new Budget { Category = "Groceries", Amount = 500, Period = "Monthly" },
                    new Budget { Category = "Utilities", Amount = 200, Period = "Monthly" },
                    new Budget { Category = "Rent", Amount = 1200, Period = "Monthly" },
                    new Budget { Category = "Entertainment", Amount = 150, Period = "Monthly" },
                    new Budget { Category = "Transportation", Amount = 100, Period = "Monthly" },
                    new Budget { Category = "Miscellaneous", Amount = 100, Period = "Monthly" }
                };

                foreach (var b in defaults)
                    await AddBudgetAsync(b);
            }
        }

        // ==================================================================
        // Categories
        // ==================================================================

        /// <summary>
        /// Gets all categories asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The task result contains a list of categories.</returns>
        public Task<List<Category>> GetCategoriesAsync() =>
            _db.Table<Category>().ToListAsync();

        /// <summary>
        /// Adds a new category asynchronously.
        /// </summary>
        /// <param name="category">The category to add.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the number of rows affected.</returns>
        public Task<int> AddCategoryAsync(Category category) =>
            _db.InsertAsync(category);

        /// <summary>
        /// Seeds default categories if none exist in the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SeedDefaultCategoriesAsync()
        {
            var existing = await GetCategoriesAsync();
            if (existing.Count == 0)
            {
                var defaults = new[]
                {
                    new Category { Name = "Groceries" },
                    new Category { Name = "Utilities" },
                    new Category { Name = "Rent" },
                    new Category { Name = "Salary" },
                    new Category { Name = "Entertainment" },
                    new Category { Name = "Transportation" },
                    new Category { Name = "Miscellaneous" }
                };

                foreach (var c in defaults)
                    await AddCategoryAsync(c);
            }
        }
    }
}

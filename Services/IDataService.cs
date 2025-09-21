using MoMoney.Models;

namespace MoMoney.Services
{
    /// <summary>
    /// Defines methods for accessing and managing transactions, budgets, and categories.
    /// </summary>
    public interface IDataService
    {
        // Transactions
        Task<List<Transaction>> GetTransactionsAsync();
        Task<int> AddTransactionAsync(Transaction t);
        Task<int> UpdateTransactionAsync(Transaction t);
        Task<int> DeleteTransactionAsync(Transaction t);

        // Budgets
        Task<List<Budget>> GetBudgetsAsync();
        Task<int> AddBudgetAsync(Budget b);
        Task<int> UpdateBudgetAsync(Budget b);
        Task<int> DeleteBudgetAsync(Budget b);

        // Categories
        Task<List<Category>> GetCategoriesAsync();
        Task<int> AddCategoryAsync(Category category);

        // Seeding methods when none exist --- MAY NEED TO SEE TRANSACTIONS, CLUNKY FOR A LIVE APP???
        Task SeedDefaultCategoriesAsync();
        Task SeedDefaultBudgetsAsync();

    }
}

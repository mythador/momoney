using MoMoney.Models;
using MoMoney.Interfaces;
using MoMoney.Services;

namespace MoMoney.ViewModels
{
    /// <summary>
    /// Represents a budget item for the UI layer, including calculated spending,
    /// progress, and budget overrun state.
    /// </summary>
    public class BudgetItemViewModel : IBudgetLike
    {
        /// <summary>
        /// Gets or sets the unique identifier of the budget.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the category name of the budget.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the budgeted amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the budget period (e.g., monthly).
        /// </summary>
        public string Period { get; set; }

        // UI Properties

        /// <summary>
        /// Gets or sets the amount spent in this budget category.
        /// </summary>
        public decimal Spent { get; set; }

        /// <summary>
        /// Gets the spending progress as a fractional value between 0 and 1.
        /// </summary>
        public double Progress => Amount <= 0 ? 0 : Math.Clamp((double)(Spent / Amount), 0, 1);

        /// <summary>
        /// Gets a text representation of the spent amount versus the budget amount.
        /// </summary>
        public string ProgressText => $"Spent: {Spent:C} of {Amount:C}";

        /// <summary>
        /// Gets a value indicating whether the budget has been exceeded.
        /// </summary>
        public bool IsOverBudget => Spent > Amount;

        /// <summary>
        /// Initializes a new instance of the <see cref="BudgetItemViewModel"/> class
        /// by calculating spending from a collection of transactions.
        /// </summary>
        /// <param name="budget">The budget model.</param>
        /// <param name="transactions">The collection of transactions used to calculate spending.</param>
        /// <exception cref="Exception">Thrown if building the view model fails.</exception>
        public BudgetItemViewModel(Budget budget, IEnumerable<Transaction> transactions)
        {
            try
            {
                Id = budget.Id;
                Category = budget.Category;
                Amount = budget.Amount;
                Period = budget.Period;

                // Sum all expenses (ignore incomes) in the same category
                Spent = transactions
                    .Where(t => t.Category == budget.Category && !t.IsIncome)
                    .Sum(t => t.Amount);
            }
            catch (Exception ex)
            {
                LoggerService.LogError<BudgetItemViewModel>(ex,
                    $"Failed to build BudgetItemViewModel for category '{budget?.Category ?? "UNKNOWN"}'");
                throw;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BudgetItemViewModel"/> class
        /// without calculating transactions, defaulting spent to zero.
        /// </summary>
        /// <param name="budget">The budget model.</param>
        public BudgetItemViewModel(Budget budget)
        {
            Id = budget.Id;
            Category = budget.Category;
            Amount = budget.Amount;
            Period = budget.Period;
            Spent = 0;

            LoggerService.LogInfo<BudgetItemViewModel>(
            $"Created (no transactions) for category '{Category}', Amount={Amount:C}");
        }
    }
}

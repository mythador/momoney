// Used for the Reports and Budgets view models.
// This allows code sharing between the two view models for display of colors on progress bars, etc.

namespace MoMoney.Interfaces
{
    /// <summary>
    /// Defines common properties for objects that behave like a budget,
    /// including budget status and progress tracking.
    /// </summary>
    public interface IBudgetLike
    {
        /// <summary>
        /// Gets a value indicating whether the budget has been exceeded.
        /// </summary>
        bool IsOverBudget { get; }

        /// <summary>
        /// Gets the progress of the budget as a fractional value
        /// between 0.0 and 1.0.
        /// </summary>
        double Progress { get; }
    }
}
using MoMoney.Interfaces;

namespace MoMoney.Converters
{
    /// <summary>
    /// Converts a budget progress value into a <see cref="Color"/> for UI display.
    /// Red when over budget, gray when at 100%, accent color when available, green otherwise.
    /// </summary>
    public class ProgressToColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts a budget-like object into a <see cref="Color"/> representing its progress state.
        /// </summary>
        /// <param name="value">The source value (expected to implement <see cref="IBudgetLike"/>).</param>
        /// <param name="targetType">The target type of the binding (not used).</param>
        /// <param name="parameter">Optional converter parameter (not used).</param>
        /// <param name="culture">The culture information (not used).</param>
        /// <returns>
        /// <see cref="Colors.Red"/> if over budget,
        /// <see cref="Colors.Gray"/> if progress is ~100%,
        /// the accent color from <c>Application.Current.Resources["BudgetProgressBar"]</c> if available,
        /// <see cref="Colors.Green"/> otherwise,
        /// or <see cref="Colors.Gray"/> if the value is not budget-like.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value is IBudgetLike budget)
            {
                if (budget.IsOverBudget)
                    return Colors.Red;
                if (Math.Abs(budget.Progress - 1) < 0.001)
                    return Colors.Gray;

                // ✅ Same accent color for both Budgets & Reports
                if (Application.Current.Resources.TryGetValue("BudgetProgressBar", out var accent))
                    return (Color)accent;

                return Colors.Green;
            }

            return Colors.Gray; // fallback
        }

        /// <summary>
        /// Not implemented — conversion from <see cref="Color"/> to <see cref="IBudgetLike"/> is not supported.
        /// </summary>
        /// <param name="value">The source value (expected to be a <see cref="Color"/>).</param>
        /// <param name="targetType">The target type of the binding.</param>
        /// <param name="parameter">Optional converter parameter (not used).</param>
        /// <param name="culture">The culture information (not used).</param>
        /// <returns>This method does not return a value.</returns>
        /// <exception cref="NotImplementedException">Always thrown when called.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}



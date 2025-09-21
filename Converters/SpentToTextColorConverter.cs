using System.Globalization;

namespace MoMoney.Converters
{
    /// <summary>
    /// Converts spent and budgeted amounts into a <see cref="Color"/> for UI display.
    /// Red when spent exceeds the budgeted amount, gray otherwise.
    /// </summary>
    public class SpentToTextColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts spent and budgeted values into a <see cref="Color"/>.
        /// </summary>
        /// <param name="value">The spent amount (expected to be a <see cref="decimal"/>).</param>
        /// <param name="targetType">The target type of the binding (not used).</param>
        /// <param name="parameter">The budgeted amount (expected to be a <see cref="decimal"/>).</param>
        /// <param name="culture">The culture information (not used).</param>
        /// <returns>
        /// <see cref="Colors.Red"/> if spent is greater than the budgeted amount,
        /// otherwise <see cref="Colors.Gray"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal spent && parameter is decimal amount)
            {
                return spent > amount ? Colors.Red : Colors.Gray;
            }
            return Colors.Gray;
        }

        /// <summary>
        /// Not implemented — conversion from <see cref="Color"/> back to spent or budgeted values is not supported.
        /// </summary>
        /// <param name="value">The source value (expected to be a <see cref="Color"/>).</param>
        /// <param name="targetType">The target type of the binding.</param>
        /// <param name="parameter">Optional converter parameter (not used).</param>
        /// <param name="culture">The culture information (not used).</param>
        /// <returns>This method does not return a value.</returns>
        /// <exception cref="NotImplementedException">Always thrown when called.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

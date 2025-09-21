using System.Globalization;

namespace MoMoney.Converters
{
    /// <summary>
    /// Converts a remaining budget text string into a <see cref="Color"/> for UI display.
    /// Green when there is budget left, red when over budget, gray for other text,
    /// and black when the input is not a string.
    /// </summary>
    public class RemainingToColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts a remaining budget text string into a <see cref="Color"/>.
        /// </summary>
        /// <param name="value">The source value (expected to be a string).</param>
        /// <param name="targetType">The target type of the binding (not used).</param>
        /// <param name="parameter">Optional converter parameter (not used).</param>
        /// <param name="culture">The culture information (not used).</param>
        /// <returns>
        /// <see cref="Colors.Green"/> if the text contains "left",
        /// <see cref="Colors.Red"/> if the text contains "over",
        /// <see cref="Colors.Gray"/> otherwise,
        /// or <see cref="Colors.Black"/> if the value is not a string.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string remainingText)
            {
                if (remainingText.Contains("left", StringComparison.OrdinalIgnoreCase))
                    return Colors.Green;
                if (remainingText.Contains("over", StringComparison.OrdinalIgnoreCase))
                    return Colors.Red;
                return Colors.Gray;
            }

            return Colors.Black;
        }

        /// <summary>
        /// Not implemented — conversion from <see cref="Color"/> to a budget text string is not supported.
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

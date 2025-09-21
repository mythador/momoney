using System.Globalization;

namespace MoMoney.Converters
{
    /// <summary>
    /// Converts a net decimal value into a <see cref="Color"/> for UI display.
    /// Positive values are green, negative values are red, zero is gray,
    /// and non-decimal inputs default to black.
    /// </summary>
    public class NetToColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts a net value into a <see cref="Color"/>.
        /// </summary>
        /// <param name="value">The source value (expected to be a <see cref="decimal"/>).</param>
        /// <param name="targetType">The target type of the binding (not used).</param>
        /// <param name="parameter">Optional converter parameter (not used).</param>
        /// <param name="culture">The culture information (not used).</param>
        /// <returns>
        /// <see cref="Colors.Green"/> if the value is positive,
        /// <see cref="Colors.Red"/> if negative,
        /// <see cref="Colors.Gray"/> if zero,
        /// or <see cref="Colors.Black"/> if the value is not a <see cref="decimal"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal net)
            {
                if (net > 0) return Colors.Green;
                if (net < 0) return Colors.Red;
                return Colors.Gray;
            }
            return Colors.Black;
        }

        /// <summary>
        /// Not implemented — conversion from <see cref="Color"/> to <see cref="decimal"/> is not supported.
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

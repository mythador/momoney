using System.Globalization;

namespace MoMoney.Converters
{
    /// <summary>
    /// Converts between currency values and strings for data binding.
    /// Formats values to currency strings for display, and parses them back
    /// into raw numeric values when editing.
    /// </summary>
    public class CurrencyConverter : IValueConverter
    {
        /// <summary>
        /// Converts a bound value (typically a string) into a formatted currency string.
        /// </summary>
        /// <param name="value">The source value being passed in (expected to be a string).</param>
        /// <param name="targetType">The target type of the binding (not used).</param>
        /// <param name="parameter">Optional converter parameter (not used).</param>
        /// <param name="culture">The culture to use for formatting.</param>
        /// <returns>
        /// A formatted currency string if parsing succeeds, or <c>null</c> if parsing fails
        /// or the input is empty.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                if (string.IsNullOrWhiteSpace(str))
                    return null; // lets placeholder show
                if (decimal.TryParse(str, NumberStyles.Currency, culture, out var parsed))
                    return parsed.ToString("C", culture);
            }
            return null;
        }

        /// <summary>
        /// Converts a currency string back into a plain string representation of the numeric value.
        /// </summary>
        /// <param name="value">The bound value being passed in (expected to be a string).</param>
        /// <param name="targetType">The target type of the binding (not used).</param>
        /// <param name="parameter">Optional converter parameter (not used).</param>
        /// <param name="culture">The culture to use for parsing.</param>
        /// <returns>
        /// A string containing the numeric value if parsing succeeds,
        /// or <see cref="string.Empty"/> if parsing fails.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && decimal.TryParse(str, NumberStyles.Currency, culture, out var result))
                return result.ToString(culture);
            return string.Empty;
        }
    }
}

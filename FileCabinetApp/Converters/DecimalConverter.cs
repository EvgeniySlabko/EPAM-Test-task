using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Convertor for decimal type.
    /// </summary>
    public class DecimalConverter : IConverter<decimal>
    {
        /// <summary>
        /// Converts a string to a value of decimal type.
        /// </summary>
        /// <param name="inputString">Given string.</param>
        /// <returns>Result tupple. bool value - validation result. string value - message string. Decimal value - result of conversion.</returns>
        public Tuple<bool, string, decimal> Convert(string inputString)
        {
            bool successfulConvert = true;
            string message = "Succesful convert";
            decimal value = 0;
            try
            {
                value = decimal.Parse(inputString, CultureInfo.CurrentCulture);
            }
            catch (ArgumentException ex)
            {
                message = ex.Message;
                successfulConvert = false;
            }
            catch (OverflowException ex)
            {
                message = ex.Message;
                successfulConvert = false;
            }
            catch (FormatException ex)
            {
                message = ex.Message;
                successfulConvert = false;
            }

            return new (successfulConvert, message, value);
        }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <returns>Gets delegate for conversion method.</returns>
        public Func<string, Tuple<bool, string, decimal>> GetDelegate()
        {
            return this.Convert;
        }
    }
}

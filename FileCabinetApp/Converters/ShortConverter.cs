using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Short conversion class.
    /// </summary>
    public class ShortConverter : IConverter<short>
    {
        /// <summary>
        /// Converts a string to a value of short type.
        /// </summary>
        /// <param name="inputString">Given string.</param>
        /// <returns>Result tupple. bool value - validation result. string value - message string. Short value - result of conversion.</returns>
        public Tuple<bool, string, short> Convert(string inputString)
        {
            bool successfulConvert = true;
            string message = "Succesful convert";
            short value = 0;
            try
            {
                value = short.Parse(inputString, CultureInfo.CurrentCulture);
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
        public Func<string, Tuple<bool, string, short>> GetDelegate()
        {
            return this.Convert;
        }
    }
}

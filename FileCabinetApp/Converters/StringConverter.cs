using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// String conversion class.
    /// </summary>
    public class StringConverter : IConverter<string>
    {
        /// <summary>
        /// Plug for string conversion method.
        /// </summary>
        /// <param name="inputString">Given string.</param>
        /// <returns>Result tupple. bool value - validation result. string value - message string. string value - result of conversion.</returns>
        public Tuple<bool, string, string> Convert(string inputString)
        {
            bool successfulConvert = true;
            string message = "Succesful convert";
            string value = string.Empty;
            if (inputString is null)
            {
                message = "String is null.";
                successfulConvert = false;
            }
            else
            {
                value = inputString;
            }

            return new (successfulConvert, message, value);
        }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <returns>Gets delegate for conversion method.</returns>
        public Func<string, Tuple<bool, string, string>> GetDelegate()
        {
            return this.Convert;
        }
    }
}

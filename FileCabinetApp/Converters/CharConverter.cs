using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Char conversion class.
    /// </summary>
    public class CharConverter : IConverter<char>
    {
        /// <summary>
        /// converts a string to a value of char type.
        /// </summary>
        /// <param name="inputString">Given string.</param>
        /// <returns>Result tupple. bool value - validation result. string value - message string. T value - result of conversion.</returns>
        public Tuple<bool, string, char> Convert(string inputString)
        {
            bool successfulConvert = true;
            string message = "Succesful convert";
            char value = default;
            try
            {
                value = char.Parse(inputString);
            }
            catch (ArgumentNullException ex)
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
    }
}

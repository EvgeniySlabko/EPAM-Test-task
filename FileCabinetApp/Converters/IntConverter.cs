using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Converters
{
    /// <summary>
    /// Int conversion class.
    /// </summary>
    public class IntConverter : IConverter<int>
    {
        /// <summary>
        /// converts a string to a value of char type.
        /// </summary>
        /// <param name="inputString">Given string.</param>
        /// <returns>Result tupple. bool value - validation result. string value - message string. T value - result of conversion.</returns>
        public Tuple<bool, string, int> Convert(string inputString)
        {
            bool successfulConvert = true;
            string message = "Succesful convert";
            int value = 0;
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

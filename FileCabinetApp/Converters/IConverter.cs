using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Interface for all validators.
    /// </summary>
    /// <typeparam name="T">Validation variable type.</typeparam>
    public interface IConverter<T>
    {
        /// <summary>
        /// converts a string to a value of T type.
        /// </summary>
        /// <param name="inputString">Given string.</param>
        /// <returns>Result tupple. bool value - validation result. string value - message string. T value - result of convertation.</returns>
        public Tuple<bool, string, T> Convert(string inputString);
    }

    /// <summary>
    /// Validator for int type.
    /// </summary>
    public class IntConverter : IConverter<int>
    {
        /// <inheritdoc/>
        public Tuple<bool, string, int> Convert(string input)
        {
            bool successfulConvert = true;
            string message = "Succesful convert";
            int value = 0;
            try
            {
                value = int.Parse(input, CultureInfo.CurrentCulture);
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
    }
}

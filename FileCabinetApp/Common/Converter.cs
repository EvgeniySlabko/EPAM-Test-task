using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Converter.
    /// </summary>
    public class Converter
    {
        /// <summary>
        /// Convert string to Value.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="inputString">Given string.</param>
        /// <returns>Result.</returns>
        public Tuple<bool, string, T> Convert<T>(string inputString)
        {
            if (inputString is null)
            {
                throw new ArgumentNullException(nameof(inputString));
            }

            string resultMessage;
            bool converted = true;
            T result = default;
            try
            {
                result = (T)System.Convert.ChangeType(inputString, typeof(T), CultureInfo.InvariantCulture);
                resultMessage = "Succesful convert";
            }
            catch (InvalidCastException)
            {
                converted = false;
                resultMessage = "Conversation error. Invalid cast";
            }
            catch (FormatException)
            {
                converted = false;
                resultMessage = "Conversation error. Format error";
            }
            catch (OverflowException)
            {
                converted = false;
                resultMessage = "Conversation error. Overflow error";
            }

            return new Tuple<bool, string, T>(converted, resultMessage, result);
        }
    }
}

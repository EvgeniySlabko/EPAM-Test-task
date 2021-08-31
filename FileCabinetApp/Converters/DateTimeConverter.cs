using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Convertor for DateTime type.
    /// </summary>
    public class DateTimeConverter : IConverter<DateTime>
    {
        private readonly string[] dateFormats = { "dd-MM-yyyy", "dd/MM/yyyy", "dd.MM.yyyy", "yyyy-MM-dd" };

        /// <summary>
        /// converts a string to a value of char type.
        /// </summary>
        /// <param name="inputString">Given string.</param>
        /// <returns>Result tupple. bool value - validation result. string value - message string. DateTime value - result of conversion.</returns>
        public Tuple<bool, string, DateTime> Convert(string inputString)
        {
            bool successfulConvert = true;
            string message = "Succesful convert";
            DateTime value = default;
            try
            {
                value = DateTime.ParseExact(inputString, this.dateFormats, CultureInfo.CurrentCulture);
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
        public Func<string, Tuple<bool, string, DateTime>> GetDelegate()
        {
            return this.Convert;
        }
    }
}

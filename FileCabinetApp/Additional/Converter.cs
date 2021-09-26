using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Converter.
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// Convert string to Value.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="inputString">Given string.</param>
        /// <returns>Result.</returns>
        public static Tuple<bool, string, T> Convert<T>(string inputString)
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
                resultMessage = string.Empty;
            }
            catch (InvalidCastException)
            {
                converted = false;
                resultMessage = "Invalid cast";
            }
            catch (FormatException)
            {
                converted = false;
                resultMessage = "Format error";
            }
            catch (OverflowException)
            {
                converted = false;
                resultMessage = "Overflow error";
            }

            return new Tuple<bool, string, T>(converted, resultMessage, result);
        }

        /// <summary>
        /// Convert string to Value.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="inputString">Given string.</param>
        /// <returns>Result.</returns>
        public static T TryConvert<T>(string inputString)
        {
            Tuple<bool, string, T> result = Convert<T>(inputString);
            if (!result.Item1)
            {
                throw new ArgumentException(result.Item2);
            }

            return result.Item3;
        }

        /// <summary>
        /// Convert string to Value.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="inputString">Given string.</param>
        /// <returns>Result.</returns>
        public static object TryConvertToObject<T>(string inputString)
        {
            return TryConvert<T>(inputString);
        }
    }
}

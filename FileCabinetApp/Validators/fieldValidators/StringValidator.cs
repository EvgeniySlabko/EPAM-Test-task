using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Vaidator for string type.
    /// </summary>
    public class StringValidator : IValidator<string>
    {
        private readonly int minLen;

        private readonly int maxLen;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringValidator"/> class.
        /// String validator constructor.
        /// </summary>
        /// <param name="maxLen">Maximum string length.</param>
        /// <param name="minLen">Minimal string length.</param>
        public StringValidator(int minLen, int maxLen)
        {
            this.maxLen = maxLen;
            this.minLen = minLen;
        }

        /// <summary>
        /// String validation.
        /// </summary>
        /// <param name="inputStr">Given string.</param>
        /// <returns>Result tupple. bool value - validation result. string value - message string.</returns>
        public Tuple<bool, string> Validate(string inputStr)
        {
            if (inputStr is null)
            {
                throw new ArgumentNullException(nameof(inputStr));
            }

            bool valid;
            string message;
            if (inputStr.Length < this.maxLen && inputStr.Length > this.minLen)
            {
                message = string.Empty;
                valid = true;
            }
            else
            {
                message = "Invaling string length";
                valid = false;
            }

            return new Tuple<bool, string>(valid, message);
        }
    }
}

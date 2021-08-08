using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Vaidator for string type.
    /// </summary>
    public class StringValidator : IValidator<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringValidator"/> class.
        /// String validator constructor.
        /// </summary>
        /// <param name="maxLen">Maximum string length.</param>
        /// <param name="minLen">Minimal string length.</param>
        public StringValidator(int maxLen, int minLen)
        {
            this.MaxLen = maxLen;
            this.MinLen = minLen;
        }

        /// <summary>
        /// Gets minimum length.
        /// </summary>
        /// <value>Minimum length.</value>
        public int MinLen { get; }

        /// <summary>
        /// Gets maximum length.
        /// </summary>
        /// <value>Maximum length.</value>
        public int MaxLen { get; }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <returns>Gets delegate for Validate mrthod.</returns>
        public Func<string, Tuple<bool, string>> GetDelegate()
        {
            return this.Validate;
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
            if (inputStr.Length < this.MaxLen && inputStr.Length > this.MinLen)
            {
                message = "Succesful";
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

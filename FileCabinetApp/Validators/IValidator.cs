using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// Interface for all validators.
    /// </summary>
    /// <typeparam name="T">The type of the variable to be validated.</typeparam>
    public interface IValidator<T>
    {
        /// <summary>
        /// Validation method.
        /// </summary>
        /// <param name="value">Given value or validation.</param>
        /// <returns>True if the value is valid otherwise false.</returns>
        public Tuple<bool, string> Validate(T value);

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <returns>True if the value is valid otherwise false.</returns>
        public Func<T, Tuple<bool, string>> GetDelegate();
    }

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

        private int MaxLen { get; }

        private int MinLen { get; }

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

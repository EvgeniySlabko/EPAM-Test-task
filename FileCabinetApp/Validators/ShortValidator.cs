using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// Vaidator for Short type.
    /// </summary>
    public class ShortValidator : IValidator<short>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShortValidator"/> class.
        /// String validator constructor.
        /// </summary>
        /// <param name="maxValue">Maximum string length.</param>
        /// <param name="minValue">Minimal string length.</param>
        public ShortValidator(short minValue, short maxValue)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }

        private short MinValue { get; }

        private short MaxValue { get; }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <returns>Gets delegate for Validate method.</returns>
        public Func<short, Tuple<bool, string>> GetDelegate()
        {
            return this.Validate;
        }

        /// <summary>
        /// String validation.
        /// </summary>
        /// <param name="inputValue">Given string.</param>
        /// <returns>Result tupple. bool value - validation result. string value - message string.</returns>
        public Tuple<bool, string> Validate(short inputValue)
        {
            bool valid;
            string message;
            if (inputValue < this.MaxValue && inputValue > this.MinValue)
            {
                message = "Succesful";
                valid = true;
            }
            else
            {
                message = "Invalind value";
                valid = false;
            }

            return new Tuple<bool, string>(valid, message);
        }
    }
}

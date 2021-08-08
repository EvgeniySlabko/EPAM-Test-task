using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// Vaidator for decimal type.
    /// </summary>
    public class DecimalValidator : IValidator<decimal>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalValidator"/> class.
        /// String validator constructor.
        /// </summary>
        /// <param name="maxValue">Maximum value.</param>
        /// <param name="minValue">Minimal value length.</param>
        public DecimalValidator(decimal minValue, decimal maxValue)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }

        private decimal MinValue { get; }

        private decimal MaxValue { get; }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <returns>Gets delegate for Validate mrthod.</returns>
        public Func<decimal, Tuple<bool, string>> GetDelegate()
        {
            return this.Validate;
        }

        /// <summary>
        /// Given value.
        /// </summary>
        /// <param name="inputValue">Given decimal value.</param>
        /// <returns>Result tupple. bool value - validation result. string value - message string.</returns>
        public Tuple<bool, string> Validate(decimal inputValue)
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

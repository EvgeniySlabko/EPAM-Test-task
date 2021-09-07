using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// Vaidator for int type.
    /// </summary>
    public class IntValidator : IValidator<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntValidator"/> class.
        /// String validator constructor.
        /// </summary>
        /// <param name="maxValue">Maximum value.</param>
        /// <param name="minValue">Minimal value length.</param>
        public IntValidator(int minValue, int maxValue)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }

        /// <summary>
        /// Gets minimum value.
        /// </summary>
        /// <value>Minimum value.</value>
        public int MinValue { get; }

        /// <summary>
        /// Gets maximum value.
        /// </summary>
        /// <value>Maximum value.</value>
        public int MaxValue { get; }

        /// <summary>
        /// String validation.
        /// </summary>
        /// <param name="inputValue">Given string.</param>
        /// <returns>Result tupple. bool value - validation result. string value - message string.</returns>
        public Tuple<bool, string> Validate(int inputValue)
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

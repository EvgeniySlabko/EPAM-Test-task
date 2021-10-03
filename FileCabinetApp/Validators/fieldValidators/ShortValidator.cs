using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Vaidator for Short type.
    /// </summary>
    public class ShortValidator : IValidator<short>
    {
        private readonly short minValue;

        private readonly short maxValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortValidator"/> class.
        /// String validator constructor.
        /// </summary>
        /// <param name="maxValue">Maximum string length.</param>
        /// <param name="minValue">Minimal string length.</param>
        public ShortValidator(short minValue, short maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
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
            if (inputValue < this.maxValue && inputValue > this.minValue)
            {
                message = string.Empty;
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

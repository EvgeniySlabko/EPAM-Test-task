using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Vaidator for int type.
    /// </summary>
    public class IntValidator : IValidator<int>
    {
        private readonly int minValue;

        private readonly int maxValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntValidator"/> class.
        /// String validator constructor.
        /// </summary>
        /// <param name="maxValue">Maximum value.</param>
        /// <param name="minValue">Minimal value length.</param>
        public IntValidator(int minValue, int maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        /// <summary>
        /// String validation.
        /// </summary>
        /// <param name="inputValue">Given string.</param>
        /// <returns>Result tupple. bool value - validation result. string value - message string.</returns>
        public Tuple<bool, string> Validate(int inputValue)
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

using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// Vaidator for DateTime type.
    /// </summary>
    public class DateValidator : IValidator<DateTime>
    {
        private readonly DateTime minDateOfBirth;
        private readonly DateTime maxDateOfBirth;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateValidator"/> class.
        /// String validator constructor.
        /// </summary>
        /// <param name="maxDateOfBirth">Maximum date of birth.</param>
        /// <param name="minDateOfBirth">Minimal date of birth.</param>
        public DateValidator(DateTime minDateOfBirth, DateTime maxDateOfBirth)
        {
            this.minDateOfBirth = minDateOfBirth;
            this.maxDateOfBirth = maxDateOfBirth;
        }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <returns>Gets delegate for Validate mrthod.</returns>
        public Func<DateTime, Tuple<bool, string>> GetDelegate()
        {
            return this.Validate;
        }

        /// <summary>
        /// String validation.
        /// </summary>
        /// <param name="inputDateOfBirth">Given string.</param>
        /// <returns>Result tupple. bool value - validation result. string value - message string.</returns>
        public Tuple<bool, string> Validate(DateTime inputDateOfBirth)
        {
            bool valid;
            string message;
            if (inputDateOfBirth < this.maxDateOfBirth && inputDateOfBirth > this.minDateOfBirth)
            {
                message = "Succesful";
                valid = true;
            }
            else
            {
                message = "Invaling date of birth";
                valid = false;
            }

            return new Tuple<bool, string>(valid, message);
        }
    }
}

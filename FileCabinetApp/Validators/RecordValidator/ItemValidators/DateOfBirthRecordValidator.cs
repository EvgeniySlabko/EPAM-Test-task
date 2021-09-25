using System;

namespace FileCabinetApp
{
    /// <summary>
    /// DateOfBirthRecord validator.
    /// </summary>
    public class DateOfBirthRecordValidator : IRecordValidator
    {
        private readonly DateTime minDate;

        private readonly DateTime maxDate;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthRecordValidator"/> class.
        /// String validator constructor.
        /// </summary>
        /// <param name="maxDate">Maximum date.</param>
        /// <param name="minDate">Minimal date.</param>
        public DateOfBirthRecordValidator(DateTime minDate, DateTime maxDate)
        {
            this.maxDate = maxDate;
            this.minDate = minDate;
        }

        /// <inheritdoc/>
        public bool ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            return record.DateOfBirth < this.maxDate && record.DateOfBirth > this.minDate;
        }
    }
}

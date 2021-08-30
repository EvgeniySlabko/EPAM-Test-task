using System;

namespace FileCabinetApp
{
    /// <summary>
    /// IdentificationNumber validator.
    /// </summary>
    public class IdentificationNumberRecordValidator : IRecordValidator
    {
        private readonly decimal minValue;

        private readonly decimal maxValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentificationNumberRecordValidator"/> class.
        /// String validator constructor.
        /// </summary>
        /// <param name="maxValue">Maximum value.</param>
        /// <param name="minValue">Minimal value.</param>
        public IdentificationNumberRecordValidator(decimal minValue, decimal maxValue)
        {
            this.maxValue = maxValue;
            this.minValue = minValue;
        }

        /// <inheritdoc/>
        public bool ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            return record.IdentificationNumber < this.maxValue && record.IdentificationNumber > this.minValue;
        }
    }
}

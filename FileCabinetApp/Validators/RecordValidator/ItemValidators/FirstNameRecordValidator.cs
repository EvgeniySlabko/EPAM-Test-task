using System;

namespace FileCabinetApp
{
    /// <summary>
    /// FirstName validator.
    /// </summary>
    public class FirstNameRecordValidator : IRecordValidator
    {
        private readonly int minLen;

        private readonly int maxLen;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstNameRecordValidator"/> class.
        /// String validator constructor.
        /// </summary>
        /// <param name="maxLen">Maximum string length.</param>
        /// <param name="minLen">Minimal string length.</param>
        public FirstNameRecordValidator(int minLen, int maxLen)
        {
            this.maxLen = maxLen;
            this.minLen = minLen;
        }

        /// <inheritdoc/>
        public bool ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            return record.FirstName.Length < this.maxLen && record.FirstName.Length > this.minLen;
        }
    }
}

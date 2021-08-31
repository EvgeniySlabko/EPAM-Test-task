using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Last name validator.
    /// </summary>
    public class LastNameRecordValidator : IRecordValidator
    {
        private readonly int minLen;

        private readonly int maxLen;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastNameRecordValidator"/> class.
        /// String validator constructor.
        /// </summary>
        /// <param name="maxLen">Maximum string length.</param>
        /// <param name="minLen">Minimal string length.</param>
        public LastNameRecordValidator(int minLen, int maxLen)
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

            return record.LastName.Length < this.maxLen && record.LastName.Length > this.minLen;
        }
    }
}

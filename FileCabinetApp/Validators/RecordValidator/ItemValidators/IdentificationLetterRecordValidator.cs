using System;

namespace FileCabinetApp
{
    /// <summary>
    /// IdentificationLetterValidator validator.
    /// </summary>
    public class IdentificationLetterRecordValidator : IRecordValidator
    {
        private readonly Predicate<char> predicate;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentificationLetterRecordValidator"/> class.
        /// </summary>
        /// <param name="predicate">Predicate for validate char character.</param>
        public IdentificationLetterRecordValidator(Predicate<char> predicate)
        {
            this.predicate = predicate;
        }

        /// <inheritdoc/>
        public bool ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            return this.predicate(record.IdentificationLetter);
        }
    }
}

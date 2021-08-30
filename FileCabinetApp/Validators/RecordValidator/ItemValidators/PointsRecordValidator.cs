using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Points validator.
    /// </summary>
    public class PointsRecordValidator : IRecordValidator
    {
        private readonly short minValue;

        private readonly short maxValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="PointsRecordValidator"/> class.
        /// String validator constructor.
        /// </summary>
        /// <param name="maxValue">Maximum value.</param>
        /// <param name="minValue">Minimal value.</param>
        public PointsRecordValidator(short minValue, short maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        /// <inheritdoc/>
        public bool ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            return record.PointsForFourTests > this.minValue && record.PointsForFourTests < this.maxValue;
        }
    }
}

using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// Composite validator builder.
    /// </summary>
    public class ValidatorBuilder
    {
        private readonly List<IRecordValidator> validators = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorBuilder"/> class.
        /// </summary>
        public ValidatorBuilder()
        {
        }

        /// <summary>
        /// Add FirstNameRecordValidator instance.
        /// </summary>
        /// <param name="min">Min value.</param>
        /// <param name="max">Max value.</param>
        /// <returns>ValidatorBuilder.</returns>
        public ValidatorBuilder ValidateFirstName(int min, int max)
        {
            this.validators.Add(new FirstNameRecordValidator(min, max));
            return this;
        }

        /// <summary>
        /// Add LastNameRecordValidator instance.
        /// </summary>
        /// <param name="min">Min value.</param>
        /// <param name="max">Max value.</param>
        /// <returns>ValidatorBuilder.</returns>
        public ValidatorBuilder ValidateLastName(int min, int max)
        {
            this.validators.Add(new LastNameRecordValidator(min, max));
            return this;
        }

        /// <summary>
        /// Add ValidateDate instance.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns>ValidatorBuilder.</returns>
        public ValidatorBuilder ValidateDate(DateTime from, DateTime to)
        {
            this.validators.Add(new DateOfBirthRecordValidator(from, to));
            return this;
        }

        /// <summary>
        /// Add ValidateIdentificationNumber instance.
        /// </summary>
        /// <param name="min">Min value.</param>
        /// <param name="max">Max value.</param>
        /// <returns>ValidatorBuilder.</returns>
        public ValidatorBuilder ValidateIdentificationNumber(decimal min, decimal max)
        {
            this.validators.Add(new IdentificationNumberRecordValidator(min, max));
            return this;
        }

        /// <summary>
        /// Add PointsRecordValidator instance.
        /// </summary>
        /// <param name="min">Min value.</param>
        /// <param name="max">Max value.</param>
        /// <returns>ValidatorBuilder.</returns>
        public ValidatorBuilder ValidatePoints(short min, short max)
        {
            this.validators.Add(new PointsRecordValidator(min, max));
            return this;
        }

        /// <summary>
        /// Build Letter validator.
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <returns>Validator builder.</returns>
        public ValidatorBuilder VallidateLetter(Predicate<char> predicate)
        {
            this.validators.Add(new IdentificationLetterRecordValidator(predicate));
            return this;
        }

        /// <summary>
        /// Create composite validator.
        /// </summary>
        /// <returns>Composite validator.</returns>
        public IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }
    }
}

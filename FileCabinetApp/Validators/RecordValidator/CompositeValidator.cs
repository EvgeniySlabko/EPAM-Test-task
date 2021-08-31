using System;
using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp
{
    /// <summary>
    /// Container for record validators.
    /// </summary>
    public class CompositeValidator : IRecordValidator
    {
        private readonly List<IRecordValidator> recordValidators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">Validators.</param>
        public CompositeValidator(IEnumerable<IRecordValidator> validators)
        {
            this.recordValidators = validators.ToList();
        }

        /// <inheritdoc/>
        public bool ValidateParameters(FileCabinetRecord record)
        {
            foreach (var validator in this.recordValidators)
            {
                if (!validator.ValidateParameters(record))
                {
                    return false;
                }
            }

            return true;
        }
    }
}

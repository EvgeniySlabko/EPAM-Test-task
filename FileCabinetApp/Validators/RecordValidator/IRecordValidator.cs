using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Interface for validators.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Method for parameter validation.
        /// </summary>
        /// <param name="record">Given parameters.</param>
        /// <returns>Validation result.</returns>
        public bool ValidateParameters(FileCabinetRecord record);
    }
}

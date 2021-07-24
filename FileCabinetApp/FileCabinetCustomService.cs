using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// File cabinet service with custom parameters validation.
    /// </summary>
    public class FileCabinetCustomService : FileCabinetService
    {
        /// <summary>
        /// Custom parameters validation method. Records with any date of birth and any number of points for fout tests.
        /// </summary>
        /// <param name="record">Given record.</param>
        /// <returns>true if passed validation otherwise false.</returns>
        protected override bool ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(record.FirstName) || record.FirstName.Length < 2 || record.FirstName.Length > 60)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(record.LastName) || record.LastName.Length < 2 || record.LastName.Length > 60)
            {
                return false;
            }

            if (!char.IsLetter(record.IdentificationLetter))
            {
                return false;
            }

            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Validator with custom rule set.
    /// </summary>
    public class CustomValidator : IRecordValidator
    {
        /// <summary>
        /// Validator with custom validation rules(reduced rele set).
        /// </summary>
        /// <param name="record">Given record.</param>
        /// <returns>true, if the parameters are valid otherwise false.</returns>
        public bool ValidateParameters(FileCabinetRecord record)
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

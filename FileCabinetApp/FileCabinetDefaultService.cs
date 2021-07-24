using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// File cabinet service with default parameters validation.
    /// </summary>
    public class FileCabinetDefaultService : FileCabinetService
    {
        /// <summary>
        /// Default parameters validation method.
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

            if (record.DateOfBirth < new DateTime(1950, 1, 1) || record.DateOfBirth > DateTime.Today)
            {
                return false;
            }

            if (!char.IsLetter(record.IdentificationLetter))
            {
                return false;
            }

            if (record.PointsForFourTests > 400 || record.PointsForFourTests < 0)
            {
                return false;
            }

            return true;
        }
    }
}

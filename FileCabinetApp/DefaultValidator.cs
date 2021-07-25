using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class DefaultValidator : IRecordValidator
    {
        /// <summary>
        /// Validator with default validation rules.
        /// </summary>
        /// <param name="record">Given record.</param>
        /// <returns></returns>
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

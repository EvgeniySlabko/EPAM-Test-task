using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Validator with default rule set.
    /// </summary>
    public class ServiceValidator : IRecordValidator
    {
        private readonly ValidationRuleSet validationRuleSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceValidator"/> class.
        /// </summary>
        /// <param name="validationRuleSet">Given validation rule set.</param>
        public ServiceValidator(ValidationRuleSet validationRuleSet)
        {
            this.validationRuleSet = validationRuleSet;
        }

        /// <summary>
        /// Validator with default validation rules.
        /// </summary>
        /// <param name="record">Given record.</param>
        /// <returns>true, if the parameters are valid otherwise false.</returns>
        public bool ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                return false;
            }

            Tuple<bool, string> validationResult;

            if (this.validationRuleSet.FirstNameVAidator is not null)
            {
                validationResult = this.validationRuleSet.FirstNameVAidator.GetDelegate()(record.FirstName);

                if (!validationResult.Item1)
                {
                    return false;
                }
            }

            if (this.validationRuleSet.LastNameValidator is not null)
            {
                validationResult = this.validationRuleSet.LastNameValidator.GetDelegate()(record.LastName);

                if (!validationResult.Item1)
                {
                    return false;
                }
            }

            if (this.validationRuleSet.PointsForFourTestsValidator is not null)
            {
                validationResult = this.validationRuleSet.PointsForFourTestsValidator.GetDelegate()(record.PointsForFourTests);

                if (!validationResult.Item1)
                {
                    return false;
                }
            }

            if (this.validationRuleSet.IdentificationNumberValidator is not null)
            {
                validationResult = this.validationRuleSet.IdentificationNumberValidator.GetDelegate()(record.IdentificationNumber);

                if (!validationResult.Item1)
                {
                    return false;
                }
            }

            if (this.validationRuleSet.IdentificationLetterValidator is not null)
            {
                validationResult = this.validationRuleSet.IdentificationLetterValidator.GetDelegate()(record.IdentificationLetter);

                if (!validationResult.Item1)
                {
                    return false;
                }
            }

            if (this.validationRuleSet.DateValidator is not null)
            {
                validationResult = this.validationRuleSet.DateValidator.GetDelegate()(record.DateOfBirth);

                if (!validationResult.Item1)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

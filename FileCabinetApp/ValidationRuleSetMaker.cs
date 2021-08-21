using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Validation rule set creator.
    /// </summary>
    public static class ValidationRuleSetMaker
    {
        private const int NameMaxLength = 40;
        private const int NameMinLength = 2;
        private const int MinPointsForFourTests = 0;
        private const int MaxPointsForFourTests = 400;
        private const decimal MinIdentificationNumber = 0;
        private const decimal MaxIdentificationNumber = decimal.MaxValue;
        private static readonly DateTime MinDateOfBirth = new (1960, 1, 1);

        /// <summary>
        /// Returns default rule set.
        /// </summary>
        /// <returns>Default rule set.</returns>
        public static ValidationRuleSet MakeDefaultValidationSet()
        {
            return new ValidationRuleSet
            {
                DateValidator = new DateValidator(MinDateOfBirth, DateTime.Now),
                FirstNameVAidator = new StringValidator(NameMaxLength, NameMinLength),
                LastNameValidator = new StringValidator(NameMaxLength, NameMinLength),
                PointsForFourTestsValidator = new ShortValidator(MinPointsForFourTests, MaxPointsForFourTests),
                IdentificationNumberValidator = new DecimalValidator(MinIdentificationNumber, MaxIdentificationNumber),
                IdentificationLetterValidator = new CharValidator(c => char.IsLetter(c)),
            };
        }

        /// <summary>
        /// Returns custom rule set.
        /// </summary>
        /// <returns>Custom rule set.</returns>
        public static ValidationRuleSet MakeCustomValidationSet()
        {
            var predicateCharValidationList = new List<Predicate<char>> { (c) => char.IsLetter(c), (c) => char.IsLower(c), };
            return new ValidationRuleSet
            {
                DateValidator = new DateValidator(MinDateOfBirth, DateTime.Now),
                FirstNameVAidator = new StringValidator(NameMaxLength, NameMinLength),
                LastNameValidator = new StringValidator(NameMaxLength, NameMinLength),
                PointsForFourTestsValidator = new ShortValidator(MinPointsForFourTests, MaxPointsForFourTests),
                IdentificationNumberValidator = new DecimalValidator(MinIdentificationNumber, MaxIdentificationNumber),
                IdentificationLetterValidator = new CharValidator(predicateCharValidationList),
            };
        }
    }
}

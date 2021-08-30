using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Default record validator.
    /// </summary>
    public class DefaultRecordValidator : CompositeValidator
    {
        private const int FirstNameMaxLength = 40;
        private const int FirstNameMinLength = 2;
        private const int LastNameMaxLength = 40;
        private const int LastNameMinLength = 2;
        private const int MinPointsForFourTests = 0;
        private const int MaxPointsForFourTests = 400;
        private const decimal MinIdentificationNumber = 0;
        private const decimal MaxIdentificationNumber = decimal.MaxValue;
        private static readonly DateTime MinDateOfBirth = new (1970, 1, 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRecordValidator"/> class.
        /// </summary>
        public DefaultRecordValidator()
            : base(new IRecordValidator[]
            {
                new FirstNameRecordValidator(FirstNameMinLength, FirstNameMaxLength),
                new LastNameRecordValidator(LastNameMinLength, LastNameMaxLength),
                new DateOfBirthRecordValidator(MinDateOfBirth, DateTime.Now),
                new IdentificationNumberRecordValidator(MinIdentificationNumber, MaxIdentificationNumber),
                new PointsRecordValidator(MinPointsForFourTests, MaxPointsForFourTests),
                new IdentificationLetterRecordValidator(c => char.IsLetter(c)),
                new IdentificationLetterRecordValidator(c => !char.IsUpper(c)),
            })
        {
        }
    }
}

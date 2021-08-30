using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Custom record validator.
    /// </summary>
    public class CustomRecordValidator : CompositeValidator
    {
        private const int FirstNameMaxLength = 40;
        private const int FirstNameMinLength = 2;
        private const int LastNameMaxLength = 50;
        private const int LastNameMinLength = 2;
        private const int MinPointsForFourTests = 0;
        private const int MaxPointsForFourTests = 400;
        private const decimal MinIdentificationNumber = 0;
        private const decimal MaxIdentificationNumber = decimal.MaxValue;
        private static readonly DateTime MinDateOfBirth = new (1960, 1, 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomRecordValidator"/> class.
        /// </summary>
        public CustomRecordValidator()
            : base(new IRecordValidator[]
            {
                new FirstNameRecordValidator(FirstNameMinLength, FirstNameMaxLength),
                new LastNameRecordValidator(LastNameMinLength, LastNameMaxLength),
                new DateOfBirthRecordValidator(MinDateOfBirth, DateTime.Now),
                new IdentificationNumberRecordValidator(MinIdentificationNumber, MaxIdentificationNumber),
                new PointsRecordValidator(MinPointsForFourTests, MaxPointsForFourTests),
                new IdentificationLetterRecordValidator(c => char.IsLetter(c)),
            })
        {
        }
    }
}

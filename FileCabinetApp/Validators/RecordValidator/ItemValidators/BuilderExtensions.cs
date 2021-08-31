using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Extensions for ValidatorBuilder.
    /// </summary>
    public static class BuilderExtensions
    {
        private const int FirstNameMaxLengthDefault = 40;
        private const int FirstNameMinLengthDefault = 2;
        private const int LastNameMaxLengthDefault = 40;
        private const int LastNameMinLengthDefault = 2;
        private const int MinPointsForFourTestsDefault = 0;
        private const int MaxPointsForFourTestsDefault = 400;
        private const decimal MinIdentificationNumberDefault = 0;
        private const decimal MaxIdentificationNumberDefault = decimal.MaxValue;

        private const int FirstNameMaxLengthCustom = 40;
        private const int FirstNameMinLengthCustom = 2;
        private const int LastNameMaxLengthCustom = 40;
        private const int LastNameMinLengthCustom = 2;
        private const int MinPointsForFourTestsCustom = 0;
        private const int MaxPointsForFourTestsCustom = 400;
        private const decimal MinIdentificationNumberCustom = 0;
        private const decimal MaxIdentificationNumberCustom = decimal.MaxValue;

        private static readonly DateTime MinDateOfBirthDefault = new (1970, 1, 1);
        private static readonly DateTime MaxDateOfBirthDefault = DateTime.Now;

        private static readonly DateTime MinDateOfBirthCustom = new (1970, 1, 1);
        private static readonly DateTime MaxDateOfBirthCustom = DateTime.Now;

        private static readonly Predicate<char> FirstPredicateDefault = (c) => char.IsLetter(c);
        private static readonly Predicate<char> SecondPredicateDefault = (c) => char.IsLower(c);

        private static readonly Predicate<char> FirstPredicateCustom = (c) => char.IsLetter(c);
        private static readonly Predicate<char> SecondPredicateCustom = (c) => char.IsLower(c);

        /// <summary>
        /// Create default composite validator extesion.
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Composite validator.</returns>
        public static IRecordValidator CreateDefault(this ValidatorBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return new ValidatorBuilder().ValidateFirstName(FirstNameMinLengthDefault, FirstNameMaxLengthDefault).
                ValidateLastName(LastNameMinLengthDefault, LastNameMaxLengthDefault).
                ValidateDate(MinDateOfBirthDefault, MaxDateOfBirthDefault).
                ValidatePoints(MinPointsForFourTestsDefault, MaxPointsForFourTestsDefault).
                VallidateLetter(FirstPredicateDefault).
                VallidateLetter(SecondPredicateDefault).
                ValidateIdentificationNumber(MinIdentificationNumberDefault, MaxIdentificationNumberDefault).
                Create();
        }

        /// <summary>
        /// Create custom composite validator extesion.
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Composite validator.</returns>
        public static IRecordValidator CreateCustom(this ValidatorBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return new ValidatorBuilder().ValidateFirstName(FirstNameMinLengthCustom, FirstNameMaxLengthCustom).
                ValidateLastName(LastNameMinLengthCustom, LastNameMaxLengthCustom).
                ValidateDate(MinDateOfBirthCustom, MaxDateOfBirthCustom).
                ValidatePoints(MinPointsForFourTestsCustom, MaxPointsForFourTestsCustom).
                VallidateLetter(FirstPredicateCustom).
                VallidateLetter(SecondPredicateCustom).
                ValidateIdentificationNumber(MinIdentificationNumberCustom, MaxIdentificationNumberCustom).
                Create();
        }
    }
}

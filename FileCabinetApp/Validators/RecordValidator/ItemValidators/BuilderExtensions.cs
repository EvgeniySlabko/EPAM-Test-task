using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Extensions for ValidatorBuilder.
    /// </summary>
    public static class BuilderExtensions
    {
        private static readonly Predicate<char> FirstPredicate = (c) => char.IsLetter(c);
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

            var defaultSettings = ValidationSetLoader.LoadRules(Constants.ValidationSettingsFileName)[Constants.DefaultValidationSettingsName];

            return new ValidatorBuilder().ValidateFirstName(defaultSettings.FirstName.Min, defaultSettings.FirstName.Max).
                ValidateLastName(defaultSettings.LastName.Min, defaultSettings.LastName.Max).
                ValidateDate(defaultSettings.DateModel.From, defaultSettings.DateModel.To).
                ValidatePoints(defaultSettings.PointsModel.Min, defaultSettings.PointsModel.Max).
                VallidateLetter(FirstPredicate).
                VallidateLetter(SecondPredicateCustom).
                ValidateIdentificationNumber(defaultSettings.IdentificationNumberModel.Min, defaultSettings.IdentificationNumberModel.Max).
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

            var customSettings = ValidationSetLoader.LoadRules(Constants.ValidationSettingsFileName)[Constants.CustomValidationSettingsName];

            return new ValidatorBuilder().ValidateFirstName(customSettings.FirstName.Min, customSettings.FirstName.Max).
                ValidateLastName(customSettings.LastName.Min, customSettings.LastName.Max).
                ValidateDate(customSettings.DateModel.From, customSettings.DateModel.To).
                ValidatePoints(customSettings.PointsModel.Min, customSettings.PointsModel.Max).
                VallidateLetter(c => char.IsLetter(c)).
                VallidateLetter(c => char.IsLower(c)).
                ValidateIdentificationNumber(customSettings.IdentificationNumberModel.Min, customSettings.IdentificationNumberModel.Max).
                Create();
        }
    }
}

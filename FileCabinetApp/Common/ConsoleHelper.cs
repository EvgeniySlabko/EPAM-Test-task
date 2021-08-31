using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Help with entering data from the console.
    /// </summary>
    public static class ConsoleHelper
    {
        /// <summary>
        /// Enter record.
        /// </summary>
        /// <param name="newRecord">Record.</param>
        /// <param name="validationRuleSet">Validation rule set.</param>
        public static void EnterRecord(out FileCabinetRecord newRecord, ValidationRuleSet validationRuleSet)
        {
            if (validationRuleSet is null)
            {
                throw new ArgumentNullException(nameof(validationRuleSet));
            }

            newRecord = new FileCabinetRecord();
            Console.WriteLine();
            Console.Write(StringManager.Rm.GetString("FirstNameMessage", CultureInfo.CurrentCulture));
            newRecord.FirstName = ReadInput(new StringConverter().GetDelegate(), validationRuleSet.FirstNameVAidator.GetDelegate());

            Console.Write(StringManager.Rm.GetString("LastNameMessage", CultureInfo.CurrentCulture));
            newRecord.LastName = ReadInput(new StringConverter().GetDelegate(), validationRuleSet.LastNameValidator.GetDelegate());

            Console.Write(StringManager.Rm.GetString("DateOfBirthMessage", CultureInfo.CurrentCulture));
            newRecord.DateOfBirth = ReadInput(new DateTimeConverter().GetDelegate(), validationRuleSet.DateValidator.GetDelegate());

            Console.Write(StringManager.Rm.GetString("IdentificationNumberMessage", CultureInfo.CurrentCulture));
            newRecord.IdentificationNumber = ReadInput(new DecimalConverter().GetDelegate(), validationRuleSet.IdentificationNumberValidator.GetDelegate());

            Console.Write(StringManager.Rm.GetString("IdentificationLetterMessage", CultureInfo.CurrentCulture));
            newRecord.IdentificationLetter = ReadInput(new CharConverter().GetDelegate(), validationRuleSet.IdentificationLetterValidator.GetDelegate());

            Console.Write(StringManager.Rm.GetString("PointsForFourTestsMessage", CultureInfo.CurrentCulture));
            newRecord.PointsForFourTests = ReadInput(new ShortConverter().GetDelegate(), validationRuleSet.PointsForFourTestsValidator.GetDelegate());
        }

        /// <summary>
        /// Rewrite file dialog.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <returns>Result.</returns>
        public static bool RewriteFileDialog(string fileName)
        {
            var reqestMessage = new StringBuilder();
            reqestMessage.Append("File is exist - rewrite ");
            reqestMessage.Append(fileName);
            reqestMessage.Append(" ?");
            reqestMessage.Append("[Y/n]");
            return YesOrNoDialog(reqestMessage.ToString());
        }

        private static bool YesOrNoDialog(string message)
        {
            Console.WriteLine(message, " [Y/n]");
            string answer = Console.ReadLine();
            if (answer.Length.Equals(1))
            {
                char answerLetter = answer.ToLower(CultureInfo.CurrentCulture)[0];
                if (answerLetter.Equals('y'))
                {
                    return true;
                }
                else if (answerLetter.Equals('n'))
                {
                    return false;
                }
            }

            return YesOrNoDialog(message);
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine(StringManager.Rm.GetString("ConversationFailedMessage", CultureInfo.CurrentCulture), conversionResult.Item2);
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine(StringManager.Rm.GetString("ValidationFailedMessage", CultureInfo.CurrentCulture), validationResult.Item2);
                    continue;
                }

                return value;
            }
            while (true);
        }
    }
}

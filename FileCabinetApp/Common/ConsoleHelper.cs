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
        /// Enter record from connsole.
        /// </summary>
        /// <param name="newRecord">Entered record.</param>
        public static void EnterRecord(out FileCabinetRecord newRecord)
        {
            newRecord = new FileCabinetRecord();
            Console.WriteLine();
            Console.Write(Program.Rm.GetString("FirstNameMessage", CultureInfo.CurrentCulture));
            newRecord.FirstName = ReadInput(new StringConverter().GetDelegate(), Program.validationRuleSet.FirstNameVAidator.GetDelegate());

            Console.Write(Program.Rm.GetString("LastNameMessage", CultureInfo.CurrentCulture));
            newRecord.LastName = ReadInput(new StringConverter().GetDelegate(), Program.validationRuleSet.LastNameValidator.GetDelegate());

            Console.Write(Program.Rm.GetString("DateOfBirthMessage", CultureInfo.CurrentCulture));
            newRecord.DateOfBirth = ReadInput(new DateTimeConverter().GetDelegate(), Program.validationRuleSet.DateValidator.GetDelegate());

            Console.Write(Program.Rm.GetString("IdentificationNumberMessage", CultureInfo.CurrentCulture));
            newRecord.IdentificationNumber = ReadInput(new DecimalConverter().GetDelegate(), Program.validationRuleSet.IdentificationNumberValidator.GetDelegate());

            Console.Write(Program.Rm.GetString("IdentificationLetterMessage", CultureInfo.CurrentCulture));
            newRecord.IdentificationLetter = ReadInput(new CharConverter().GetDelegate(), Program.validationRuleSet.IdentificationLetterValidator.GetDelegate());

            Console.Write(Program.Rm.GetString("PointsForFourTestsMessage", CultureInfo.CurrentCulture));
            newRecord.PointsForFourTests = ReadInput(new ShortConverter().GetDelegate(), Program.validationRuleSet.PointsForFourTestsValidator.GetDelegate());
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

        /// <summary>
        /// Display records.
        /// </summary>
        /// <param name="records">Records.</param>
        public static void DisplayRecordList(ReadOnlyCollection<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            if (records.Count.Equals(0))
            {
                Console.WriteLine(Program.Rm.GetString("EmptyListMessage", CultureInfo.CurrentCulture));
            }

            foreach (var record in records)
            {
                Console.WriteLine(Program.Rm.GetString("RecordInfoString", CultureInfo.CurrentCulture), record.Id, record.FirstName, record.LastName, record.DateOfBirth.ToString("yyyy-MMM-dd", DateTimeFormatInfo.InvariantInfo), record.IdentificationNumber, record.IdentificationLetter, record.PointsForFourTests);
            }
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
                    Console.WriteLine(Program.Rm.GetString("ConversationFailedMessage", CultureInfo.CurrentCulture), conversionResult.Item2);
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine(Program.Rm.GetString("ValidationFailedMessage", CultureInfo.CurrentCulture), validationResult.Item2);
                    continue;
                }

                return value;
            }
            while (true);
        }
    }
}

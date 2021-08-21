using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Handler for all service commands.
    /// </summary>
    public class CommandHandler : CommandHandlerBase
    {
        private const int DescriptionHelpIndex = 1;
        private const int CommandHelpIndex = 0;
        private const int ExplanationHelpIndex = 2;
        private readonly Tuple<string, Action<string>>[] commands;

        private readonly string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints the record statistics", "The '_stat_' command prints the record statistics." },
            new string[] { "create", "create a new record", "The 'create' command create a new record." },
            new string[] { "list", "display list of records", "The '_list_' display list of records." },
            new string[] { "edit", "edit existing record", "The 'edit' edit existing record." },
            new string[] { "find", "find existing record", "The 'find' find existing record." },
            new string[] { "export", "Export in CSV file", "The 'export' export records in CSV file." },
            new string[] { "import", "Import records from file", "The 'import' import records from file." },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler"/> class.
        /// </summary>
        public CommandHandler()
        {
            this.commands = new Tuple<string, Action<string>>[]
            {
                new Tuple<string, Action<string>>("help", this.PrintHelp),
                new Tuple<string, Action<string>>("exit", this.Exit),
                new Tuple<string, Action<string>>("stat", this.Stat),
                new Tuple<string, Action<string>>("create", this.Create),
                new Tuple<string, Action<string>>("list", this.List),
                new Tuple<string, Action<string>>("edit", this.Edit),
                new Tuple<string, Action<string>>("find", this.Find),
                new Tuple<string, Action<string>>("export", this.Export),
                new Tuple<string, Action<string>>("import", this.Import),
                new Tuple<string, Action<string>>("remove", this.Remove),
            };
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

        private void List(string parameters)
        {
            DisplayRecordList(Program.fileCabinetService.GetRecords());
        }

        private static void DisplayRecordList(ReadOnlyCollection<FileCabinetRecord> records)
        {
            if (records.Count.Equals(0))
            {
                Console.WriteLine(Program.Rm.GetString("EmptyListMessage", CultureInfo.CurrentCulture));
            }

            foreach (var record in records)
            {
                Console.WriteLine(Program.Rm.GetString("RecordInfoString", CultureInfo.CurrentCulture), record.Id, record.FirstName, record.LastName, record.DateOfBirth.ToString("yyyy-MMM-dd", DateTimeFormatInfo.InvariantInfo), record.IdentificationNumber, record.IdentificationLetter, record.PointsForFourTests);
            }
        }

        private void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(this.helpMessages, 0, this.helpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(this.helpMessages[index][ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine(Program.Rm.GetString("CommandExplanationMessage", CultureInfo.CurrentCulture), parameters);
                }
            }
            else
            {
                Console.WriteLine(Program.Rm.GetString("AvailableCommandsMessage", CultureInfo.CurrentCulture));

                foreach (var helpMessage in this.helpMessages)
                {
                    Console.WriteLine(Program.Rm.GetString("HelpMessageTemplate", CultureInfo.CurrentCulture), helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private void EnterRecord(out FileCabinetRecord newRecord)
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

        private void Exit(string parameters)
        {
            Console.WriteLine(Program.Rm.GetString("ExitMessage", CultureInfo.CurrentCulture));
            Program.isRunning = false;
        }

        private void Stat(string parameters)
        {
            var result = Program.fileCabinetService.GetStat();
            Console.WriteLine(Program.Rm.GetString("StatMessage", CultureInfo.CurrentCulture), result.Item1, result.Item2);
        }

        private void Edit(string parameters)
        {
            var result = new IntConverter().Convert(parameters);

            if (!result.Item1)
            {
                Console.WriteLine(result.Item2);
                return;
            }

            EnterRecord(out FileCabinetRecord record);
            record.Id = result.Item3;
            try
            {
                Program.fileCabinetService.Edit(record);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine(Program.Rm.GetString("UppdateRecordMessage", CultureInfo.CurrentCulture), record.Id);
        }

        private void Find(string parameters)
        {
            var args = parameters.Split(' ');
            if (!args.Length.Equals(2) || !args[1].StartsWith('"') || !args[1].EndsWith('"'))
            {
                Console.WriteLine(Program.Rm.GetString("InvalidArgumentsMessage", CultureInfo.CurrentCulture));
                return;
            }

            args[1] = args[1][1..^1];
            ReadOnlyCollection<FileCabinetRecord> subList;
            switch (args[0])
            {
                case "firstname":
                    subList = Program.fileCabinetService.FindByFirstName(args[1]);
                    break;

                case "lastname":
                    subList = Program.fileCabinetService.FindByLastName(args[1]);
                    break;

                case "dateofbirth":
                    DateTime tmpDate = new();

                    var result = new DateTimeConverter().Convert(args[1]);
                    if (result.Item1)
                    {
                        tmpDate = result.Item3;
                    }
                    else
                    {
                        Console.WriteLine(Program.Rm.GetString("InvalidFormatMessage", CultureInfo.CurrentCulture));
                    }

                    subList = Program.fileCabinetService.FindByDate(tmpDate);
                    break;

                default:
                    Console.WriteLine(Program.Rm.GetString("InvalidArgumentsMessage", CultureInfo.CurrentCulture));
                    return;
            }

            if (subList is null)
            {
                Console.WriteLine(Program.Rm.GetString("RecordFindMissMessage", CultureInfo.CurrentCulture));
                return;
            }

            DisplayRecordList(subList);
        }

        private void Create(string parameters)
        {
            this.EnterRecord(out FileCabinetRecord newRecord);
            int recordId = -1;
            try
            {
                recordId = Program.fileCabinetService.CreateRecord(newRecord);
            }
            catch (ArgumentException exeption)
            {
                Console.Write(exeption.Message);
                this.Create(parameters);
            }

            Console.WriteLine(Program.Rm.GetString("CreateRecordMessage", CultureInfo.CurrentCulture), recordId);
        }

        private void Import(string parameters)
        {
            string[] separateParameters = parameters.Split(' ');
            if (!separateParameters.Length.Equals(2))
            {
                Console.WriteLine(Program.Rm.GetString("InvalidArgumentsMessage", CultureInfo.CurrentCulture));
                return;
            }

            var snapshot = new FileCabinetServiceSnapshot();
            string firstParameter = separateParameters[0].ToLower(CultureInfo.CurrentCulture);
            Action<FileStream> reader;
            if (firstParameter == "csv")
            {
                reader = snapshot.LoadFromCsv;
            }
            else if (firstParameter == "xml")
            {
                reader = snapshot.LoadFromXml;
            }
            else
            {
                Console.WriteLine(Program.Rm.GetString("InvalidArgumentsMessage", CultureInfo.CurrentCulture));
                return;
            }

            FileStream stream;
            try
            {
                using (stream = new FileStream(separateParameters[1], FileMode.Open))
                {
                    reader(stream);
                    Program.fileCabinetService.Restore(snapshot);
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(Program.Rm.GetString("FileDoesNotExistMessage", CultureInfo.CurrentCulture));
                return;
            }
            catch (ArgumentException)
            {
                Console.WriteLine(Program.Rm.GetString("СouldТotOpenFile", CultureInfo.CurrentCulture));
                return;
            }
            catch (IOException)
            {
                Console.WriteLine(Program.Rm.GetString("СouldТotOpenFile", CultureInfo.CurrentCulture));
                return;
            }

            Console.WriteLine(Program.Rm.GetString("RecordsWereImport", CultureInfo.CurrentCulture));
        }

        private void Export(string parameters)
        {
            string[] separateParameters = parameters.Split(' ');
            if (!separateParameters.Length.Equals(2))
            {
                Console.WriteLine(Program.Rm.GetString("InvalidArgumentsMessage", CultureInfo.CurrentCulture));
                return;
            }

            if (File.Exists(separateParameters[1]) && !RevriteFileDialod(separateParameters[1]))
            {
                return;
            }

            string secondParameter = separateParameters[0].ToLower(CultureInfo.CurrentCulture);
            if (secondParameter.Equals("csv"))
            {
                try
                {
                    using var writer = new StreamWriter(separateParameters[1]);
                    FileCabinetServiceSnapshot snapshot = Program.fileCabinetService.MakeSnapshot();
                    snapshot.SaveToCsv(writer);
                    Console.WriteLine(Program.Rm.GetString("SuccessfulWriteToFileMessage", CultureInfo.CurrentCulture), separateParameters[1]);
                }
                catch (DirectoryNotFoundException)
                {
                    Console.WriteLine(Program.Rm.GetString("ErrorWriteToFileMessage", CultureInfo.CurrentCulture), separateParameters[1]);
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine(Program.Rm.GetString("ErrorWriteToFileMessage", CultureInfo.CurrentCulture), separateParameters[1]);
                }
            }
            else if (secondParameter.Equals("xml"))
            {
                try
                {
                    using var writer = new StreamWriter(separateParameters[1]);
                    FileCabinetServiceSnapshot snapshot = Program.fileCabinetService.MakeSnapshot();
                    snapshot.SaveToXml(writer);
                    Console.WriteLine(Program.Rm.GetString("SuccessfulWriteToFileMessage", CultureInfo.CurrentCulture), separateParameters[1]);
                }
                catch (DirectoryNotFoundException)
                {
                    Console.WriteLine(Program.Rm.GetString("ErrorWriteToFileMessage", CultureInfo.CurrentCulture), separateParameters[1]);
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine(Program.Rm.GetString("ErrorWriteToFileMessage", CultureInfo.CurrentCulture), separateParameters[1]);
                }
            }
            else
            {
                Console.WriteLine(Program.Rm.GetString("InvalidArgumentsMessage", CultureInfo.CurrentCulture), separateParameters[1]);
            }
        }

        private void Remove(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine(Program.Rm.GetString("InvalidArgumentsMessage", CultureInfo.CurrentCulture));
            }

            var result = new IntConverter().Convert(parameters);
            if (result.Item1)
            {
                try
                {
                    Program.fileCabinetService.Remove(result.Item3);
                    Console.WriteLine(Program.Rm.GetString("RecordIsRemoved", CultureInfo.CurrentCulture), result.Item3);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine(Program.Rm.GetString("RecordDoesNotExists", CultureInfo.CurrentCulture), result.Item3);
                }
            }
            else
            {
                Console.WriteLine(Program.Rm.GetString("InvalidArgumentsMessage", CultureInfo.CurrentCulture));
            }
        }

        private bool YesOrNoDialog(string message)
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

        public override void Handle(AppCommandRequest request)
        {
            var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(request.Command, StringComparison.InvariantCultureIgnoreCase));
            if (index >= 0)
            {

                commands[index].Item2(request.Parameters);
            }
            else
            {
                PrintMissedCommandInfo(request.Command);
            }
        }

        private bool RevriteFileDialod(string fileName)
        {
            var reqestMessage = new StringBuilder();
            reqestMessage.Append("File is exist - rewrite ");
            reqestMessage.Append(fileName);
            reqestMessage.Append(" ?");
            reqestMessage.Append("[Y/n]");
            return this.YesOrNoDialog(reqestMessage.ToString());
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine(Program.Rm.GetString("MissedCommandInfoMessage", CultureInfo.CurrentCulture), command);
            Console.WriteLine();
        }

    }
}

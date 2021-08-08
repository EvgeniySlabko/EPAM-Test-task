using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Main class.
    /// </summary>
    public static class Program
    {
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private const string ConsoleStartSymbol = ">";


        private static readonly string[][] HelpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "_stat_", "prints the record statistics", "The '_stat_' command prints the record statistics." },
            new string[] { "create", "create a new record", "The 'create' command create a new record." },
            new string[] { "_list_", "display list of records", "The '_list_' display list of records." },
            new string[] { "edit", "edit existing record", "The 'edit' edit existing record." },
            new string[] { "find", "find existing record", "The 'find' find existing record." },
            new string[] { "export", "Export in CSV file", "The 'export' export records in CSV file." },
        };

        private static readonly Tuple<string, Action<string>>[] Commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("_stat_", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("_list_", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("export", Export),
        };

        private static readonly Tuple<string, string, Tuple<string, Action>[]>[] CommandLineArguments = new Tuple<string, string, Tuple<string, Action>[]>[]
        {
#pragma warning disable SA1118 // Parameter should not span multiple lines
            new Tuple<string, string, Tuple<string, Action>[]>("--validation-rules", "-v", new Tuple<string, Action>[]
            {
                new Tuple<string, Action>("default", () => validationRule = ValidationRule.Default),
                new Tuple<string, Action>("custom", () => validationRule = ValidationRule.Custom),
            }),

            new Tuple<string, string, Tuple<string, Action>[]>("--storage", "-s", new Tuple<string, Action>[]
            {
                new Tuple<string, Action>("file", () => serviceType = ServiceType.FileService),
                new Tuple<string, Action>("memory", () => serviceType = ServiceType.MemoryService),
            }),
        };
#pragma warning restore SA1118 // Parameter should not span multiple lines

        private static readonly ResourceManager Rm = new ("FileCabinetApp.Resource.Strings", Assembly.GetExecutingAssembly());
        private static IFileCabinetService fileCabinetService;
        private static ValidationRule validationRule = ValidationRule.Default;
        private static ServiceType serviceType = ServiceType.MemoryService;
        private static ValidationRuleSet validationRuleSet;
        private static bool isRunning = true;

        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            Console.WriteLine(Rm.GetString("WelcomeMessage", CultureInfo.CurrentCulture));
            ParseCommandLineArguments(args);
            DisplayValidationRuleMessage();
            Console.WriteLine();

#if DEBUG
            // AddSomeRecords();
#endif

            do
            {
                Console.Write(ConsoleStartSymbol);
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Rm.GetString("HintMessage", CultureInfo.CurrentCulture));
                    continue;
                }

                var index = Array.FindIndex(Commands, 0, Commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    Commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void DisplayValidationRuleMessage()
        {
            switch (validationRule)
            {
                case ValidationRule.Default:
                    Console.WriteLine(Rm.GetString("ValidationRuleString", CultureInfo.CurrentCulture), "default");
                    break;
                case ValidationRule.Custom:
                    Console.WriteLine(Rm.GetString("ValidationRuleString", CultureInfo.CurrentCulture), "custom");
                    break;
            }
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine(Rm.GetString("StatMessage", CultureInfo.CurrentCulture), recordsCount);
        }

        private static void AddSomeRecords()
        {
            var records = new FileCabinetRecord[]
            {
                new FileCabinetRecord
                {
                    Id = 0,
                    FirstName = "Max",
                    LastName = "maxov",
                    DateOfBirth = new DateTime(1991, 6, 14),
                    IdentificationNumber = 324,
                    IdentificationLetter = 'f',
                    PointsForFourTests = 123,
                },
                new FileCabinetRecord
                {
                    Id = 0,
                    FirstName = "Evgeniy",
                    LastName = "Slabko",
                    DateOfBirth = new DateTime(1992, 6, 14),
                    IdentificationNumber = 4323,
                    IdentificationLetter = 'f',
                    PointsForFourTests = 123,
                },
                new FileCabinetRecord
                {
                    Id = 0,
                    FirstName = "Kiril",
                    LastName = "Slabko",
                    DateOfBirth = new DateTime(1993, 6, 14),
                    IdentificationNumber = 342,
                    IdentificationLetter = 'd',
                    PointsForFourTests = 55,
                },
                new FileCabinetRecord
                {
                    Id = 0,
                    FirstName = "Elena",
                    LastName = "Chernaya",
                    DateOfBirth = new DateTime(1999, 6, 14),
                    IdentificationNumber = 4324,
                    IdentificationLetter = 'f',
                    PointsForFourTests = 44,
                },
                new FileCabinetRecord
                {
                    Id = 0,
                    FirstName = "Masha",
                    LastName = "Chernaya",
                    DateOfBirth = new DateTime(1996, 6, 14),
                    IdentificationNumber = 34234,
                    IdentificationLetter = 'r',
                    PointsForFourTests = 23,
                },
                new FileCabinetRecord
                {
                    Id = 0,
                    FirstName = "Nik",
                    LastName = "Arturov",
                    DateOfBirth = new DateTime(1996, 6, 14),
                    IdentificationNumber = 2342,
                    IdentificationLetter = 'k',
                    PointsForFourTests = 34,
                },
                new FileCabinetRecord
                {
                    Id = 0,
                    FirstName = "uram",
                    LastName = "Urumov",
                    DateOfBirth = new DateTime(1991, 6, 14),
                    IdentificationNumber = 3242,
                    IdentificationLetter = 'b',
                    PointsForFourTests = 32,
                },
                new FileCabinetRecord
                {
                    Id = 0,
                    FirstName = "uram",
                    LastName = "Urumov",
                    DateOfBirth = new DateTime(1992, 6, 14),
                    IdentificationNumber = 33,
                    IdentificationLetter = 'h',
                    PointsForFourTests = 1,
                },
                new FileCabinetRecord
                {
                    Id = 0,
                    FirstName = "uram",
                    LastName = "Urumov",
                    DateOfBirth = new DateTime(1992, 6, 14),
                    IdentificationNumber = 33,
                    IdentificationLetter = 'h',
                    PointsForFourTests = 1,
                },
            };

            foreach (var record in records)
            {
                fileCabinetService.CreateRecord(record);
            }
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine(Rm.GetString("MissedCommandInfoMessage", CultureInfo.CurrentCulture), command);
            Console.WriteLine();
        }

        private static void ParseCommandLineArguments(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (args.Length.Equals(0))
            {
                return;
            }

            bool wasArgumentType = false;
            int argumentIndex = 0;
            foreach (var arg in args)
            {
                var lowerArg = arg.ToLower(CultureInfo.CurrentCulture);
                if (wasArgumentType)
                {
                    var index = Array.FindIndex(CommandLineArguments[argumentIndex].Item3, 0, CommandLineArguments[argumentIndex].Item3.Length, i => i.Item1.Equals(lowerArg, StringComparison.CurrentCulture));
                    if (index != -1)
                    {
                        wasArgumentType = false;
                        CommandLineArguments[argumentIndex].Item3[index].Item2();
                        continue;
                    }
                }
                else if (lowerArg.StartsWith("--", StringComparison.CurrentCulture) && !wasArgumentType)
                {
                    var splitedArg = lowerArg.Split('=');
                    if (splitedArg.Length == 2)
                    {
                        var index = Array.FindIndex(CommandLineArguments, 0, CommandLineArguments.Length, i => i.Item1.Equals(splitedArg[0], StringComparison.CurrentCulture));
                        if (index != -1)
                        {
                            var index2 = Array.FindIndex(CommandLineArguments[index].Item3, 0, CommandLineArguments[index].Item3.Length, i => i.Item1.Equals(splitedArg[1], StringComparison.CurrentCulture));
                            if (index2 != -1)
                            {
                                CommandLineArguments[index].Item3[index2].Item2();
                                continue;
                            }
                        }
                    }
                }
                else if (lowerArg.StartsWith("-", StringComparison.CurrentCulture) && !wasArgumentType)
                {
                    argumentIndex = Array.FindIndex(CommandLineArguments, 0, CommandLineArguments.Length, i => i.Item2.Equals(lowerArg, StringComparison.CurrentCulture));
                    if (argumentIndex != -1)
                    {
                        wasArgumentType = true;
                        continue;
                    }
                }

                throw new ArgumentException(Rm.GetString("UnableCommandLineArgumentsMessage", CultureInfo.CurrentCulture));
            }

            ApplyCommandLineArguments();
        }

        private static void List(string parameters)
        {
            DisplayRecordList(fileCabinetService.GetRecords());
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine(Rm.GetString("CommandExplanationMessage", CultureInfo.CurrentCulture), parameters);
                }
            }
            else
            {
                Console.WriteLine(Rm.GetString("AvailableCommandsMessage", CultureInfo.CurrentCulture));

                foreach (var helpMessage in HelpMessages)
                {
                    Console.WriteLine(Rm.GetString("HelpMessageTemplate", CultureInfo.CurrentCulture), helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void EnterRecord(out FileCabinetRecord newRecord)
        {
            newRecord = new FileCabinetRecord();
            Console.WriteLine();
            Console.Write(Rm.GetString("FirstNameMessage", CultureInfo.CurrentCulture));
            newRecord.FirstName = ReadInput(new StringConverter().GetDelegate(), validationRuleSet.FirstNameVAidator.GetDelegate());

            Console.Write(Rm.GetString("LastNameMessage", CultureInfo.CurrentCulture));
            newRecord.LastName = ReadInput(new StringConverter().GetDelegate(), validationRuleSet.LastNameValidator.GetDelegate());

            Console.Write(Rm.GetString("DateOfBirthMessage", CultureInfo.CurrentCulture));
            newRecord.DateOfBirth = ReadInput(new DateTimeConverter().GetDelegate(), validationRuleSet.DateValidator.GetDelegate());

            Console.Write(Rm.GetString("IdentificationNumberMessage", CultureInfo.CurrentCulture));
            newRecord.IdentificationNumber = ReadInput(new DecimalConverter().GetDelegate(), validationRuleSet.IdentificationNumberValidator.GetDelegate());

            Console.Write(Rm.GetString("IdentificationLetterMessage", CultureInfo.CurrentCulture));
            newRecord.IdentificationLetter = ReadInput(new CharConverter().GetDelegate(), validationRuleSet.IdentificationLetterValidator.GetDelegate());

            Console.Write(Rm.GetString("PointsForFourTestsMessage", CultureInfo.CurrentCulture));
            newRecord.PointsForFourTests = ReadInput(new ShortConverter().GetDelegate(), validationRuleSet.PointsForFourTestsValidator.GetDelegate());
        }

        private static void DisplayRecordList(ReadOnlyCollection<FileCabinetRecord> records)
        {
            if (records.Count.Equals(0))
            {
                Console.WriteLine(Rm.GetString("EmptyListMessage", CultureInfo.CurrentCulture));
            }

            foreach (var record in records)
            {
                Console.WriteLine(Rm.GetString("RecordInfoString", CultureInfo.CurrentCulture), record.Id, record.FirstName, record.LastName, record.DateOfBirth.ToString("yyyy-MMM-dd", DateTimeFormatInfo.InvariantInfo), record.IdentificationNumber, record.IdentificationLetter, record.PointsForFourTests);
            }
        }

        private static string[] SplitParameterString(string parameters)
        {
            return parameters.Split(' ');
        }

        private static void Find(string parameters)
        {
            var args = parameters.Split(' ');
            if (!args.Length.Equals(2) || !args[1].StartsWith('"') || !args[1].EndsWith('"'))
            {
                Console.WriteLine(Rm.GetString("InvalidArgumentsMessage", CultureInfo.CurrentCulture));
                return;
            }

            args[1] = args[1][1..^1];
            ReadOnlyCollection<FileCabinetRecord> subList;
            switch (args[0])
            {
                case "firstname":
                    subList = fileCabinetService.FindByFirstName(args[1]);
                    break;

                case "lastname":
                    subList = fileCabinetService.FindByLastName(args[1]);
                    break;

                case "dateofbirth":
                    DateTime tmpDate = new ();

                    var result = new DateTimeConverter().Convert(args[1]);
                    if (result.Item1)
                    {
                        tmpDate = result.Item3;
                    }
                    else
                    {
                        Console.WriteLine(Rm.GetString("InvalidFormatMessage", CultureInfo.CurrentCulture));
                    }

                    subList = fileCabinetService.FindByDate(tmpDate);
                    break;

                default:
                    Console.WriteLine(Rm.GetString("InvalidArgumentsMessage", CultureInfo.CurrentCulture));
                    return;
            }

            if (subList is null)
            {
                Console.WriteLine(Rm.GetString("RecordFindMissMessage", CultureInfo.CurrentCulture));
                return;
            }

            DisplayRecordList(subList);
        }

        private static void Create(string parameters)
        {
            EnterRecord(out FileCabinetRecord newRecord);
            int recordId = -1;
            try
            {
                recordId = fileCabinetService.CreateRecord(newRecord);
            }
            catch (ArgumentException exeption)
            {
                Console.Write(exeption.Message);
                Create(parameters);
            }

            Console.WriteLine(Rm.GetString("CreateRecordMessage", CultureInfo.CurrentCulture), recordId);
        }

        private static void Export(string parameters)
        {
            string[] separateParameters = SplitParameterString(parameters);
            if (!separateParameters.Length.Equals(2))
            {
                Console.WriteLine(Rm.GetString("InvalidArgumentsMessage", CultureInfo.CurrentCulture));
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
                    FileCabinetServiceSnapshot snapshot = fileCabinetService.MakeSnapshot();
                    snapshot.SaveToCsv(writer);
                    Console.WriteLine(Rm.GetString("SuccessfulWriteToFileMessage", CultureInfo.CurrentCulture), separateParameters[1]);
                }
                catch (DirectoryNotFoundException)
                {
                    Console.WriteLine(Rm.GetString("ErrorWriteToFileMessage", CultureInfo.CurrentCulture), separateParameters[1]);
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine(Rm.GetString("ErrorWriteToFileMessage", CultureInfo.CurrentCulture), separateParameters[1]);
                }
            }
            else if (secondParameter.Equals("xml"))
            {
                try
                {
                    using var writer = new StreamWriter(separateParameters[1]);
                    FileCabinetServiceSnapshot snapshot = fileCabinetService.MakeSnapshot();
                    snapshot.SaveToXml(writer);
                    Console.WriteLine(Rm.GetString("SuccessfulWriteToFileMessage", CultureInfo.CurrentCulture), separateParameters[1]);
                }
                catch (DirectoryNotFoundException)
                {
                    Console.WriteLine(Rm.GetString("ErrorWriteToFileMessage", CultureInfo.CurrentCulture), separateParameters[1]);
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine(Rm.GetString("ErrorWriteToFileMessage", CultureInfo.CurrentCulture), separateParameters[1]);
                }
            }
            else
            {
                Console.WriteLine(Rm.GetString("InvalidArgumentsMessage", CultureInfo.CurrentCulture), separateParameters[1]);
            }
        }

        private static bool RevriteFileDialod(string fileName)
        {
            var reqestMessage = new StringBuilder();
            reqestMessage.Append("File is exist - rewrite ");
            reqestMessage.Append(fileName);
            reqestMessage.Append(" ?");
            reqestMessage.Append("[Y/n]");
            return YesOrNoDialog(reqestMessage.ToString());
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine(Rm.GetString("ExitMessage", CultureInfo.CurrentCulture));
            isRunning = false;
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

        private static void ApplyCommandLineArguments()
        {
            switch (validationRule)
            {
                case ValidationRule.Default:
                    validationRuleSet = ValidationRuleSetMaker.MakeDefaultValidationSet();
                    break;

                case ValidationRule.Custom:
                    validationRuleSet = ValidationRuleSetMaker.MakeCustomValidationSet();
                    break;
            }

            var serviceValidator = new ServiceValidator(validationRuleSet);
            switch (serviceType)
            {
                case ServiceType.MemoryService:
                    fileCabinetService = new FileCabinetMemoryService(serviceValidator);
                    break;

                case ServiceType.FileService:
                    fileCabinetService = new FileCabinetFilesystemService(serviceValidator);
                    break;
            }
        }

        private static void Edit(string parameters)
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
                fileCabinetService.Edit(record);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine(Rm.GetString("UppdateRecordMessage", CultureInfo.CurrentCulture), record.Id);
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
                    Console.WriteLine(Rm.GetString("ConversationFailedMessage", CultureInfo.CurrentCulture), conversionResult.Item2);
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine(Rm.GetString("ValidationFailedMessage", CultureInfo.CurrentCulture), validationResult.Item2);
                    continue;
                }

                return value;
            }
            while (true);
        }
    }
}
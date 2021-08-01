using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace FileCabinetApp
{
    /// <summary>
    /// Main class.
    /// </summary>
    public static class Program
    {
        private const string ConsoleStartSymbol = ">";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private static ResourceManager rm = new ("FileCabinetApp.Resource.Strings", Assembly.GetExecutingAssembly());
        private static ValidationRule validationRule;
        private static bool isRunning = true;
        private static IFileCabinetService fileCabinetService;
        private static ValidationRuleSet validationRuleSet;
        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("_stat_", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("_list_", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "_stat_", "prints the record statistics", "The '_stat_' command prints the record statistics." },
            new string[] { "create", "create a new record", "The 'create' command create a new record." },
            new string[] { "_list_", "display list of records", "The '_list_' display list of records." },
            new string[] { "edit", "edit existing record", "The 'edit' edit existing record." },
            new string[] { "find", "find existing record", "The 'find' find existing record." },
        };

        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            Console.WriteLine(rm.GetString("WelcomeMessage", CultureInfo.CurrentCulture));
            ParseCommandLineArguments(args);
            DisplayValidationRuleMessage();
            Console.WriteLine();

#if DEBUG
            AddSomeRecords();
#endif

            do
            {
                Console.Write(ConsoleStartSymbol);
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(rm.GetString("HintMessage", CultureInfo.CurrentCulture));
                    continue;
                }

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
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
                    Console.WriteLine(rm.GetString("ValidationRuleString", CultureInfo.CurrentCulture), "default");
                    break;
                case ValidationRule.Custom:
                    Console.WriteLine(rm.GetString("ValidationRuleString", CultureInfo.CurrentCulture), "custom");
                    break;
            }
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void MakeDefaultValidationSet()
        {
            validationRuleSet = new ValidationRuleSet
            {
                DateValidator = new DateValidator(new DateTime(1960), DateTime.Now),
                FirstNameVAidator = new StringValidator(40, 2),
                LastNameValidator = new StringValidator(40, 2),
                PointsForFourTestsValidator = new ShortValidator(0, 400),
                IdentificationNumberValidator = new DecimalValidator(0, decimal.MaxValue),
                IdentificationLetterValidator = new CharValidator((c) => char.IsLetter(c)),
            };
        }

        private static void MakeCustomValidationSet()
        {
            validationRuleSet = new ValidationRuleSet
            {
                DateValidator = new DateValidator(new DateTime(1960), DateTime.Now),
                FirstNameVAidator = null,
                LastNameValidator = null,
                PointsForFourTestsValidator = new ShortValidator(0, 400),
                IdentificationNumberValidator = null,
                IdentificationLetterValidator = new CharValidator((c) => char.IsLetter(c)),
            };
        }

        private static void AddSomeRecords()
        {
            FileCabinetRecord[] records = new FileCabinetRecord[]
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
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void SetValidator(string validator = null)
        {
            if (validator is null)
            {
                MakeDefaultValidationSet();
            }
            else
            {
                switch (validator.ToLower(CultureInfo.CurrentCulture))
                {
                    case "default":

                        MakeDefaultValidationSet();
                        validationRule = ValidationRule.Default;
                        break;
                    case "custom":
                        MakeCustomValidationSet();
                        validationRule = ValidationRule.Custom;
                        break;
                    default:
                        throw new ArgumentException("Unable command line arguments");
                }
            }

            fileCabinetService = new FileCabinetService(new ServiceValidator(validationRuleSet));
        }

        private static void ParseCommandLineArguments(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (args.Length == 0)
            {
                SetValidator();
                return;
            }

            if (args[0] == "-v")
            {
                if (args.Length != 2)
                {
                    throw new ArgumentNullException(nameof(args));
                }

                SetValidator(args[1]);
            }
            else
            {
                string[] ruleArgument = args[0].Split('=');
                if (ruleArgument.Length == 2)
                {
                    SetValidator(ruleArgument[1]);
                }
                else if (ruleArgument[0] == "--validation-rules")
                {
                    SetValidator(ruleArgument[1]);
                }
                else
                {
                    throw new ArgumentException("Unable command line arguments");
                }
            }
        }

        private static void List(string parameters)
        {
            var list = fileCabinetService.GetRecords();
            DisplayRecordList(list);
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine(rm.GetString("AvailableCommandsMessage", CultureInfo.CurrentCulture));

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine(rm.GetString("HelpMessageTemplate", CultureInfo.CurrentCulture), helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void EnterRecord(out FileCabinetRecord newRecord)
        {
            newRecord = new FileCabinetRecord();
            Console.WriteLine();
            Console.Write(rm.GetString("FirstNameMessage", CultureInfo.CurrentCulture));
            newRecord.FirstName = ReadInput(new StringConverter().GetDelegate(), validationRuleSet.FirstNameVAidator.GetDelegate());

            Console.Write(rm.GetString("LastNameMessage", CultureInfo.CurrentCulture));
            newRecord.LastName = ReadInput(new StringConverter().GetDelegate(), validationRuleSet.LastNameValidator.GetDelegate());

            Console.Write(rm.GetString("DateOfBirthMessage", CultureInfo.CurrentCulture));
            newRecord.DateOfBirth = ReadInput(new DateTimeConverter().GetDelegate(), validationRuleSet.DateValidator.GetDelegate());

            Console.Write(rm.GetString("IdentificationNumberMessage", CultureInfo.CurrentCulture));
            newRecord.IdentificationNumber = ReadInput(new DecimalConverter().GetDelegate(), validationRuleSet.IdentificationNumberValidator.GetDelegate());

            Console.Write(rm.GetString("IdentificationLetterMessage", CultureInfo.CurrentCulture));
            newRecord.IdentificationLetter = ReadInput(new CharConverter().GetDelegate(), validationRuleSet.IdentificationLetterValidator.GetDelegate());

            Console.Write(rm.GetString("PointsForFourTestsMessage", CultureInfo.CurrentCulture));
            newRecord.PointsForFourTests = ReadInput(new ShortConverter().GetDelegate(), validationRuleSet.PointsForFourTestsValidator.GetDelegate());
        }

        private static void DisplayRecordList(ReadOnlyCollection<FileCabinetRecord> records)
        {
            if (records.Count == 0)
            {
                Console.WriteLine(rm.GetString("EmptyListMessage", CultureInfo.CurrentCulture));
            }

            foreach (var record in records)
            {
                Console.WriteLine(rm.GetString("RecordInfoString", CultureInfo.CurrentCulture), record.Id, record.FirstName, record.LastName, record.DateOfBirth.ToString("yyyy-MMM-dd", DateTimeFormatInfo.InvariantInfo), record.IdentificationNumber, record.IdentificationLetter, record.PointsForFourTests);
            }
        }

        private static void Find(string parameters)
        {
            string[] args = parameters.Split(' ');
            if (args.Length != 2 || !args[1].StartsWith('"') || !args[1].EndsWith('"'))
            {
                Console.WriteLine(rm.GetString("InvalidArgumentsMessage", CultureInfo.CurrentCulture));
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
                    try
                    {
                        tmpDate = Convert.ToDateTime(args[1], CultureInfo.CurrentCulture);
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine(rm.GetString("InvalidFormatMessage", CultureInfo.CurrentCulture));
                        return;
                    }

                    subList = fileCabinetService.FindByDate(tmpDate);
                    break;

                default:
                    Console.WriteLine(rm.GetString("InvalidArgumentsMessage", CultureInfo.CurrentCulture));
                    return;
            }

            if (subList is null)
            {
                Console.WriteLine(rm.GetString("RecordFindMissMessage", CultureInfo.CurrentCulture));
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

            Console.WriteLine(rm.GetString("CreateRecordMessage", CultureInfo.CurrentCulture), recordId);
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine(rm.GetString("ExitMessage", CultureInfo.CurrentCulture));
            isRunning = false;
        }

        private static void Edit(string parameters)
        {
            int id;
            try
            {
                id = int.Parse(parameters, CultureInfo.CurrentCulture);
            }
            catch (FormatException)
            {
                Console.WriteLine(rm.GetString("InvalidFormatMessage", CultureInfo.CurrentCulture));
                return;
            }
            catch (OverflowException)
            {
                Console.WriteLine(rm.GetString("OverflowMessage", CultureInfo.CurrentCulture));
                return;
            }

            FileCabinetRecord editRecord;
            try
            {
                EnterRecord(out editRecord);
            }
            catch (ArgumentException exeption)
            {
                Console.WriteLine(exeption.Message);
                return;
            }

            editRecord.Id = id;
            try
            {
                fileCabinetService.Edit(editRecord);
            }
            catch (ArgumentException exeption)
            {
                Console.WriteLine(exeption.Message);
                return;
            }

            Console.WriteLine(rm.GetString("UppdateRecordMessage", CultureInfo.CurrentCulture), id);
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
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }
    }
}
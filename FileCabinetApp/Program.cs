using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "Evgeniy Slabko";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static bool isRunning = true;
        private static FileCabinetService fileCabinetService = new ();

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

        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
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

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void List(string parameters)
        {
            FileCabinetRecord[] list = fileCabinetService.GetRecords();
            foreach (var record in list)
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth.ToString("yyyy-MMM-dd", DateTimeFormatInfo.InvariantInfo)}, (Personal number: {record.IdentificationNumber}{record.IdentificationLetter}, total points: {record.PointsForFourTests})");
            }
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
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static bool EnterRecord(out FileCabinetRecord inputRecord)
        {
            inputRecord = new FileCabinetRecord();
            string enteredData;

            Console.WriteLine();
            Console.Write($"First name: ");
            inputRecord.FirstName = Console.ReadLine();

            Console.Write($"Last name: ");
            inputRecord.LastName = Console.ReadLine();

            Console.Write($"Date of birth: ");
            enteredData = Console.ReadLine();

            try
            {
                inputRecord.DateOfBirth = Convert.ToDateTime(enteredData, CultureInfo.CurrentCulture);
            }
            catch (FormatException)
            {
                Console.WriteLine($"Invalid date format");
                return false;
            }

            Console.Write($"Points For Four Tests: ");
            enteredData = Console.ReadLine();
            try
            {
                inputRecord.PointsForFourTests = short.Parse(enteredData, CultureInfo.CurrentCulture);
            }
            catch (FormatException)
            {
                Console.WriteLine($"{nameof(inputRecord.PointsForFourTests)} invalid format");
                return false;
            }
            catch (OverflowException)
            {
                Console.WriteLine($"{nameof(inputRecord.PointsForFourTests)} overflow");
                return false;
            }

            Console.Write($"Identification Letter: ");
            enteredData = Console.ReadLine();
            try
            {
                inputRecord.IdentificationLetter = char.Parse(enteredData);
            }
            catch (FormatException)
            {
                Console.WriteLine($"{nameof(enteredData)} invalid format");
                return false;
            }

            Console.Write($"Identification Number: ");
            enteredData = Console.ReadLine();
            try
            {
                inputRecord.IdentificationNumber = decimal.Parse(enteredData, CultureInfo.CurrentCulture);
            }
            catch (FormatException)
            {
                Console.WriteLine($"{nameof(inputRecord.IdentificationNumber)} invalid format");
                return false;
            }
            catch (OverflowException)
            {
                Console.WriteLine($"{nameof(inputRecord.IdentificationNumber)} over flow");
                return false;
            }

            return true;
        }

        private static void DisplayList(FileCabinetRecord[] records)
        {
            foreach (var record in records)
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth.ToString("yyyy-MMM-dd", DateTimeFormatInfo.InvariantInfo)}, (Personal number: {record.IdentificationNumber}{record.IdentificationLetter}, total points: {record.PointsForFourTests})");
            }
        }

        private static void Find(string parameters)
        {
            string[] args = parameters.Split(' ');
            if (args.Length != 2 || !args[1].StartsWith('"') || !args[1].EndsWith('"'))
            {
                Console.WriteLine("Invalid arguments");
                return;
            }

            args[1] = args[1][1..^1];
            FileCabinetRecord[] subList;
            switch (args[0])
            {
                case "firstname":
                    subList = fileCabinetService.FindByFirstName(args[1]);
                    break;

                case "lastname":
                    subList = fileCabinetService.FindByLastName(args[1]);
                    break;

                default:
                    Console.WriteLine("Invalid arguments");
                    return;
            }

            DisplayList(subList);
        }

        private static void Create(string parameters)
        {
            if (!EnterRecord(out FileCabinetRecord inputRecord))
            {
                return;
            }

            int recordId = -1;
            try
            {
                recordId = fileCabinetService.CreateRecord(inputRecord);
            }
            catch (ArgumentException exeption)
            {
                Console.Write(exeption);
                Create(string.Empty);
            }

            Console.WriteLine($"Record #{recordId} is created.");
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
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
                Console.WriteLine("Format error. Enter valid id");
                return;
            }
            catch (OverflowException)
            {
                Console.WriteLine("Overflow error. Enter valid id");
                return;
            }

            if (!EnterRecord(out FileCabinetRecord editRecord))
            {
                return;
            }

            editRecord.Id = id;
            try
            {
                fileCabinetService.Edit(editRecord);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            Console.WriteLine("Record #{id} is updated.");
        }
    }
}
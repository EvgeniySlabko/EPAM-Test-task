using System;
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
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "_stat_", "prints the record statistics", "The '_stat_' command prints the record statistics." },
            new string[] { "create", "create a new record", "The 'create' command create a new record." },
            new string[] { "_list_", "display list of records", "The '_list_' display list of records." },
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
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth.ToString("yyyy-MMM-dd", DateTimeFormatInfo.InvariantInfo)}, ({record.ShortProperty}, {record.DecimalProperty}, {record.CharProperty})");
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

        private static void Create(string parameters)
        {
            string enteredFirstName;
            string enteredLastName;
            string enteredDateOfBirth;

            string enteredShortProperty;
            string enteredDecimalProperty;
            string enteredCharProperty;
            short shortProperty;
            char charProperty;
            decimal decimalProperty;

            DateTime dateOfBirth = new ();
            Console.Write($"First name: ");
            enteredFirstName = Console.ReadLine();
            if (string.IsNullOrEmpty(enteredFirstName))
            {
                Console.WriteLine($"Invalid first name. Enter a non-empty string");
                return;
            }

            Console.Write($"Last name: ");
            enteredLastName = Console.ReadLine();
            if (string.IsNullOrEmpty(enteredLastName))
            {
                Console.WriteLine($"Invalid last name. Enter a non-empty string");
                return;
            }

            Console.Write($"Date of birth: ");
            enteredDateOfBirth = Console.ReadLine();
            try
            {
                dateOfBirth = Convert.ToDateTime(enteredDateOfBirth, CultureInfo.CurrentCulture);
            }
            catch (FormatException)
            {
                Console.WriteLine($"Invalid date format");
                return;
            }

            Console.Write($"Short property: ");
            enteredShortProperty = Console.ReadLine();
            try
            {
                shortProperty = short.Parse(enteredShortProperty, CultureInfo.CurrentCulture);
            }
            catch (FormatException)
            {
                Console.WriteLine($"{nameof(enteredShortProperty)} invalid format");
                return;
            }
            catch (OverflowException)
            {
                Console.WriteLine($"{nameof(enteredShortProperty)} over flow");
                return;
            }

            Console.Write($"Char property: ");
            enteredCharProperty = Console.ReadLine();
            try
            {
                charProperty = char.Parse(enteredCharProperty);
            }
            catch (FormatException)
            {
                Console.WriteLine($"{nameof(enteredCharProperty)} invalid format");
                return;
            }

            Console.Write($"Decimal property: ");
            enteredDecimalProperty = Console.ReadLine();
            try
            {
                decimalProperty = decimal.Parse(enteredDecimalProperty, CultureInfo.CurrentCulture);
            }
            catch (FormatException)
            {
                Console.WriteLine($"{nameof(enteredDecimalProperty)} invalid format");
                return;
            }
            catch (OverflowException)
            {
                Console.WriteLine($"{nameof(enteredDecimalProperty)} over flow");
                return;
            }

            int id = fileCabinetService.CreateRecord(enteredFirstName, enteredLastName, dateOfBirth, shortProperty, decimalProperty, charProperty);
            Console.WriteLine($"Record #{id} is created.");
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }
    }
}
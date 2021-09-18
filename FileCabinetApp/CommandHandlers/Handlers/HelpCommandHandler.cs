using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Handler for help command.
    /// </summary>
    public class HelpCommandHandler : CommandHandlerBase
    {
        private const string Command = "help";
        private const int DescriptionHelpIndex = 1;
        private const int CommandHelpIndex = 0;
        private const int ExplanationHelpIndex = 2;

        public static string[][] HelpMessages = new string[][]
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
            new string[] { "remove", "remove record from service", "The 'remove' remove record from service." },
            new string[] { "insert", "insert record to service", "The 'insert' insert record to service." },
            new string[] { "delete", "delete records from service", "The 'delete' delete compliant records." },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpCommandHandler"/> class.
        /// </summary>
        public HelpCommandHandler()
            : base(Command)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (this.CheckCommand(commandRequest) && string.IsNullOrEmpty(commandRequest.Parameters))
            {
                PrintHelp(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine(StringManager.Rm.GetString("CommandExplanationMessage", CultureInfo.CurrentCulture), parameters);
                }
            }
            else
            {
                Console.WriteLine(StringManager.Rm.GetString("AvailableCommandsMessage", CultureInfo.CurrentCulture));

                foreach (var helpMessage in HelpMessages)
                {
                    Console.WriteLine(StringManager.Rm.GetString("HelpMessageTemplate", CultureInfo.CurrentCulture), helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }
    }
}
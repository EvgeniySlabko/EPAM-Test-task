using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Handler for help command.
    /// </summary>
    public class HelpCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Commands and its description.
        /// </summary>
        public static readonly string[][] HelpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints the record statistics", "The 'stat' command prints the record statistics." },
            new string[] { "create", "create a new record", "The 'create' command create a new record." },
            new string[]
            {
                "select", "select records", "shows specific fields of records that match a condition." +
                $"{Environment.NewLine}Example: select firstname, lastname, dateofbirth, letter where letter = 'f' or letter = 'g' and dateofbirth = '04/19/1999'",
            },

            new string[]
            {
                "update", "update record", "Uppdate records using expressions match a condition." +
                $"{Environment.NewLine}Example: update set firstname = 'John', lastname = 'Doe' , dateofbirth = '06/06/2006' where id = '1' or id = '2' or id = '3' or id = '4'",
            },

            new string[]
            {
                "insert", "insert record to service", "The 'insert' insert record to service." +
                 $"{Environment.NewLine}Example: insert (id, firstname, lastname, dateofbirth, letter, identificationnumber, points) values ('77', 'John', 'Doe', '5/18/1986', 'h', '555', '6')",
            },
            new string[]
            {
                "delete", "delete records from service", "The 'delete' delete compliant records." +
                $"{Environment.NewLine}Example: delete where id = '1'",
            },
            new string[] { "export", "Export in csv or xml file", "The 'export' export records in csv or xml file." },
            new string[] { "import", "Import records from csv or xml file", "The 'import' import records from csv or xml file." },
            new string[] { "purge", "clear filesystem", "The 'purge' command clear filesystem. Use only in FileCabinetFilesystemService." },
        };

        private const string Command = "help";
        private const int DescriptionHelpIndex = 1;
        private const int CommandHelpIndex = 0;
        private const int ExplanationHelpIndex = 2;

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
            if (this.CheckCommand(commandRequest))
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
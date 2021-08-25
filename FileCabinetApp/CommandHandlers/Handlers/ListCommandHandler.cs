using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Handler for list command.
    /// </summary>
    public class ListCommandHandler : CommandHandlerBase
    {
        private const string Command = "list";

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        public ListCommandHandler()
            : base(Command)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (this.CheckCommand(commandRequest) && string.IsNullOrEmpty(commandRequest.Parameters))
            {
                List();
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static void List()
        {
            var records = Program.fileCabinetService.GetRecords();
            ConsoleHelper.DisplayRecordList(records);
        }
    }
}

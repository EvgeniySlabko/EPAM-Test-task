using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Handler for list command.
    /// </summary>
    public class ListCommandHandler : FileCabinetServiceCommandHandlerBase
    {
        private const string Command = "list";

        private readonly Action<IEnumerable<FileCabinetRecord>> printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service.</param>
        /// <param name="printer">Printer.</param>
        public ListCommandHandler(IFileCabinetService service, Action<IEnumerable<FileCabinetRecord>> printer)
            : base(Command, service)
        {
            this.printer = printer;
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (this.CheckCommand(commandRequest) && string.IsNullOrEmpty(commandRequest.Parameters))
            {
                this.List();
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void List()
        {
            var iter = this.Service.GetRecords();
            this.printer(iter);
        }
    }
}

using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Handler for exit command.
    /// </summary>
    public class ExitCommandHandler : CommandHandlerBase
    {
        private const string Command = "exit";

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
        /// </summary>
        public ExitCommandHandler()
            : base(Command)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (this.CheckCommand(commandRequest) && string.IsNullOrEmpty(commandRequest.Parameters))
            {
                Console.WriteLine(Program.Rm.GetString("ExitMessage", CultureInfo.CurrentCulture));
                Program.isRunning = false;
            }
            else
            {
                base.Handle(commandRequest);
            }
        }
    }
}

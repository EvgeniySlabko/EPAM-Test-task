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

        private readonly Action<bool> stopProgram;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
        /// </summary>
        /// <param name="stopProgram">Stop program action.</param>
        public ExitCommandHandler(Action<bool> stopProgram)
            : base(Command)
        {
            this.stopProgram = stopProgram;
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (this.CheckCommand(commandRequest) && string.IsNullOrEmpty(commandRequest.Parameters))
            {
                Console.WriteLine(StringManager.Rm.GetString("ExitMessage", CultureInfo.CurrentCulture));
                this.stopProgram(false);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }
    }
}

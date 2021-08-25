using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Base class for command handler.
    /// </summary>
    public class CommandHandlerBase : ICommandHandler
    {
        private readonly string command;

        private ICommandHandler commandHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandlerBase"/> class.
        /// </summary>
        /// <param name="command">Command.</param>
        protected CommandHandlerBase(string command)
        {
            this.command = command;
        }

        /// <summary>
        /// Handle.
        /// </summary>
        /// <param name="commandRequest">Command handler.</param>
        public virtual void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest));
            }

            if (this.commandHandler is not null)
            {
                this.commandHandler.Handle(commandRequest);
            }
            else
            {
                PrintMissedCommandInfo(commandRequest.Command);
            }
        }

        /// <summary>
        /// Set next command.
        /// </summary>
        /// <param name="commandHandler">Command handler.</param>
        /// <returns>Next command.</returns>
        public ICommandHandler SetNext(ICommandHandler commandHandler)
        {
            this.commandHandler = commandHandler;
            return this.commandHandler;
        }

        /// <summary>
        /// Common command processing.
        /// </summary>
        /// <param name="commandRequest">Given command reqest.</param>
        /// <returns>Result.</returns>
        protected bool CheckCommand(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest));
            }

            return commandRequest.Command.Equals(this.command);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine(Program.Rm.GetString("MissedCommandInfoMessage", CultureInfo.CurrentCulture), command);
            Console.WriteLine();
        }
    }
}

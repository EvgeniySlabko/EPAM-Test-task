using System;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Base class for command handler.
    /// </summary>
    public abstract class CommandHandlerBase : ICommandHandler
    {
        private ICommandHandler commandHandler;

        /// <summary>
        /// Handle.
        /// </summary>
        /// <param name="commandRequest">Command handler.</param>
        public virtual void Handle(AppCommandRequest commandRequest)
        {

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
    }
}

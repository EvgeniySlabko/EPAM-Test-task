using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// handler for remove command.
    /// </summary>
    public class RemoveCommandHandler : CommandHandlerBase
    {
        private const string Command = "remove";

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveCommandHandler"/> class.
        /// </summary>
        public RemoveCommandHandler()
            : base(Command)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (!this.CheckCommand(commandRequest))
            {
                base.Handle(commandRequest);
                return;
            }

            var result = new IntConverter().Convert(commandRequest.Parameters);
            if (result.Item1)
            {
                Remove(result.Item3);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static void Remove(int id)
        {
            try
            {
                Program.fileCabinetService.Remove(id);
                Console.WriteLine(Program.Rm.GetString("RecordIsRemoved", CultureInfo.CurrentCulture), id);
            }
            catch (ArgumentException)
            {
                Console.WriteLine(Program.Rm.GetString("RecordDoesNotExists", CultureInfo.CurrentCulture), id);
            }
        }
    }
}

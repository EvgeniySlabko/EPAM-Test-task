using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// handler for remove command.
    /// </summary>
    public class RemoveCommandHandler : FileCabinetServiceCommandHandlerBase
    {
        private const string Command = "remove";

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service.</param>
        public RemoveCommandHandler(IFileCabinetService service)
            : base(Command, service)
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

            var result = Converter.Convert<int>(commandRequest.Parameters);
            if (result.Item1)
            {
                this.Remove(result.Item3);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Remove(int id)
        {
            try
            {
                this.Service.Remove(id);
                Console.WriteLine(StringManager.Rm.GetString("RecordIsRemoved", CultureInfo.CurrentCulture), id);
            }
            catch (ArgumentException)
            {
                Console.WriteLine(StringManager.Rm.GetString("RecordDoesNotExists", CultureInfo.CurrentCulture), id);
            }
        }
    }
}

using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Handler for Stat command.
    /// </summary>
    public class PurgeCommandHandler : FileCabinetServiceCommandHandlerBase
    {
        private const string Command = "purge";

        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service.</param>
        public PurgeCommandHandler(IFileCabinetService service)
            : base(Command, service)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (this.CheckCommand(commandRequest))
            {
                if (string.IsNullOrEmpty(commandRequest.Parameters))
                {
                    this.Purge();
                }
                else
                {
                    Console.WriteLine(StringManager.Rm.GetString("InvalidArgumentsMessage", CultureInfo.CurrentCulture));
                }
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Purge()
        {
            var purgedRecors = this.Service.Purge();
            Console.WriteLine(StringManager.Rm.GetString("RecordsWerePurgedMessage", CultureInfo.CurrentCulture), purgedRecors);
        }
    }
}

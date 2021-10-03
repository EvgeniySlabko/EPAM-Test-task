using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Handler for Stat command.
    /// </summary>
    public class StatCommandHandler : FileCabinetServiceCommandHandlerBase
    {
        private const string Command = "stat";

        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service.</param>
        public StatCommandHandler(IFileCabinetService service)
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
                    this.Stat();
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

        private void Stat()
        {
            var result = this.Service.GetStat();
            Console.WriteLine(StringManager.Rm.GetString("StatMessage", CultureInfo.CurrentCulture), result.Item1, result.Item2);
        }
    }
}

using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Handler for Stat command.
    /// </summary>
    public class StatCommandHandler : CommandHandlerBase
    {
        private const string Command = "stat";

        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        public StatCommandHandler()
            : base(Command)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (this.CheckCommand(commandRequest) && string.IsNullOrEmpty(commandRequest.Parameters))
            {
                Stat();
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static void Stat()
        {
            var result = Program.fileCabinetService.GetStat();
            Console.WriteLine(Program.Rm.GetString("StatMessage", CultureInfo.CurrentCulture), result.Item1, result.Item2);
        }
    }
}

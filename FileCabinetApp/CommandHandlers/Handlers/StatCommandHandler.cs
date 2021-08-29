﻿using System;
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
        /// <param name="service"></param>
        public StatCommandHandler(IFileCabinetService service)
            : base(Command, service)
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

        private void Stat()
        {
            var result = this.Service.GetStat();
            Console.WriteLine(Program.Rm.GetString("StatMessage", CultureInfo.CurrentCulture), result.Item1, result.Item2);
        }
    }
}

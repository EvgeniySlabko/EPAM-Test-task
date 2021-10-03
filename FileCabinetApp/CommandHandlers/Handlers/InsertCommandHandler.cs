using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// InsertCommandHandler.
    /// </summary>
    public class InsertCommandHandler : FileCabinetServiceCommandHandlerBase
    {
        private const string Command = "insert";

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service.</param>
        public InsertCommandHandler(IFileCabinetService service)
            : base(Command, service)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (this.CheckCommand(commandRequest))
            {
                var result = CommandParser.InsertParser(commandRequest.Parameters, out FileCabinetRecord record);
                if (result.Item1)
                {
                    this.Insert(record);
                }
                else
                {
                    Console.WriteLine(result.Item2);
                }
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        /// <summary>
        /// Create ne Record.
        /// </summary>
        /// <param name="record">Given record.</param>
        private void Insert(FileCabinetRecord record)
        {
            int recordId;
            try
            {
                recordId = this.Service.Insert(record);
            }
            catch (ArgumentException exeption)
            {
                Console.WriteLine(exeption.Message);
                return;
            }

            Console.WriteLine(StringManager.Rm.GetString("CreateRecordMessage", CultureInfo.CurrentCulture), recordId);
        }
    }
}

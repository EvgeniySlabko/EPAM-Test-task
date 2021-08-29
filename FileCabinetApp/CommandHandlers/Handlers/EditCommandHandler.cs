using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// handler for Edit command.
    /// </summary>
    public class EditCommandHandler : FileCabinetServiceCommandHandlerBase
    {
        private const string Command = "edit";


        /// <summary>
        /// Initializes a new instance of the <see cref="EditCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Srvice.</param>
        public EditCommandHandler(IFileCabinetService service)
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

            var result = new IntConverter().Convert(commandRequest.Parameters);
            if (result.Item1)
            {
                this.Edit(result.Item3);
            }
            else
            {
                Console.WriteLine(result.Item2);
                base.Handle(commandRequest);
            }
        }

        private void Edit(int id)
        {
            ConsoleHelper.EnterRecord(out FileCabinetRecord record);
            record.Id = id;
            try
            {
                this.Service.Edit(record);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine(Program.Rm.GetString("UppdateRecordMessage", CultureInfo.CurrentCulture), record.Id);
        }
    }
}

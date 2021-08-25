using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// handler for Edit command.
    /// </summary>
    public class EditCommandHandler : CommandHandlerBase
    {
        private const string Command = "edit";

        /// <summary>
        /// Initializes a new instance of the <see cref="EditCommandHandler"/> class.
        /// </summary>
        public EditCommandHandler()
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
                Edit(result.Item3);
            }
            else
            {
                Console.WriteLine(result.Item2);
                base.Handle(commandRequest);
            }
        }

        private static void Edit(int id)
        {
            ConsoleHelper.EnterRecord(out FileCabinetRecord record);
            record.Id = id;
            try
            {
                Program.fileCabinetService.Edit(record);
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

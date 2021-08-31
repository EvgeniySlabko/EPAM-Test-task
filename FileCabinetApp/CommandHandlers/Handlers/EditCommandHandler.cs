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

        private readonly ValidationRuleSet validationRuleSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service.</param>
        /// <param name="validationRuleSet">Validation rule set.</param>
        public EditCommandHandler(IFileCabinetService service, ValidationRuleSet validationRuleSet)
            : base(Command, service)
        {
            this.validationRuleSet = validationRuleSet;
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
            ConsoleHelper.EnterRecord(out FileCabinetRecord record, this.validationRuleSet);
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

            Console.WriteLine(StringManager.Rm.GetString("UppdateRecordMessage", CultureInfo.CurrentCulture), record.Id);
        }
    }
}

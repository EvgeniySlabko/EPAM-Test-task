using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Handler for create command.
    /// </summary>
    public class CreateCommandHandler : FileCabinetServiceCommandHandlerBase
    {
        private const string Command = "create";

        private readonly ValidationSettings validationRuleSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service.</param>
        /// <param name="validationRuleSet">Validation rule set.</param>
        public CreateCommandHandler(IFileCabinetService service, ValidationSettings validationRuleSet)
            : base(Command, service)
        {
            this.validationRuleSet = validationRuleSet;
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (this.CheckCommand(commandRequest))
            {
                if (string.IsNullOrEmpty(commandRequest.Parameters))
                {
                    this.Create();
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

        private void Create()
        {
            ConsoleHelper.EnterRecord(out ValidationRecord newRecord, this.validationRuleSet);
            int recordId = -1;
            try
            {
                recordId = this.Service.CreateRecord(newRecord);
            }
            catch (ArgumentException exeption)
            {
                Console.Write(exeption.Message);
                this.Create();
            }

            Console.WriteLine(StringManager.Rm.GetString("CreateRecordMessage", CultureInfo.CurrentCulture), recordId);
        }
    }
}

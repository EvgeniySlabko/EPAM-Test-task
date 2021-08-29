﻿using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Handler for create command.
    /// </summary>
    public class CreateCommandHandler : FileCabinetServiceCommandHandlerBase
    {
        private const string Command = "create";

        private readonly ValidationRuleSet validationRuleSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service.</param>
        /// <param name="validationRuleSet">Validation rule set.</param>
        public CreateCommandHandler(IFileCabinetService service, ValidationRuleSet validationRuleSet)
            : base(Command, service)
        {
            this.validationRuleSet = validationRuleSet;
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (this.CheckCommand(commandRequest) && string.IsNullOrEmpty(commandRequest.Parameters))
            {
                this.Create();
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Create()
        {
            ConsoleHelper.EnterRecord(out FileCabinetRecord newRecord, this.validationRuleSet);
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
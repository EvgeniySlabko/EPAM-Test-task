using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Main class.
    /// </summary>
    public static class Program
    {
        private static ValidationRule validationRule = ValidationRule.Default;
        private static ServiceType serviceType = ServiceType.MemoryService;

        private static IFileCabinetService fileCabinetService;
        private static ValidationRuleSet validationRuleSet;
        private static bool isRunning = true;

        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            Console.WriteLine(StringManager.Rm.GetString("WelcomeMessage", CultureInfo.CurrentCulture));
            ParseCommandLineArguments(args);
            DisplayInfoMessage();
            Console.WriteLine();

            do
            {
                Console.Write(StringManager.Rm.GetString("ConsoleStartSymbol", CultureInfo.CurrentCulture));
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];
                var commandHandler = CreateCommandHanders();

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(StringManager.Rm.GetString("HintMessage", CultureInfo.CurrentCulture));
                    continue;
                }

                commandHandler.Handle(new AppCommandRequest(command, (inputs.Length == 2) ? inputs[1] : string.Empty));
            }
            while (isRunning);
            fileCabinetService.Purge();
        }

        private static ICommandHandler CreateCommandHanders()
        {
            var createHandler = new CreateCommandHandler(fileCabinetService, validationRuleSet);
            var editHandler = new EditCommandHandler(fileCabinetService, validationRuleSet);
            var exitHandler = new ExitCommandHandler(stop => isRunning = stop);
            var exportHandler = new ExportCommandHandler(fileCabinetService);
            var findHandler = new FindCommandHandler(fileCabinetService, Program.DefaultRecordsPrint);
            var helpHandler = new HelpCommandHandler();
            var importHandler = new ImportCommandHandler(fileCabinetService);
            var listHandler = new ListCommandHandler(fileCabinetService, Program.DefaultRecordsPrint);
            var removeHandler = new RemoveCommandHandler(fileCabinetService);
            var statHandler = new StatCommandHandler(fileCabinetService);

            statHandler.SetNext(removeHandler);
            removeHandler.SetNext(listHandler);
            listHandler.SetNext(importHandler);
            importHandler.SetNext(helpHandler);
            helpHandler.SetNext(findHandler);
            findHandler.SetNext(exportHandler);
            exportHandler.SetNext(exitHandler);
            exitHandler.SetNext(editHandler);
            editHandler.SetNext(createHandler);

            return statHandler;
        }

        private static void DefaultRecordsPrint(IEnumerable<FileCabinetRecord> records)
        {
            var list = new List<FileCabinetRecord>(records);

            if (records is null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            if (list.Count.Equals(0))
            {
                Console.WriteLine(StringManager.Rm.GetString("EmptyListMessage", CultureInfo.CurrentCulture));
            }

            foreach (var record in records)
            {
                Console.WriteLine(StringManager.Rm.GetString("RecordInfoString", CultureInfo.CurrentCulture), record.Id, record.FirstName, record.LastName, record.DateOfBirth.ToString("yyyy-MMM-dd", DateTimeFormatInfo.InvariantInfo), record.IdentificationNumber, record.IdentificationLetter, record.PointsForFourTests);
            }
        }

        private static void DisplayInfoMessage()
        {
            string validationRuleString = (validationRule == ValidationRule.Default) ? "default" : "custom";
            string serviceTypeString = (serviceType == ServiceType.FileService) ? "file" : "memory";
            Console.WriteLine(StringManager.Rm.GetString("InfoMessage", CultureInfo.CurrentCulture), validationRuleString, serviceTypeString);
        }

        private static void ParseCommandLineArguments(string[] args)
        {
            var parser = new CommandLineParser();
            static void ValidationRuleAction(string arg)
            {
                if (arg.Equals("custom"))
                {
                    validationRule = ValidationRule.Custom;
                }
                else if (arg.Equals("default"))
                {
                    validationRule = ValidationRule.Default;
                }
                else
                {
                    throw new ArgumentException(StringManager.Rm.GetString("UnableCommandLineArgumentsMessage", CultureInfo.CurrentCulture));
                }
            }

            static void StorageRuleAction(string arg)
            {
                if (arg.Equals("file"))
                {
                    serviceType = ServiceType.FileService;
                }
                else if (arg.Equals("memory"))
                {
                    serviceType = ServiceType.MemoryService;
                }
                else
                {
                    throw new ArgumentException(StringManager.Rm.GetString("UnableCommandLineArgumentsMessage", CultureInfo.CurrentCulture));
                }
            }

            parser.AddCommandLineArgumentDescription("--validation-rules", "-v", ValidationRuleAction);
            parser.AddCommandLineArgumentDescription("--storage", "-s", StorageRuleAction);

            parser.ParseCommandLineArguments(args);
            ApplyCommandLineArguments();
        }

        private static void ApplyCommandLineArguments()
        {
            IRecordValidator recordvalidator;
            switch (validationRule)
            {
                case ValidationRule.Default:
                    validationRuleSet = ValidationRuleSetMaker.MakeDefaultValidationSet();
                    recordvalidator = new DefaultRecordValidator();
                    break;

                case ValidationRule.Custom:
                    validationRuleSet = ValidationRuleSetMaker.MakeCustomValidationSet();
                    recordvalidator = new CustomRecordValidator();
                    break;
                default:
                    throw new ArgumentException(nameof(validationRule));
            }

            switch (serviceType)
            {
                case ServiceType.MemoryService:
                    fileCabinetService = new FileCabinetMemoryService(recordvalidator);
                    break;

                case ServiceType.FileService:
                    fileCabinetService = new FileCabinetFilesystemService(recordvalidator);
                    break;
                default:
                    throw new ArgumentException(nameof(serviceType));
            }
        }
    }
}
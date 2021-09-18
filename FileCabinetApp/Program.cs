using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FileCabinetApp
{
    /// <summary>
    /// Main class.
    /// </summary>
    public static class Program
    {
        private static string validationRule = Constants.DefaultValidationSettingsName;
        private static ServiceType serviceType = Constants.DefaultServiceType;
        private static IFileCabinetService fileCabinetService;
        private static ValidationSettings validationSettings;
        private static bool isRunning = true;
        private static bool useStopWatch;
        private static bool useLogger;

        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            Console.WriteLine(StringManager.Rm.GetString("WelcomeMessage", CultureInfo.CurrentCulture));
            ParseCommandLineArguments(args);
            LoadValidationSettings();
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
            var uppdateHandler = new UppdateCommandHandler(fileCabinetService);
            var deleteHandler = new DeleteCommandHandler(fileCabinetService);
            var insertHandler = new InsertCommandHandler(fileCabinetService);
            var createHandler = new CreateCommandHandler(fileCabinetService, validationSettings);
            var exitHandler = new ExitCommandHandler(stop => isRunning = stop);
            var exportHandler = new ExportCommandHandler(fileCabinetService);
            var findHandler = new FindCommandHandler(fileCabinetService, Program.DefaultRecordsPrint);
            var helpHandler = new HelpCommandHandler();
            var importHandler = new ImportCommandHandler(fileCabinetService);
            var listHandler = new ListCommandHandler(fileCabinetService, Program.DefaultRecordsPrint);
            var statHandler = new StatCommandHandler(fileCabinetService);

            statHandler.SetNext(listHandler);
            listHandler.SetNext(importHandler);
            importHandler.SetNext(helpHandler);
            helpHandler.SetNext(findHandler);
            findHandler.SetNext(exportHandler);
            exportHandler.SetNext(exitHandler);
            exitHandler.SetNext(createHandler);
            createHandler.SetNext(insertHandler);
            insertHandler.SetNext(deleteHandler);
            deleteHandler.SetNext(uppdateHandler);

            return statHandler;
        }

        private static void LoadValidationSettings()
        {
            validationSettings = ValidationSetLoader.LoadRules(Constants.ValidationSettingsFileName)[validationRule];
        }

        private static void DefaultRecordsPrint(IEnumerable<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            foreach (var record in records)
            {
                PrintOneRecord(record);
            }
        }

        private static void PrintOneRecord(FileCabinetRecord record)
        {
            Console.WriteLine(StringManager.Rm.GetString("RecordInfoString", CultureInfo.CurrentCulture), record.Id, record.FirstName, record.LastName, record.DateOfBirth.ToString("yyyy-MMM-dd", DateTimeFormatInfo.InvariantInfo), record.IdentificationNumber, record.IdentificationLetter, record.PointsForFourTests);
        }

        private static void DisplayInfoMessage()
        {
            string validationRuleString = validationRule;
            string serviceTypeString = (serviceType == ServiceType.FileService) ? "file" : "memory";
            string stopWatchMode = useStopWatch ? "on" : "off";
            string loggerMode = useLogger ? "on" : "off";
            Console.WriteLine(StringManager.Rm.GetString("InfoMessage", CultureInfo.CurrentCulture), validationRuleString, serviceTypeString, stopWatchMode, loggerMode);
        }

        private static void ParseCommandLineArguments(string[] args)
        {
            var parser = new CommandLineParser();

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

            parser.AddCommandLineArgumentDescription("--validation-rules", "-v", s => validationRule = s);
            parser.AddCommandLineArgumentDescription("--storage", "-s", StorageRuleAction);
            parser.AddCommandLineArgumentDescription("--use-stopwatch", "-sw", s => useStopWatch = bool.Parse(s));
            parser.AddCommandLineArgumentDescription("--use-logger", "-l", s => useLogger = bool.Parse(s));

            parser.ParseCommandLineArguments(args);
            ApplyCommandLineArguments();
        }

        private static void ApplyCommandLineArguments()
        {
            IRecordValidator recordvalidator;

            if (validationRule.Equals("default"))
            {
                recordvalidator = new ValidatorBuilder().CreateDefault();
            }
            else if (validationRule.Equals("custom"))
            {
                recordvalidator = new ValidatorBuilder().CreateCustom();
            }
            else
            {
                throw new ArgumentNullException(nameof(validationRule));
            }

            IFileCabinetService service;
            service = serviceType switch
            {
                ServiceType.MemoryService => new FileCabinetMemoryService(recordvalidator),
                ServiceType.FileService => new FileCabinetFilesystemService(recordvalidator),
                _ => throw new ArgumentException(nameof(serviceType)),
            };

            fileCabinetService = useStopWatch ? new ServiceMeter(service) : service;
            if (useLogger)
            {
                fileCabinetService = new ServiceLogger(fileCabinetService);
            }
        }
    }
}
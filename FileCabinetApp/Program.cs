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
        private const string ConsoleStartSymbol = ">";

        public static readonly ResourceManager Rm = new ("FileCabinetApp.Resource.Strings", Assembly.GetExecutingAssembly());
        public static IFileCabinetService fileCabinetService;
        private static ValidationRule validationRule = ValidationRule.Default;
        private static ServiceType serviceType = ServiceType.MemoryService;
        public static ValidationRuleSet validationRuleSet;
        public static bool isRunning = true;

        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            Console.WriteLine(Rm.GetString("WelcomeMessage", CultureInfo.CurrentCulture));
            ParseCommandLineArguments(args);
            DisplayInfoMessage();
            Console.WriteLine();

            do
            {
                Console.Write(ConsoleStartSymbol);
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];
                var commandHandler = CreateCommandHanders();

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Rm.GetString("HintMessage", CultureInfo.CurrentCulture));
                    continue;
                }

                commandHandler.Handle(new AppCommandRequest(command, (inputs.Length == 2) ? inputs[1] : string.Empty));
            }
            while (isRunning);
            fileCabinetService.Purge();
        }

        private static CommandHandler CreateCommandHanders()
        {
            var commandHandler = new CommandHandler();
            return commandHandler;
        }

        private static void DisplayInfoMessage()
        {
            string validationRuleString = (validationRule == ValidationRule.Default) ? "default" : "custom";
            string serviceTypeString = (serviceType == ServiceType.FileService) ? "file" : "memory";
            Console.WriteLine(Rm.GetString("InfoMessage", CultureInfo.CurrentCulture), validationRuleString, serviceTypeString);
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
                    throw new ArgumentException(Rm.GetString("UnableCommandLineArgumentsMessage", CultureInfo.CurrentCulture));
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
                    throw new ArgumentException(Rm.GetString("UnableCommandLineArgumentsMessage", CultureInfo.CurrentCulture));
                }
            }

            parser.AddCommandLineArgumentDescription("--validation-rules", "-v", ValidationRuleAction);
            parser.AddCommandLineArgumentDescription("--storage", "-s", StorageRuleAction);

            parser.ParseCommandLineArguments(args);
            ApplyCommandLineArguments();
        }

        private static void ApplyCommandLineArguments()
        {
            switch (validationRule)
            {
                case ValidationRule.Default:
                    validationRuleSet = ValidationRuleSetMaker.MakeDefaultValidationSet();
                    break;

                case ValidationRule.Custom:
                    validationRuleSet = ValidationRuleSetMaker.MakeCustomValidationSet();
                    break;
            }

            var serviceValidator = new ServiceValidator(validationRuleSet);
            switch (serviceType)
            {
                case ServiceType.MemoryService:
                    fileCabinetService = new FileCabinetMemoryService(serviceValidator);
                    break;

                case ServiceType.FileService:
                    fileCabinetService = new FileCabinetFilesystemService(serviceValidator);
                    break;
            }
        }

    }
}
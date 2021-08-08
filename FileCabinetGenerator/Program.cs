using FileCabinetApp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace FileCabinetGenerator
{
    class Program
    {
        static OutputFileType outputFileType = new();
        static string outputFileName;
        static int amountOfGeneratedRecords;
        static int firstIdValue;
        const string resultString = "{0} records were written to {1}";
        private static readonly ResourceManager Rm = new("FileCabinetApp.Resource.Strings", Assembly.GetExecutingAssembly());
        private static void ParseCommandLineArguments(string[] args)
        {
            var parser = new CommandLineParser();
            static void OutputFileTypeAction(string arg)
            {
                if (arg.Equals("csv"))
                {
                    outputFileType = OutputFileType.Cvs;
                }
                else if (arg.Equals("xml"))
                {
                    outputFileType = OutputFileType.Xml;
                }
                else
                {
                    throw new ArgumentException(Rm.GetString("UnableCommandLineArgumentsMessage", CultureInfo.CurrentCulture));
                }
            }


            parser.AddCommandLineArgumentDescription("--output-type", "-t", OutputFileTypeAction);
            parser.AddCommandLineArgumentDescription("--output", "-o", arg => outputFileName = arg);
            parser.AddCommandLineArgumentDescription("--records-amount", "-a", arg => amountOfGeneratedRecords = int.Parse(arg));
            parser.AddCommandLineArgumentDescription("--start-id", "-i", arg => firstIdValue = int.Parse(arg));

            parser.ParseCommandLineArguments(args);
        }

        private static ReadOnlyCollection<FileCabinetRecord> GenerateRandomRecords()
        {
            ValidationRuleSet defaultRuleSet = ValidationRuleSetMaker.MakeDefaultValidationSet();
            var records = new List<FileCabinetRecord>();
            var random = new Random();
            for (int i = 0; i < amountOfGeneratedRecords; i++)
            {
                var year = random.Next(random.Next(defaultRuleSet.DateValidator.MinDateOfBirth.Year, defaultRuleSet.DateValidator.MaxDateOfBirth.Year));
                var month = random.Next(random.Next(defaultRuleSet.DateValidator.MinDateOfBirth.Month, defaultRuleSet.DateValidator.MaxDateOfBirth.Month));
                var day = random.Next(random.Next(defaultRuleSet.DateValidator.MinDateOfBirth.Day, defaultRuleSet.DateValidator.MaxDateOfBirth.Day));
                var pointForFourTests = (short)random.Next(random.Next(defaultRuleSet.PointsForFourTestsValidator.MinValue, defaultRuleSet.PointsForFourTestsValidator.MaxValue));
                var IdentificationNumber = (decimal)random.Next(random.Next((int)defaultRuleSet.IdentificationNumberValidator.MinValue, (int)defaultRuleSet.IdentificationNumberValidator.MaxValue));

                FileCabinetRecord newRecord = new FileCabinetRecord();
                newRecord.Id = firstIdValue + i;
                newRecord.FirstName = GetRandomString(defaultRuleSet.FirstNameVAidator.MaxLen / 2);
                newRecord.LastName = GetRandomString(defaultRuleSet.FirstNameVAidator.MaxLen / 2);
                newRecord.DateOfBirth = new DateTime(year, month, day);
                newRecord.PointsForFourTests = pointForFourTests;
                newRecord.IdentificationNumber = IdentificationNumber;
                newRecord.IdentificationLetter = 'f';
                records.Add(newRecord);
            }
            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        private static string GetRandomString(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        
        static void Main(string[] args)
        {
            ParseCommandLineArguments(args);
            Console.WriteLine(resultString, amountOfGeneratedRecords, outputFileName);
        }
    }
}

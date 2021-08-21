using FileCabinetApp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Xml;
using System.Xml.Serialization;

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
                var year = random.Next(defaultRuleSet.DateValidator.MinDateOfBirth.Year, defaultRuleSet.DateValidator.MaxDateOfBirth.Year);
                var month = random.Next(defaultRuleSet.DateValidator.MinDateOfBirth.Month, defaultRuleSet.DateValidator.MaxDateOfBirth.Month);
                var day = random.Next(defaultRuleSet.DateValidator.MinDateOfBirth.Day, defaultRuleSet.DateValidator.MaxDateOfBirth.Day);
                var pointForFourTests = (short)random.Next(defaultRuleSet.PointsForFourTestsValidator.MinValue, defaultRuleSet.PointsForFourTestsValidator.MaxValue);
                var identificationNumber = GetRandomtDecimal(defaultRuleSet.IdentificationNumberValidator.MinValue, defaultRuleSet.IdentificationNumberValidator.MaxValue);

                var newRecord = new FileCabinetRecord
                {
                    Id = firstIdValue + i,
                    FirstName = GetRandomString(defaultRuleSet.FirstNameVAidator.MaxLen / 4),
                    LastName = GetRandomString(defaultRuleSet.FirstNameVAidator.MaxLen / 4),
                    DateOfBirth = new DateTime(year, month, day),
                    PointsForFourTests = pointForFourTests,
                    IdentificationNumber = identificationNumber,
                    IdentificationLetter = 'f'
                };
                records.Add(newRecord);
            }
            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        public static decimal GetRandomtDecimal(decimal from, decimal to)
        {
            var rnd = new Random();
            byte fromScale = new System.Data.SqlTypes.SqlDecimal(from).Scale;
            byte toScale = new System.Data.SqlTypes.SqlDecimal(to).Scale;

            byte scale = (byte)(fromScale + toScale);
            if (scale > 28)
                scale = 28;

            var r = new decimal(rnd.Next(), rnd.Next(), rnd.Next(), false, scale);
            if (Math.Sign(from) == Math.Sign(to) || from == 0 || to == 0)
                return decimal.Remainder(r, to - from) + from;

            bool getFromNegativeRange = (double)from + rnd.NextDouble() * ((double)to - (double)from) < 0;
            return getFromNegativeRange ? decimal.Remainder(r, -from) + from : decimal.Remainder(r, to);
        }

        private static string GetRandomString(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        
        private static void WriteCsv(IEnumerable<FileCabinetRecord> records)
        {
            using var writer = new StreamWriter(outputFileName);
            var csvWriter = new FileCabinetRecordCsvWriter(writer);
            csvWriter.Write("Id, First Name, Last Name, Date of Birth, Identification number, Identification letter, Points for four tests");
            foreach (var record in records)
            {
                csvWriter.Write(record);
            }
        }

        private static void WriteXml(IEnumerable<FileCabinetRecord> records)
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var recordSerializeble = new FileCabinetRecordsSerializable(records);
            var formatter = new XmlSerializer(typeof(FileCabinetRecordsSerializable));
            using var fileStream = new FileStream(outputFileName, FileMode.OpenOrCreate);
            formatter.Serialize(fileStream, recordSerializeble, ns);
        }
        
        static void Main(string[] args)
        {
            ParseCommandLineArguments(args);

            var records = GenerateRandomRecords();
            if (outputFileType == OutputFileType.Cvs)
            {
                WriteCsv(records);
            }
            else
            {
                WriteXml(records);
            }
            Console.WriteLine(resultString, amountOfGeneratedRecords, outputFileName);
        }
    }
}

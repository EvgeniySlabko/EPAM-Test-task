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
        static FileType outputFileType;
        static string outputFileName;
        static string validationRulesFilePath;
        static int amountOfGeneratedRecords;
        static int firstIdValue;
        const string randomString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string resultString = "{0} records were written to {1}";
        private static readonly ResourceManager Rm = new("FileCabinetApp.Resource.Strings", Assembly.GetExecutingAssembly());
        private static void ParseCommandLineArguments(string[] args)
        {
            var parser = new CommandLineParser();
            static void OutputFileTypeAction(string arg)
            {
                if (arg.Equals("csv"))
                {
                    outputFileType = FileType.Сsv;
                }
                else if (arg.Equals("xml"))
                {
                    outputFileType = FileType.Xml;
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
            parser.AddCommandLineArgumentDescription("--validation-file-path", "-v", arg => validationRulesFilePath = arg);

            parser.ParseCommandLineArguments(args);
        }

        private static ReadOnlyCollection<FileCabinetRecord> GenerateRandomRecords()
        {
            var defaultRule = ValidationSetLoader.LoadRules(validationRulesFilePath)["default"];

            var records = new List<FileCabinetRecord>();
            var random = new Random();
            for (int i = 0; i < amountOfGeneratedRecords; i++)
            {
                var dateFrom = new DateTime(defaultRule.DateModel.From.Year, defaultRule.DateModel.From.Month, defaultRule.DateModel.From.Day);
                var dateTo = new DateTime(defaultRule.DateModel.To.Year, defaultRule.DateModel.To.Month, defaultRule.DateModel.To.Day);
                var date = dateFrom.AddDays(random.Next((dateTo - dateFrom).Days));
                var pointForFourTests = (short)random.Next(defaultRule.PointsModel.Min, defaultRule.PointsModel.Max);
                var identificationNumber = GetRandomtDecimal(defaultRule.IdentificationNumberModel.Min, defaultRule.IdentificationNumberModel.Max);

                var newRecord = new FileCabinetRecord
                {
                    Id = firstIdValue + i,
                    FirstName = GetRandomString(defaultRule.FirstName.Max / 4),
                    LastName = GetRandomString(defaultRule.LastName.Max / 4),
                    DateOfBirth = date,
                    PointsForFourTests = pointForFourTests,
                    IdentificationNumber = identificationNumber,
                    IdentificationLetter = GetRandomChar(),
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

        private static char GetRandomChar()
        {
            return char.ToLower(randomString[new Random().Next(randomString.Length - 1)]);
        }
       
        private static string GetRandomString(int length)
        {
            var random = new Random();
            return new string(Enumerable.Repeat(randomString, length)
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
            using var fileStream = new FileStream(outputFileName, FileMode.Create);
            formatter.Serialize(fileStream, recordSerializeble, ns);
        }
        
        static void Main(string[] args)
        {
            ParseCommandLineArguments(args);

            var records = GenerateRandomRecords();
            if (outputFileType == FileType.Сsv)
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

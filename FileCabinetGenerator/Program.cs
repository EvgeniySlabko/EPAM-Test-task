using FileCabinetApp;
using System;
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

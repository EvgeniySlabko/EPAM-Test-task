using System;
using System.Globalization;

namespace FileCabinetGenerator
{
    class Program
    {
        static OutputFileType outputFileType = new ();
        static string outputFileName;
        static int amountOfGeneratedRecords;
        static int firstIdValue;
        const string resultString = "{0} records were written to {1}";

        private static void ParseCommandLineArguments(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (args.Length.Equals(0))
            {
                return;
            }

            static void SetArguments(int argumentIndex, string argument)
            {
                if (CommandLineArguments[argumentIndex].Item3[0].Item1 is null)
                {
                    CommandLineArguments[argumentIndex].Item3[0].Item2(argument);
                }
                else
                {
                    var index = Array.FindIndex(CommandLineArguments[argumentIndex].Item3, 0, CommandLineArguments[argumentIndex].Item3.Length, i => i.Item1.Equals(argument, StringComparison.CurrentCulture));
                    if (index != -1)
                    {
                        CommandLineArguments[argumentIndex].Item3[index].Item2(argument);
                    }
                }
            }

            bool wasArgumentType = false;
            int argumentIndex = 0;
            foreach (var arg in args)
            {
                var lowerArg = arg.ToLower(CultureInfo.CurrentCulture);
                if (wasArgumentType)
                {
                    SetArguments(argumentIndex, lowerArg);
                    wasArgumentType = false;
                    continue;
                }
                else if (lowerArg.StartsWith("--", StringComparison.CurrentCulture) && !wasArgumentType)
                {
                    var splitedArg = lowerArg.Split('=');
                    if (splitedArg.Length == 2)
                    {
                        var index = Array.FindIndex(CommandLineArguments, 0, CommandLineArguments.Length, i => i.Item1.Equals(splitedArg[0], StringComparison.CurrentCulture));
                        SetArguments(index, splitedArg[1]);
                        wasArgumentType = false;
                        continue;
                    }
                }
                else if (lowerArg.StartsWith("-", StringComparison.CurrentCulture) && !wasArgumentType)
                {
                    argumentIndex = Array.FindIndex(CommandLineArguments, 0, CommandLineArguments.Length, i => i.Item2.Equals(lowerArg, StringComparison.CurrentCulture));
                    if (argumentIndex != -1)
                    {
                        wasArgumentType = true;
                        continue;
                    }
                }

                throw new ArgumentException("Unable command line arguments");
            }
        }
#pragma warning disable SA1118 // Parameter should not span multiple lines
        private static readonly Tuple<string, string, Tuple<string, Action<string>>[]>[] CommandLineArguments = new Tuple<string, string, Tuple<string, Action<string>>[]>[]
            {
                new Tuple<string, string, Tuple<string, Action<string>>[]>("--output-type", "-t", new Tuple<string, Action<string> >[]
                {
                    new Tuple<string, Action<string>>("cvc", (arg) => outputFileType = OutputFileType.Cvs),
                    new Tuple<string, Action<string>>("xml", (arg) => outputFileType = OutputFileType.Xml),
                }),

                new Tuple<string, string, Tuple<string, Action<string>>[]>("--output", "-o", new Tuple<string, Action<string>>[]
                {
                    new Tuple<string, Action<string>>(null, (arg) => outputFileName = arg),
                }),

                new Tuple<string, string, Tuple<string, Action<string>>[]>("--records-amount", "-a", new Tuple<string, Action<string>>[]
                {
                    new Tuple<string, Action<string>>(null, (arg) => amountOfGeneratedRecords = int.Parse(arg)),
                }),

                new Tuple<string, string, Tuple<string, Action<string>>[]>("--start-id", "-i", new Tuple<string, Action<string>>[]
                {
                    new Tuple<string, Action<string>>(null, (arg) => firstIdValue = int.Parse(arg)),
                }),
            };
#pragma warning restore SA1118 // Parameter should not span multiple lines
        static void Main(string[] args)
        {
            ParseCommandLineArguments(args);
            Console.WriteLine(resultString, amountOfGeneratedRecords, outputFileName);
        }
    }
}

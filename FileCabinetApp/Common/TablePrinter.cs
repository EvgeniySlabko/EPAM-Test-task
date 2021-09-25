using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FileCabinetApp
{
    /// <summary>
    /// Table printer.
    /// </summary>
    public static class TablePrinter
    {
        private const char Dash = '-';
        private const char Plus = '+';
        private const char Stik = '|';

        /// <summary>
        /// Pints parameters in the table.
        /// </summary>
        /// <param name="headers">Headers in order.</param>
        /// <param name="parameters">parameters in order.</param>
        public static void Print(string[] headers, IEnumerable<List<string>> parameters)
        {
            if (headers is null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            var allItems = new List<List<string>>
            {
                headers.ToList(),
            };
            allItems.AddRange(parameters);
            var gaps = new int[headers.Length];
            for (int i = 0; i < gaps.Length; i++)
            {
                gaps[i] = allItems.Max(p => p[i].Length);
            }

            Console.WriteLine();
            PrintLine(gaps);
            foreach (var parametersSet in allItems)
            {
                Console.WriteLine();
                Console.Write(Stik);
                for (int j = 0; j < parametersSet.Count; j++)
                {
                    if (Regex.IsMatch(parametersSet[j], @"^\d+$"))
                    {
                        Console.Write(string.Concat(Enumerable.Repeat(" ", gaps[j] - parametersSet[j].Length)) + parametersSet[j]);
                    }
                    else
                    {
                        Console.Write(parametersSet[j] + string.Concat(Enumerable.Repeat(" ", gaps[j] - parametersSet[j].Length)));
                    }

                    Console.Write(Stik);
                }

                Console.WriteLine();
                PrintLine(gaps);
            }

            Console.WriteLine();
        }

        private static void PrintLine(int[] gaps)
        {
            Console.Write(Plus);
            foreach (var gap in gaps)
            {
                Console.Write(string.Concat(Enumerable.Repeat(Dash, gap)));
                Console.Write(Plus);
            }
        }
    }
}

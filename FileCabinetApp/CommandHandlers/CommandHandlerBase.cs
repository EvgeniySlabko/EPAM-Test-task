using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Base class for command handler.
    /// </summary>
    public class CommandHandlerBase : ICommandHandler
    {
        private const float MinamalSimilarityСoefficientForShow = 0.3f;
        private readonly string command;
        private ICommandHandler commandHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandlerBase"/> class.
        /// </summary>
        /// <param name="command">Command.</param>
        protected CommandHandlerBase(string command)
        {
            this.command = command;
        }

        /// <summary>
        /// Handle.
        /// </summary>
        /// <param name="commandRequest">Command handler.</param>
        public virtual void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest));
            }

            if (this.commandHandler is not null)
            {
                this.commandHandler.Handle(commandRequest);
            }
            else
            {
                PrintMissedCommandInfo(commandRequest.Command);
            }
        }

        /// <summary>
        /// Set next command.
        /// </summary>
        /// <param name="commandHandler">Command handler.</param>
        /// <returns>Next command.</returns>
        public ICommandHandler SetNext(ICommandHandler commandHandler)
        {
            this.commandHandler = commandHandler;
            return this.commandHandler;
        }

        /// <summary>
        /// Common command processing.
        /// </summary>
        /// <param name="commandRequest">Given command reqest.</param>
        /// <returns>Result.</returns>
        protected bool CheckCommand(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest));
            }

            return commandRequest.Command.Equals(this.command);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine(StringManager.Rm.GetString("MissedCommandInfoMessage", CultureInfo.CurrentCulture), command);

            var similarCommands = new List<string>();
            foreach (var currentCommand in HelpCommandHandler.HelpMessages)
            {
                if (GetSimilarity(currentCommand[0], command) >= MinamalSimilarityСoefficientForShow)
                {
                    similarCommands.Add(currentCommand[0]);
                }
            }

            if (similarCommands.Count != 0)
            {
                if (similarCommands.Count > 1)
                {
                    Console.WriteLine(StringManager.Rm.GetString("SimilarCommandsMessage", CultureInfo.CurrentCulture), command);
                }
                else
                {
                    Console.WriteLine(StringManager.Rm.GetString("SimilarCommandMessage", CultureInfo.CurrentCulture), command);
                }

                similarCommands.ForEach(c => Console.WriteLine(c));
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Get similarity coefficient.
        /// </summary>
        /// <param name="first">Given command.</param>
        /// <param name="second">ExistsCommand.</param>
        /// <returns>Similarity coefficient.</returns>
        private static float GetSimilarity(string first, string second)
        {
            if (string.IsNullOrEmpty(first) || string.IsNullOrEmpty(second))
            {
                return 0;
            }

            var firstLength = first.Length;
            var secondtLength = second.Length;
            int stepsToSame;

            int[][] matrix = new int[firstLength + 1][];
            for (int i = 0; i < matrix.Length; i++)
            {
                matrix[i] = new int[secondtLength + 1];
            }

            if (firstLength == 0)
            {
                stepsToSame = secondtLength;
            }
            else if (secondtLength == 0)
            {
                stepsToSame = firstLength;
            }
            else
            {
                for (var i = 0; i <= firstLength; i++)
                {
                    matrix[i][0] = i;
                }

                for (var j = 0; j <= secondtLength; j++)
                {
                    matrix[0][j] = j;
                }

                for (var i = 1; i <= firstLength; i++)
                {
                    for (var j = 1; j <= secondtLength; j++)
                    {
                        var cost = (second[j - 1] == first[i - 1]) ? 0 : 1;

                        matrix[i][j] = Math.Min(
                            Math.Min(matrix[i - 1][j] + 1, matrix[i][j - 1] + 1),
                            matrix[i - 1][j - 1] + cost);
                    }
                }

                stepsToSame = matrix[firstLength][secondtLength];
            }

            return 1 - ((float)stepsToSame / (float)Math.Max(first.Length, second.Length));
        }
    }
}

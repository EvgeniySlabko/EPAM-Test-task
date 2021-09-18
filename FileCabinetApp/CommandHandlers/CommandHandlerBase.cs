using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FileCabinetApp
{
    /// <summary>
    /// Base class for command handler.
    /// </summary>
    public class CommandHandlerBase : ICommandHandler
    {
        private const float MinamalSimilarityСoefficientForShow = 0.5f;
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
        /// Get similarityСoefficient.
        /// </summary>
        /// <param name="source">Given command.</param>
        /// <param name="target">ExistsCommand.</param>
        /// <returns>Similarity coefficient.</returns>
        private static double GetSimilarity(string source, string target)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            {
                return 0;
            }

            float sameLetters = 0;
            foreach (var letter in target)
            {
                var matches = new Regex($@"{letter}").Matches(source).Count;
                sameLetters += (matches != 0) ? 1 / matches : 0;
            }

            return (float)sameLetters / (float)source.Length;
        }
    }
}

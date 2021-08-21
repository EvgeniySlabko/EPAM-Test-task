using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Common parser for command line arguments.
    /// </summary>
    public class CommandLineParser
    {
        private readonly List<Tuple<string, string, Action<string>>> commandLineArgumentDescriptions = new ();

        /// <summary>
        /// Add command line argument description.
        /// </summary>
        /// <param name="fullArgumentName">Full argument name.</param>
        /// <param name="shortArgumentName">Short argument name.</param>
        /// <param name="argumentAction">Action for argument.</param>
        public void AddCommandLineArgumentDescription(string fullArgumentName, string shortArgumentName, Action<string> argumentAction)
        {
            if (fullArgumentName is null)
            {
                throw new ArgumentNullException(nameof(fullArgumentName));
            }

            if (shortArgumentName is null)
            {
                throw new ArgumentNullException(nameof(shortArgumentName));
            }

            if (shortArgumentName is null)
            {
                throw new ArgumentNullException(nameof(argumentAction));
            }

            var description = new Tuple<string, string, Action<string>>(fullArgumentName.ToLower(CultureInfo.CurrentCulture), shortArgumentName.ToLower(CultureInfo.CurrentCulture), argumentAction);
            this.commandLineArgumentDescriptions.Add(description);
        }

        /// <summary>
        /// Parse command line arguments.
        /// </summary>
        /// <param name="args">Given command line arguments.</param>
        public void ParseCommandLineArguments(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (args.Length.Equals(0))
            {
                return;
            }

            bool wasShortArgument = false;
            int index = 0;
            foreach (var arg in args)
            {
                var currentArg = arg.ToLower(CultureInfo.CurrentCulture);
                if (wasShortArgument)
                {
                    this.commandLineArgumentDescriptions[index].Item3(currentArg);
                    wasShortArgument = false;
                    continue;
                }
                else if (currentArg.StartsWith("--", StringComparison.CurrentCulture) && !wasShortArgument)
                {
                    var splitedArgs = currentArg.Split('=');
                    if (splitedArgs.Length == 2)
                    {
                        index = this.commandLineArgumentDescriptions.FindIndex(description => description.Item1 == splitedArgs[0]);
                        if (index != -1)
                        {
                            this.commandLineArgumentDescriptions[index].Item3(splitedArgs[1]);
                            wasShortArgument = false;
                            continue;
                        }
                    }
                }
                else if (currentArg.StartsWith("-", StringComparison.CurrentCulture) && !wasShortArgument)
                {
                    index = this.commandLineArgumentDescriptions.FindIndex(description => description.Item2 == currentArg);
                    if (index != -1)
                    {
                        wasShortArgument = true;
                        continue;
                    }
                }

                throw new ArgumentException("Unable command line arguments");
            }
        }
    }
}

using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace FileCabinetApp
{
    /// <summary>
    /// UppdateCommandHandler.
    /// </summary>
    public class UppdateCommandHandler : FileCabinetServiceCommandHandlerBase
    {
        private const string Command = "update";
        private const string Where = "where";

        /// <summary>
        /// Initializes a new instance of the <see cref="UppdateCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service.</param>
        public UppdateCommandHandler(IFileCabinetService service)
            : base(Command, service)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (this.CheckCommand(commandRequest))
            {
                var result = ParseArguments(commandRequest.Parameters, out Action<FileCabinetRecord> action, out Query query);
                if (result.Item1)
                {
                    this.Uppdate(action, query);
                }
                else
                {
                    Console.WriteLine(result.Item2);
                }
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static Tuple<bool, string> ParseArguments(string parameters, out Action<FileCabinetRecord> action, out Query query)
        {
            action = null;
            query = new Query();
            int index = parameters.IndexOf(Where, 0, StringComparison.InvariantCultureIgnoreCase);
            if (index < 1 || index == parameters.Length)
            {
                return new (false, "Invalid arguments)");
            }

            var whereString = parameters.Substring(index, parameters.Length - index);
            var setString = parameters.Substring(0, index);

            var result1 = new Parser().WhereParser(whereString, out query);
            if (!result1.Item1)
            {
                return result1;
            }

            var result2 = new Parser().SetParser(setString, out action);
            if (!result2.Item1)
            {
                return result2;
            }

            return new (true, string.Empty);
        }

        private void Uppdate(Action<FileCabinetRecord> action, Query query)
        {
            var changedRecords = this.Service.Update(query, action);
            Console.WriteLine(StringManager.Rm.GetString("RecordsChangedString", CultureInfo.CurrentCulture), changedRecords);
        }
    }
}

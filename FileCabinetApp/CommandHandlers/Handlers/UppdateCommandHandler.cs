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
        private const string Set = "set";
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
            if (this.CheckCommand(commandRequest) && ParseArguments(commandRequest.Parameters, out Action<FileCabinetRecord> action, out Query query))
            {
                this.Uppdate(action, query);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static bool ParseArguments(string parameters, out Action<FileCabinetRecord> action, out Query query)
        {
            action = null;
            query = new Query();
            var splitParameters = Regex.Split(parameters, $@"({Set})|({Where})").Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            if (splitParameters.Length != 4)
            {
                return false;
            }

            if (!splitParameters[0].Equals(Set) || !splitParameters[2].Equals(Where))
            {
                return false;
            }

            var result1 = new Parser().WhereParser(splitParameters[3], out query);
            if (!result1.Item1)
            {
                Console.WriteLine(result1.Item2);
                return false;
            }

            var result2 = new Parser().SetParser(splitParameters[1], out action);
            if (!result2.Item1)
            {
                Console.WriteLine(result2.Item2);
                return false;
            }

            return true;
        }

        private void Uppdate(Action<FileCabinetRecord> action, Query query)
        {
            var changedRecords = this.Service.Update(query, action);
            Console.WriteLine(StringManager.Rm.GetString("RecordsChangedString", CultureInfo.CurrentCulture), changedRecords);
        }
    }
}

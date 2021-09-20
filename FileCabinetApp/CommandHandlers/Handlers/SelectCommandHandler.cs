using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Select command handler.
    /// </summary>
    public class SelectCommandHandler : FileCabinetServiceCommandHandlerBase
    {
        private const string Command = "select";
        private const string Where = "where";
        private readonly Action<string[], IEnumerable<List<string>>> printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service.</param>
        /// <param name="printer">Printer.</param>
        public SelectCommandHandler(IFileCabinetService service, Action<string[], IEnumerable<List<string>>> printer)
            : base(Command, service)
        {
            this.printer = printer;
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (this.CheckCommand(commandRequest) && ParseParameters(commandRequest.Parameters, out Func<FileCabinetRecord, List<string>> parametersGetter, out string[] headers, out Query query))
            {
                var parameters = this.Select(parametersGetter, query);
                this.printer(headers, parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static bool ParseParameters(string parameters, out Func<FileCabinetRecord, List<string>> parametersGetter, out string[] headers, out Query query)
        {
            parametersGetter = null;
            headers = null;
            query = new Query();

            var parser = new Parser();
            if (string.IsNullOrWhiteSpace(parameters))
            {
                query.Predicate = r => true;
                parametersGetter = parser.GlobalParametersGetter;
                headers = Parser.GetAllHeaders();
                return true;
            }

            var splited = parameters.Split(Where, StringSplitOptions.RemoveEmptyEntries);
            if (splited.Length != 2)
            {
                return false;
            }

            headers = splited[0].Split(',').Select(h => h.Trim(' ')).ToArray();
            if (!parser.SelectParser(splited[0], out parametersGetter))
            {
                return false;
            }

            if (!parser.WhereParser(splited[1], out query).Item1)
            {
                return false;
            }

            return true;
        }

        private IEnumerable<List<string>> Select(Func<FileCabinetRecord, List<string>> parametersGetter, Query query)
        {
            return this.Service.SelectParameters(query, parametersGetter);
        }
    }
}

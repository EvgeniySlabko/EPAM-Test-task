using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            if (this.CheckCommand(commandRequest))
            {
                var result = ParseParameters(commandRequest.Parameters, out Func<FileCabinetRecord, List<string>> parametersGetter, out string[] headers, out Query query);
                if (result.Item1)
                {
                    var parameters = this.Select(parametersGetter, query);
                    this.printer(headers, parameters);
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

        private static Tuple<bool, string> ParseParameters(string parameters, out Func<FileCabinetRecord, List<string>> parametersGetter, out string[] headers, out Query query)
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
                return new (true, string.Empty);
            }

            var queryStartIndex = parameters.IndexOf(Where, StringComparison.InvariantCultureIgnoreCase);
            var selectString = parameters.Substring(0, (queryStartIndex < 0) ? parameters.Length : queryStartIndex);
            var queryString = (queryStartIndex < 0) ? string.Empty : parameters.Substring(queryStartIndex, parameters.Length - queryStartIndex);

            if (string.IsNullOrWhiteSpace(selectString))
            {
                headers = Parser.GetAllHeaders();
                selectString = string.Join(',', Parser.GetAllHeaders());
            }
            else
            {
                headers = selectString.Split(',').Select(h => h.Trim(' ')).ToArray();
            }

            var selectParserResult = parser.SelectParser(selectString, out parametersGetter);
            if (!selectParserResult.Item1)
            {
                return selectParserResult;
            }

            var whereParserResult = parser.WhereParser(queryString, out query);
            if (!whereParserResult.Item1)
            {
                return whereParserResult;
            }

            return new (true, string.Empty);
        }

        private IEnumerable<List<string>> Select(Func<FileCabinetRecord, List<string>> parametersGetter, Query query)
        {
            return this.Service.SelectParameters(query, parametersGetter);
        }
    }
}

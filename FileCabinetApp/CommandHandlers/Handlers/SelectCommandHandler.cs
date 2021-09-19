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
            if (this.CheckCommand(commandRequest) && ParseParameters(commandRequest.Parameters, out Predicate<FileCabinetRecord> predicate, out Func<FileCabinetRecord, List<string>> parametersGetter, out string[] headers))
            {
                var parameters = this.Select(parametersGetter, predicate);
                this.printer(headers, parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static bool ParseParameters(string parameters, out Predicate<FileCabinetRecord> predicate, out Func<FileCabinetRecord, List<string>> parametersGetter, out string[] headers)
        {
            predicate = null;
            parametersGetter = null;
            headers = null;

            var parser = new Parser();
            if (string.IsNullOrWhiteSpace(parameters))
            {
                predicate = r => true;
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

            if (!parser.WhereParser(splited[1], out predicate).Item1)
            {
                return false;
            }

            return true;
        }

        private IEnumerable<List<string>> Select(Func<FileCabinetRecord, List<string>> parametersGetter, Predicate<FileCabinetRecord> predicate)
        {
            return this.Service.SelectParameters(predicate, parametersGetter);
        }
    }
}

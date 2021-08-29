using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// handler for find command.
    /// </summary>
    public class FindCommandHandler : FileCabinetServiceCommandHandlerBase
    {
        private const string Command = "find";

        private readonly IRecordPrinter printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service.</param>
        /// <param name="printer">Printer.</param>
        public FindCommandHandler(IFileCabinetService service, IRecordPrinter printer)
            : base(Command, service)
        {
            this.printer = printer;
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (this.CheckCommand(commandRequest) && ParseParameters(commandRequest.Parameters, out RecordParameter recordParameter, out object searchParameter))
            {
                this.Find(recordParameter, searchParameter);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static bool ParseParameters(string parameters, out RecordParameter recordParameter, out object searchParameter)
        {
            recordParameter = default;
            searchParameter = default;
            var splitedParameters = parameters.Split(' ');

            if (splitedParameters.Length != 2)
            {
                return false;
            }

            try
            {
                recordParameter = (RecordParameter)Enum.Parse(typeof(RecordParameter), splitedParameters[0], true);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }

            if (recordParameter.Equals(RecordParameter.DateOfBirth))
            {
                var convertResult = new DateTimeConverter().Convert(splitedParameters[1]);
                if (!convertResult.Item1)
                {
                    Console.WriteLine(convertResult.Item2, CultureInfo.CurrentCulture);
                    return false;
                }

                searchParameter = convertResult.Item3;
            }
            else if (splitedParameters[1][0] == '"' && splitedParameters[1][^1] == '"')
            {
                searchParameter = splitedParameters[1][1..^1];
            }
            else
            {
                return false;
            }

            return true;
        }

        private void Find(RecordParameter recordParameter, object objectForFind)
        {
            ReadOnlyCollection<FileCabinetRecord> subList;
            switch (recordParameter)
            {
                case RecordParameter.FirstName:
                    subList = this.Service.FindByFirstName((string)objectForFind);
                    break;

                case RecordParameter.LastName:
                    subList = this.Service.FindByLastName((string)objectForFind);
                    break;

                case RecordParameter.DateOfBirth:
                    subList = this.Service.FindByDate((DateTime)objectForFind);
                    break;

                default:
                    Console.WriteLine(StringManager.Rm.GetString("InvalidArgumentsMessage", CultureInfo.CurrentCulture));
                    return;
            }

            if (subList is null)
            {
                Console.WriteLine(StringManager.Rm.GetString("RecordFindMissMessage", CultureInfo.CurrentCulture));
                return;
            }

            this.printer.Print(subList);
        }
    }
}

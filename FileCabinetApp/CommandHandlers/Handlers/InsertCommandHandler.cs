using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// InsertCommandHandler.
    /// </summary>
    public class InsertCommandHandler : FileCabinetServiceCommandHandlerBase
    {
        private const string Command = "insert";
        private readonly Dictionary<string, ConverterType> typer = new ()
        {
            { "firstname", ConverterType.StringConverter },
            { "lastname", ConverterType.StringConverter },
            { "dateofbirth", ConverterType.DateTimeConverter },
            { "identificationnumber", ConverterType.DecimalConverter },
            { "points", ConverterType.ShortConverter },
            { "letter", ConverterType.CharConverter },
            { "id", ConverterType.IntConverter },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service.</param>
        public InsertCommandHandler(IFileCabinetService service)
            : base(Command, service)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (this.CheckCommand(commandRequest) && this.ParseParameters(commandRequest.Parameters, out FileCabinetRecord record))
            {
                this.Create(record);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private bool ParseParameters(string parameters, out FileCabinetRecord record)
        {
            record = new FileCabinetRecord();
            var separatedByString = parameters.Split("values");
            if (separatedByString.Length != 2)
            {
                return false;
            }

            char[] brackets = { '(', ')', ' ' };
            var separatedByBrackets = separatedByString.Select(s => s.Trim(brackets)).ToArray();
            var fullySeparated = separatedByBrackets.Select(s => s.Split(',')).ToArray();
            if (fullySeparated[0].Length != 7 && fullySeparated[1].Length != 7)
            {
                return false;
            }

            fullySeparated[1] = fullySeparated[1].Select(s => s.Trim('\'')).ToArray();
            for (int i = 0; i < 7; i++)
            {
                var key = fullySeparated[0][i].ToLower(CultureInfo.CurrentCulture);
                if (!this.typer.ContainsKey(fullySeparated[0][i]))
                {
                    return false;
                }

                var converterType = this.typer[key];

                switch (converterType)
                {
                    case ConverterType.CharConverter:
                        var result = new Converter().Convert<char>(fullySeparated[1][i]);
                        if (result.Item1)
                        {
                            record.IdentificationLetter = result.Item3;
                            break;
                        }

                        return false;
                    case ConverterType.StringConverter:
                        if (fullySeparated[0][i].Equals("firstname"))
                        {
                            record.FirstName = fullySeparated[1][i];
                            break;
                        }

                        if (fullySeparated[0][i].Equals("lastname"))
                        {
                            record.LastName = fullySeparated[1][i];
                            break;
                        }

                        return false;
                    case ConverterType.DecimalConverter:
                        var decimalResult = new Converter().Convert<decimal>(fullySeparated[1][i]);
                        if (decimalResult.Item1)
                        {
                            record.IdentificationNumber = decimalResult.Item3;
                            break;
                        }

                        return false;
                    case ConverterType.DateTimeConverter:
                        var dateResult = new Converter().Convert<DateTime>(fullySeparated[1][i]);
                        if (dateResult.Item1)
                        {
                            record.DateOfBirth = dateResult.Item3;
                            break;
                        }

                        return false;
                    case ConverterType.IntConverter:
                        var intResult = new Converter().Convert<int>(fullySeparated[1][i]);
                        if (intResult.Item1)
                        {
                            record.Id = intResult.Item3;
                            break;
                        }

                        return false;
                    case ConverterType.ShortConverter:
                        var shortResult = new Converter().Convert<short>(fullySeparated[1][i]);
                        if (shortResult.Item1)
                        {
                            record.PointsForFourTests = shortResult.Item3;
                            break;
                        }

                        return false;
                    default:
                        throw new ArgumentException(nameof(converterType));
                }
            }

            return true;
        }

        /// <summary>
        /// Create ne Record.
        /// </summary>
        /// <param name="record">Given record.</param>
        private void Create(FileCabinetRecord record)
        {
            int recordId;
            try
            {
                recordId = this.Service.CreateRecord(record, false);
            }
            catch (ArgumentException exeption)
            {
                Console.Write(exeption.Message);
                return;
            }

            Console.WriteLine(StringManager.Rm.GetString("CreateRecordMessage", CultureInfo.CurrentCulture), recordId);
        }
    }
}

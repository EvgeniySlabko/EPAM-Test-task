using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Delete command handler.
    /// </summary>
    public class DeleteCommandHandler : FileCabinetServiceCommandHandlerBase
    {
        private const string Command = "delete";

        private readonly Dictionary<string, Tuple<Func<string, object>, Func<object, Predicate<FileCabinetRecord>>>> predicateMapper = new ()
        {
            { "firstname", new Tuple<Func<string, object>, Func<object, Predicate<FileCabinetRecord>>>(Converter.TryConvertToObject<string>, o => r => r.FirstName.Equals((string)o)) },
            { "lastName", new Tuple<Func<string, object>, Func<object, Predicate<FileCabinetRecord>>>(Converter.TryConvertToObject<string>, o => r => r.LastName.Equals((string)o)) },
            { "dateofbirth", new Tuple<Func<string, object>, Func<object, Predicate<FileCabinetRecord>>>(Converter.TryConvertToObject<DateTime>, o => r => r.DateOfBirth.Equals((DateTime)o)) },
            { "identificationNumber", new Tuple<Func<string, object>, Func<object, Predicate<FileCabinetRecord>>>(Converter.TryConvertToObject<decimal>, o => r => r.IdentificationNumber.Equals((decimal)o)) },
            { "points", new Tuple<Func<string, object>, Func<object, Predicate<FileCabinetRecord>>>(Converter.TryConvertToObject<short>, o => r => r.PointsForFourTests.Equals((short)o)) },
            { "id", new Tuple<Func<string, object>, Func<object, Predicate<FileCabinetRecord>>>(Converter.TryConvertToObject<int>, o => r => r.Id.Equals((int)o)) },
            { "letter", new Tuple<Func<string, object>, Func<object, Predicate<FileCabinetRecord>>>(Converter.TryConvertToObject<char>, o => r => r.IdentificationLetter.Equals((char)o)) },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service.</param>
        public DeleteCommandHandler(IFileCabinetService service)
            : base(Command, service)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (this.CheckCommand(commandRequest) && this.ParseParameters(commandRequest.Parameters, out Predicate<FileCabinetRecord> predicate))
            {
                this.Delete(predicate);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private bool ParseParameters(string parameters, out Predicate<FileCabinetRecord> predicate)
        {
            predicate = null;
            var separatedParameters = parameters.Split(' ', 2);
            if (separatedParameters.Length != 2 || !separatedParameters[0].Equals("where"))
            {
                return false;
            }

            var separatedParameters2 = separatedParameters[1].Replace(" ", string.Empty).Split('=');
            if (separatedParameters2.Length != 2)
            {
                return false;
            }

            separatedParameters2[1] = separatedParameters2[1].Trim('\'');
            if (!this.predicateMapper.TryGetValue(separatedParameters2[0], out Tuple<Func<string, object>, Func<object, Predicate<FileCabinetRecord>>> result))
            {
                return false;
            }

            try
            {
                var convertedValue = result.Item1(separatedParameters2[1]);
                predicate = result.Item2(convertedValue);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        private void Delete(Predicate<FileCabinetRecord> predicate)
        {
            var ids = this.Service.Delete(predicate);
            if (ids.Count != 0)
            {
                var idsStr = ids.Select(i => "#" + i.ToString(CultureInfo.CurrentCulture)).ToArray();
                var result = string.Join(", ", idsStr);
                Console.WriteLine($"Records {result} are deleted.");
            }
            else
            {
                Console.WriteLine(StringManager.Rm.GetString("NoRecordsMatchingTheseParameters", CultureInfo.CurrentCulture));
            }
        }
    }
}

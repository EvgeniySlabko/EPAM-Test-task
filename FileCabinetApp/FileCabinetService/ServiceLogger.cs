using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Service logger.
    /// </summary>
    public class ServiceLogger : IFileCabinetService, IDisposable
    {
        private const string DateFormat = "dd/MM/yyyy";
        private readonly TextWriter writer;
        private readonly IFileCabinetService service;
        private readonly string logFileName = "logs.txt";

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        /// <param name="service">Service for logging.</param>
        public ServiceLogger(IFileCabinetService service)
        {
            this.service = service;
            this.writer = new StreamWriter(this.logFileName);
        }

        /// <inheritdoc/>
        public int CreateRecord(FileCabinetRecord newRecord, bool generateNewId)
        {
            if (newRecord is null)
            {
                throw new ArgumentNullException(nameof(newRecord));
            }

            this.Log($"Calling Create() with FirstName = {newRecord.FirstName}, LastName = {newRecord.LastName}, DateOfBirth = {newRecord.DateOfBirth.ToString(DateFormat, CultureInfo.CurrentCulture)}, IdentificationNumber = {newRecord.IdentificationNumber}, IdentificationLetter = {newRecord.IdentificationLetter}, Points = {newRecord.PointsForFourTests}");

            var result = this.service.CreateRecord(newRecord, generateNewId);

            this.Log($"Calling Create() returne {result.ToString(CultureInfo.CurrentCulture)}");
            return result;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByDate(DateTime dataOfBirthday)
        {
            this.Log($"Calling FindByDate() with argument {dataOfBirthday.ToString(DateFormat, CultureInfo.CurrentCulture)}");

            var result = this.service.FindByDate(dataOfBirthday);

            this.Log($"Calling FindByDate() succeeded.");

            return result;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            this.Log($"Calling FindByFirstName() with argument {firstName}");

            var result = this.service.FindByFirstName(firstName);

            this.Log($"Calling FindByFirstName() succeeded.");

            return result;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            this.Log($"Calling FindByLastName() with argument {lastName}");

            var result = this.service.FindByLastName(lastName);

            this.Log($"Calling FindByLastName() succeeded.");

            return result;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            this.Log($"Calling GetRecords()");

            var result = this.service.GetRecords();

            this.Log($"Calling GetRecords() succeeded.");

            return result;
        }

        /// <inheritdoc/>
        public Tuple<int, int> GetStat()
        {
            this.Log($"Calling GetStat()");

            var result = this.service.GetStat();

            this.Log($"Calling GetStat() returne ({result.Item1}, {result.Item1}).");

            return result;
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            this.Log($"Calling MakeSnapshot()");

            var result = this.service.MakeSnapshot();

            this.Log($"Calling MakeSnapshot() returne snapshot with {result.Records.Count} records.");

            return result;
        }

        /// <inheritdoc/>
        public void Purge()
        {
            this.Log($"Calling Purge()");

            try
            {
                this.service.Purge();
            }
            catch (NotImplementedException)
            {
                this.Log("Purge() method invalid for this service.");
                throw;
            }

            this.Log($"Calling Purge() succeeded.");
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            this.Log($"Calling Restore() with snapshot with {snapshot.Records.Count} records");

            try
            {
                this.service.Restore(snapshot);
            }
            catch (ArgumentNullException)
            {
                this.Log($"Calling Restore() returne ArgumentNullException");
                throw;
            }

            this.Log($"Calling Restore() succeeded.");
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<int> Delete(Predicate<FileCabinetRecord> predicate)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            this.Log($"Calling Delete()");

            var result = this.service.Delete(predicate);
            this.Log($"Calling Delete() return {result.Count} id. ");
            return result;
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public int Update(Predicate<FileCabinetRecord> predicate, Action<FileCabinetRecord> action)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            this.Log($"Calling Update()");

            var result = this.service.Update(predicate, action);
            this.Log($"Calling pdate() ipdate {result} records. ");
            return result;
        }

        /// <inheritdoc/>
        public IEnumerable<List<string>> SelectParameters(Predicate<FileCabinetRecord> predicate, Func<FileCabinetRecord, List<string>> parameters)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            this.Log($"Calling Select()");
            var result = this.service.SelectParameters(predicate, parameters);
            this.Log($"Calling Select() succeeded.");
            return result;
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing">Disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.writer.Close();
            }
        }

        private void Log(string log)
        {
            var logString = new StringBuilder();
            logString.Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.CurrentCulture));
            logString.Append(' ');
            logString.Append(log);
            this.writer.WriteLine(logString.ToString());
            this.writer.Flush();
        }
    }
}

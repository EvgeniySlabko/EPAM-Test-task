using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;

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
            this.writer = new StreamWriter(this.logFileName, true);
        }

        /// <inheritdoc/>
        public int CreateRecord(ValidationRecord newRecord)
        {
            if (newRecord is null)
            {
                throw new ArgumentNullException(nameof(newRecord));
            }

            this.Log($"Calling Create() with FirstName = {newRecord.FirstName}, LastName = {newRecord.LastName}, DateOfBirth = {newRecord.DateOfBirth.ToString(DateFormat, CultureInfo.CurrentCulture)}, IdentificationNumber = {newRecord.IdentificationNumber}, IdentificationLetter = {newRecord.IdentificationLetter}, Points = {newRecord.PointsForFourTests}");

            var result = this.service.CreateRecord(newRecord);

            this.Log($"Calling Create() returne {result.ToString(CultureInfo.CurrentCulture)}");
            return result;
        }

        /// <inheritdoc/>
        public int Insert(FileCabinetRecord newRecord)
        {
            if (newRecord is null)
            {
                throw new ArgumentNullException(nameof(newRecord));
            }

            this.Log($"Calling Create() with FirstName = {newRecord.FirstName}, LastName = {newRecord.LastName}, DateOfBirth = {newRecord.DateOfBirth.ToString(DateFormat, CultureInfo.CurrentCulture)}, IdentificationNumber = {newRecord.IdentificationNumber}, IdentificationLetter = {newRecord.IdentificationLetter}, Points = {newRecord.PointsForFourTests}");

            var result = this.service.Insert(newRecord);

            this.Log($"Calling Create() returne {result.ToString(CultureInfo.CurrentCulture)}");
            return result;
        }

        /// <inheritdoc/>
        public Tuple<int, int> GetStat()
        {
            this.Log($"Calling GetStat()");

            var result = this.service.GetStat();

            this.Log($"Calling GetStat() return ({result.Item1}, {result.Item1}).");

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
        public int Purge()
        {
            this.Log($"Calling Purge()");

            var result = this.service.Purge();

            this.Log($"Calling Purge() succeeded. {result} were purged");
            return result;
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
        public ReadOnlyCollection<int> Delete(Query query)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            this.Log($"Calling Delete()");

            var result = this.service.Delete(query);

            this.Log($"Calling Delete() return {result.Count} id. ");
            return result;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public int Update(Query query, Action<FileCabinetRecord> action)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            this.Log($"Calling Update()");

            var result = this.service.Update(query, action);
            this.Log($"Calling update() updates {result} records. ");
            return result;
        }

        /// <inheritdoc/>
        public IEnumerable<List<string>> SelectParameters(Query query, Func<FileCabinetRecord, List<string>> parameters)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            this.Log($"Calling Select(). Query hash - {query.Hash}");
            var result = this.service.SelectParameters(query, parameters);
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

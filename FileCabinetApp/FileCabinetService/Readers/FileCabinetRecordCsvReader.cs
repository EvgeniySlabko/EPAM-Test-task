using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Reader for Csv files.
    /// </summary>
    public class FileCabinetRecordCsvReader : IDisposable
    {
        private readonly string validationMessage = "Record with id {0} did not pass validation";
        private readonly string couldNotReadTheRecordMessage = "Сould not read the record";
        private readonly StreamReader reader;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.
        /// </summary>
        /// <param name="reader">Given reader.</param>
        public FileCabinetRecordCsvReader(FileStream reader)
        {
            this.reader = new StreamReader(reader);
        }

        /// <summary>
        /// Dispose method for StramReader.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Read all records.
        /// </summary>
        /// <returns>List of records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            var records = new List<FileCabinetRecord>();

            // Skip first info line.
            this.reader.ReadLine();

            while (!this.reader.EndOfStream)
            {
                var record = this.ReadOneRecord();

                if (record is null)
                {
                    Console.WriteLine(this.couldNotReadTheRecordMessage);
                    continue;
                }

                if (new ValidatorBuilder().CreateDefault().ValidateParameters(record))
                {
                    records.Add(record);
                }
                else
                {
                    Console.WriteLine(this.validationMessage, record.Id);
                }
            }

            return records;
        }

        /// <summary>
        /// Implementation of IDisposable.
        /// </summary>
        /// <param name="disposing">Disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.reader.Close();
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// Create record from file.
        /// </summary>
        /// <returns>Read recond.</returns>
        private FileCabinetRecord ReadOneRecord()
        {
            var paramaters = this.reader.ReadLine().Split(", ", StringSplitOptions.RemoveEmptyEntries);
            if (paramaters.Length != 7)
            {
                return null;
            }

            var idresult = new Converter().Convert<int>(paramaters[0]);
            var firstName = paramaters[1].Trim();
            var lastName = paramaters[2].Trim();
            var dateOfBirthResult = new Converter().Convert<DateTime>(paramaters[3]);
            var identificationNumberResult = new Converter().Convert<decimal>(paramaters[4]);
            var identificationLetterResult = new Converter().Convert<char>(paramaters[5]);
            var pointsForFourTestsResult = new Converter().Convert<short>(paramaters[6]);

            if (!idresult.Item1 || !dateOfBirthResult.Item1 || !identificationNumberResult.Item1 || !pointsForFourTestsResult.Item1 || !identificationLetterResult.Item1)
            {
                return null;
            }

            return new FileCabinetRecord()
            {
                Id = idresult.Item3,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirthResult.Item3,
                PointsForFourTests = pointsForFourTestsResult.Item3,
                IdentificationNumber = identificationNumberResult.Item3,
                IdentificationLetter = identificationLetterResult.Item3,
            };
        }
    }
}

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Writes records to cvc File.
    /// </summary>
    public class FileCabinetRecordCsvWriter
    {
        private readonly TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="writer">Given TextWriter instance.</param>
        public FileCabinetRecordCsvWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Write string to file.
        /// </summary>
        /// <param name="str">String to write.</param>
        public void Write(string str)
        {
            if (str is null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            this.writer.WriteLine(str);
        }

        /// <summary>
        /// Write record.
        /// </summary>
        /// <param name="record">Record to write.</param>
        public void Write(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            var csvRecord = new StringBuilder();

            csvRecord.Append(record.Id.ToString(CultureInfo.CurrentCulture));
            csvRecord.Append(", ");

            csvRecord.Append(record.FirstName);
            csvRecord.Append(", ");

            csvRecord.Append(record.LastName);
            csvRecord.Append(", ");

            csvRecord.Append(record.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture));
            csvRecord.Append(", ");

            csvRecord.Append(record.IdentificationNumber.ToString(CultureInfo.CurrentCulture));
            csvRecord.Append(", ");

            csvRecord.Append(record.IdentificationLetter);
            csvRecord.Append(", ");

            csvRecord.Append(record.PointsForFourTests.ToString(CultureInfo.CurrentCulture));
            this.writer.WriteLine(csvRecord);
        }
    }
}

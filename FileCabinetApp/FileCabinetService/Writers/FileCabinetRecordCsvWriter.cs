using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Write string.
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

            var cvcRecord = new StringBuilder();

            cvcRecord.Append(record.Id.ToString(CultureInfo.CurrentCulture));
            cvcRecord.Append(", ");

            cvcRecord.Append(record.FirstName);
            cvcRecord.Append(", ");

            cvcRecord.Append(record.LastName);
            cvcRecord.Append(", ");

            cvcRecord.Append(record.DateOfBirth.ToString("dd/MM/yyyy", DateTimeFormatInfo.InvariantInfo));
            cvcRecord.Append(", ");

            cvcRecord.Append(record.IdentificationNumber.ToString(CultureInfo.CurrentCulture));
            cvcRecord.Append(", ");

            cvcRecord.Append(record.IdentificationLetter);
            cvcRecord.Append(", ");

            cvcRecord.Append(record.PointsForFourTests.ToString(CultureInfo.CurrentCulture));
            this.writer.WriteLine(cvcRecord);
        }
    }
}

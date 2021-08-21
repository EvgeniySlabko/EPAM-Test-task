using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Writes records to cvc File.
    /// </summary>
    public class FileCabinetRecordXmlWriter
    {
        private readonly XmlWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="writer">Given XmltWriter instance.</param>
        public FileCabinetRecordXmlWriter(XmlWriter writer)
        {
            this.writer = writer;
            this.writer.WriteStartDocument();
            this.writer.WriteStartElement("records");
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        ~FileCabinetRecordXmlWriter() => this.writer.WriteEndDocument();

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

            this.writer.WriteStartElement("record");
            this.writer.WriteAttributeString("id", record.Id.ToString(CultureInfo.CurrentCulture));

            this.writer.WriteStartElement("name");
            this.writer.WriteAttributeString("last", record.LastName);
            this.writer.WriteAttributeString("first", record.FirstName);
            this.writer.WriteEndElement();

            this.writer.WriteStartElement("dateOfBirth");
            this.writer.WriteString(record.DateOfBirth.ToString("dd/MM/yyyy", DateTimeFormatInfo.InvariantInfo));
            this.writer.WriteEndElement();

            this.writer.WriteStartElement("identificationNumber");
            this.writer.WriteString(record.IdentificationNumber.ToString(CultureInfo.CurrentCulture));
            this.writer.WriteEndElement();

            this.writer.WriteStartElement("identificationLetter");
            this.writer.WriteString(record.IdentificationLetter.ToString(CultureInfo.CurrentCulture));
            this.writer.WriteEndElement();

            this.writer.WriteStartElement("pointsForFourTests");
            this.writer.WriteString(record.PointsForFourTests.ToString(CultureInfo.CurrentCulture));
            this.writer.WriteEndElement();

            this.writer.WriteEndElement();
        }
    }
}

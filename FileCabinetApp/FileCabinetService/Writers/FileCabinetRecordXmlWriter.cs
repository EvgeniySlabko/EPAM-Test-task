using System;
using System.Globalization;
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

            this.writer.WriteElementString("dateOfBirth", record.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

            this.writer.WriteElementString("identificationNumber", record.IdentificationNumber.ToString(CultureInfo.CurrentCulture));

            this.writer.WriteElementString("identificationLetter", record.IdentificationLetter.ToString(CultureInfo.CurrentCulture));

            this.writer.WriteElementString("pointsForFourTests", record.PointsForFourTests.ToString(CultureInfo.CurrentCulture));

            this.writer.WriteEndElement();
        }
    }
}

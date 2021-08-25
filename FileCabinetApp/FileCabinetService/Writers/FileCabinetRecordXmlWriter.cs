using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

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
            //this.writer.WriteStartElement("records");
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


            //using var xmlReader = new XmlTextReader(this.reader);
            var serializer = new XmlSerializer(typeof(FileCabinetRecordsSerializable));

            var r = new FileCabinetRecordSerializable
            {
                DateOfBirth = new DateTime(1999, 10, 10),
                Id = 2,
                IdentificationLetter = 'g',
                IdentificationNumber = 333,
                Name = new NameSerializeble
                {
                    FirstName = "dsf",
                    LastName = "dfs"
                },
                PointsForFourTests = 123,

            };
            var rr = new FileCabinetRecordsSerializable();
            rr.Records.Add(r);
            serializer.Serialize(writer, rr);

            //this.writer.WriteStartElement("record");
            //this.writer.WriteAttributeString("id", record.Id.ToString(CultureInfo.CurrentCulture));

            //this.writer.WriteStartElement("name");
            //this.writer.WriteAttributeString("last", record.LastName);
            //this.writer.WriteAttributeString("first", record.FirstName);
            //this.writer.WriteEndElement();

            //this.writer.WriteElementString("dateOfBirth", record.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

            //this.writer.WriteElementString("identificationNumber", record.IdentificationNumber.ToString(CultureInfo.CurrentCulture));

            //this.writer.WriteElementString("identificationLetter", record.IdentificationLetter.ToString(CultureInfo.CurrentCulture));

            //this.writer.WriteElementString("pointsForFourTests", record.PointsForFourTests.ToString(CultureInfo.CurrentCulture));

            //this.writer.WriteEndElement();
        }
    }
}

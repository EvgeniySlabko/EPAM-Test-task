using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Snapshot class. Stores the state of the list of records file cabinet service.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private const string HeadersString = "Id, First Name, Last Name, Date of Birth, Identification number, Identification letter, Points for four tests";
        private FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">Given records.</param>
        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        public FileCabinetServiceSnapshot()
        {
            this.records = Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Gets records.
        /// </summary>
        /// <value>Read records.</value>
        public IReadOnlyCollection<FileCabinetRecord> Records
        {
            get
            {
                return new ReadOnlyCollection<FileCabinetRecord>(this.records);
            }
        }

        /// <summary>
        /// Load records from Csv file.
        /// </summary>
        /// <param name="stream">Given stream.</param>
        public void LoadFromXml(FileStream stream)
        {
            using var reader = new StreamReader(stream, Encoding.ASCII);
            this.records = new FileCabinerXmlReader(reader).ReadAll().ToArray();
        }

        /// <summary>
        /// Load records from Csv file.
        /// </summary>
        /// <param name="stream">Given stream.</param>
        public void LoadFromCsv(FileStream stream)
        {
            using var reader = new FileCabinetRecordCsvReader(stream);
            this.records = reader.ReadAll().ToArray();
        }

        /// <summary>
        /// Save records to CSV file.
        /// </summary>
        /// <param name="writer">Given StreamWriter object.</param>
        public void SaveToCsv(StreamWriter writer)
        {
            var csvWriter = new FileCabinetRecordCsvWriter(writer);
            csvWriter.Write(HeadersString);
            foreach (var record in this.records)
            {
                csvWriter.Write(record);
            }
        }

        /// <summary>
        /// Save records to xml file.
        /// </summary>
        /// <param name="writer">Given StreamWriter object.</param>
        public void SaveToXml(StreamWriter writer)
        {
            var sts = new XmlWriterSettings()
            {
                Indent = true,
            };

            using var xmlWriter = XmlWriter.Create(writer, sts);
            var fileCabinetRecordXmlWriter = new FileCabinetRecordXmlWriter(xmlWriter);

            foreach (var record in this.records)
            {
                fileCabinetRecordXmlWriter.Write(record);
            }
        }
    }
}

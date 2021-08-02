using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Snapshot class. Stores the state of the list of records file cabinet service.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private readonly FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">Given records.</param>
        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        /// <summary>
        /// Save records to CSV file.
        /// </summary>
        /// <param name="writer">Given StreamWriter object.</param>
        public void SaveToCsv(StreamWriter writer)
        {
            var csvWriter = new FileCabinetRecordCsvWriter(writer);
            csvWriter.Write("Id, First Name, Last Name, Date of Birth, Identification number, Identification letter, Points for four tests");
            foreach (var record in this.records)
            {
                csvWriter.Write(record);
            }
        }

        /// <summary>
        /// Save records to XML file.
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

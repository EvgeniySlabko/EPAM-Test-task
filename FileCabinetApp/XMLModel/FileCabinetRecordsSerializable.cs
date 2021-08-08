using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Model for serialization record.
    /// </summary>
    [XmlRoot(ElementName ="records")]
    public class FileCabinetRecordsSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordsSerializable"/> class.
        /// </summary>
        public FileCabinetRecordsSerializable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordsSerializable"/> class.
        /// </summary>
        /// <param name="fileCabinetRecords">Records.</param>
        public FileCabinetRecordsSerializable(IEnumerable<FileCabinetRecord> fileCabinetRecords)
        {
            if (fileCabinetRecords is null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecords));
            }

            this.Records = new List<FileCabinetRecordSerializable>();
            foreach (var record in fileCabinetRecords)
            {
                this.Records.Add(new FileCabinetRecordSerializable(record));
            }
        }

        /// <summary>
        /// Gets serialization records.
        /// </summary>
        /// <value>
        /// Records.
        /// </value>
        [XmlElement(ElementName = "record")]
        public List<FileCabinetRecordSerializable> Records { get; }
    }
}

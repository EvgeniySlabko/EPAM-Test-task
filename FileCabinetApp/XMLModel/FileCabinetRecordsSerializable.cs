using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Model for record serialization.
    /// </summary>
    [Serializable]
    [XmlRoot("records")]
    public class FileCabinetRecordsSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordsSerializable"/> class.
        /// </summary>
        public FileCabinetRecordsSerializable()
        {
            this.Records = new List<FileCabinetRecordSerializable>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordsSerializable"/> class.
        /// </summary>
        /// <param name="fileCabinetRecords">Records.</param>
        public FileCabinetRecordsSerializable(IEnumerable<FileCabinetRecord> fileCabinetRecords)
            : this()
        {
            if (fileCabinetRecords is null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecords));
            }

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

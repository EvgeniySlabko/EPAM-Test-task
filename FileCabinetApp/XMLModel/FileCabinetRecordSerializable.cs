using System;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Record.
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "record")]
    public class FileCabinetRecordSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordSerializable"/> class.
        /// </summary>
        public FileCabinetRecordSerializable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordSerializable"/> class.
        /// </summary>
        /// <param name="record">Given record.</param>
        public FileCabinetRecordSerializable(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.Name = new NameSerializeble();
            this.Id = record.Id;
            this.Name.FirstName = record.FirstName;
            this.Name.LastName = record.LastName;
            this.DateOfBirth = record.DateOfBirth;
            this.IdentificationNumber = record.IdentificationNumber;
            this.IdentificationLetter = record.IdentificationLetter.ToString();
            this.PointsForFourTests = record.PointsForFourTests;
        }

        /// <summary>
        /// Gets or sets record id.
        /// </summary>
        /// <value>Record id.</value>
        [XmlAttribute("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets record name.
        /// </summary>
        /// <value>Record name.</value>
        [XmlElement("name")]
        public NameSerializeble Name { get; set; }

        /// <summary>
        /// Gets or sets record date of birthday.
        /// </summary>
        /// <value>Record date of birthday.</value>
        [XmlElement(DataType = "date", ElementName = "dateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets identification number.
        /// </summary>
        /// <value>Record identification number.</value>
        [XmlElement("identificationNumber")]
        public decimal IdentificationNumber { get; set; }

        /// <summary>
        /// Gets or sets identification letter.
        /// </summary>
        /// <value>Record identification letter.</value>
        [XmlElement("identificationLetter")]
        public string IdentificationLetter { get; set; }

        /// <summary>
        /// Gets or sets record points for four tests.
        /// </summary>
        /// <value>Record points for four tests.</value>
        [XmlElement("pointsForFourTests")]
        public short PointsForFourTests { get; set; }
    }
}

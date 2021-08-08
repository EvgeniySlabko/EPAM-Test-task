using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Record.
    /// </summary>
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
            this.IdentificationLetter = record.IdentificationLetter;
            this.PointsForFourTests = record.PointsForFourTests;
        }

        /// <summary>
        /// Gets or sets record id.
        /// </summary>
        /// <value>Record id.</value>
        [XmlAttribute("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets record name.
        /// </summary>
        /// <value>Record name.</value>
        [XmlElement("name")]
        public NameSerializeble Name { get; set; }

        /// <summary>
        /// Gets or sets record last name.
        /// </summary>
        /// <value>Record last name.</value>
        [XmlElement("lastName")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets record date of birthday.
        /// </summary>
        /// <value>Record date of birthday.</value>
        [XmlElement("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets record points for four tests.
        /// </summary>
        /// <value>Record points for four tests.</value>
        [XmlElement("points")]
        public short PointsForFourTests { get; set; }

        /// <summary>
        /// Gets or sets identification number.
        /// </summary>
        /// <value>Record identification number.</value>
        [XmlElement("IdentificationNumber")]
        public decimal IdentificationNumber { get; set; }

        /// <summary>
        /// Gets or sets identification letter.
        /// </summary>
        /// <value>Record identification letter.</value>
        [XmlElement("IdentificationLetter")]
        public char IdentificationLetter { get; set; }
    }
}

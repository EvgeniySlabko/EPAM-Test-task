using System;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// First and last record name.
    /// </summary>
    [XmlRoot("name")]
    public class NameSerializeble
    {
        /// <summary>
        /// Gets or sets record first name.
        /// </summary>
        /// <value>Record first name.</value>
        [XmlAttribute("firstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets record first name.
        /// </summary>
        /// <value>Record first name.</value>
        [XmlAttribute("lastName")]
        public string LastName { get; set; }
    }
}

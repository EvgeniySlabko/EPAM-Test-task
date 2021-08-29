using System;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// First and last record name.
    /// </summary>
    [Serializable]
    [XmlRoot("name")]
    public class NameSerializeble
    {
        /// <summary>
        /// Gets or sets record first name.
        /// </summary>
        /// <value>Record first name.</value>
        [XmlAttribute("first")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets record first name.
        /// </summary>
        /// <value>Record first name.</value>
        [XmlAttribute("last")]
        public string LastName { get; set; }
    }
}

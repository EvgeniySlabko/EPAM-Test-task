using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Xml reader.
    /// </summary>
    public class FileCabinerXmlReader
    {
        private readonly StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinerXmlReader"/> class.
        /// </summary>
        /// <param name="reader">Given reader.</param>
        public FileCabinerXmlReader(StreamReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Read all records.
        /// </summary>
        /// <returns>List of records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            var records = new List<FileCabinetRecord>();

            this.reader.BaseStream.Position = 0;

            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            using var xmlReader = new XmlTextReader(this.reader);
            var serializer = new XmlSerializer(typeof(FileCabinetRecordsSerializable));
            var recordRepository = (FileCabinetRecordsSerializable)serializer.Deserialize(xmlReader);

            foreach (var record in recordRepository.Records)
            {
                records.Add(CreateFileCabinetRecord(record));
            }

            return records;
        }

        /// <summary>
        /// Create FileCabinet record from FileCabinetRecordSerializable.
        /// </summary>
        /// <param name="recordSerializeble">Given FileCabinetRecordSerializable.</param>
        /// <returns>FileCabinetRecord.</returns>
        private static FileCabinetRecord CreateFileCabinetRecord(FileCabinetRecordSerializable recordSerializeble)
        {
            return new FileCabinetRecord()
            {
                Id = recordSerializeble.Id,
                FirstName = recordSerializeble.Name.FirstName,
                LastName = recordSerializeble.Name.LastName,
                DateOfBirth = recordSerializeble.DateOfBirth,
                IdentificationNumber = recordSerializeble.IdentificationNumber,
                IdentificationLetter = recordSerializeble.IdentificationLetter,
                PointsForFourTests = recordSerializeble.PointsForFourTests,
            };
        }
    }
}

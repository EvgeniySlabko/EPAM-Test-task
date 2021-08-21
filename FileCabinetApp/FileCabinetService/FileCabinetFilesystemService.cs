using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Service for working with records in memory.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService, IDisposable
    {
        private const int RecordSize = 277;
        private const int MaxNameLength = 120;
        private readonly IRecordValidator recordValidator;

        private readonly FileStream fileStrieam;
        private readonly BinaryWriter binaryWriter;
        private readonly BinaryReader binaryReader;

        private int id;
        private int iterationIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="recordValidator">Given validator.</param>
        public FileCabinetFilesystemService(IRecordValidator recordValidator)
        {
            this.fileStrieam = new FileStream("cabinet-records.db", FileMode.OpenOrCreate);
            this.binaryReader = new BinaryReader(this.fileStrieam);
            this.binaryWriter = new BinaryWriter(this.fileStrieam);
            this.recordValidator = recordValidator;

            this.id = this.GetHigherId() + 1;
        }

        /// <summary>
        /// Create new record.
        /// </summary>
        /// <param name="record">Record to add.</param>
        /// <param name="generateNewId">determines whether a new id needs to be generated.</param>
        /// <returns>id of the new record.</returns>
        public int CreateRecord(FileCabinetRecord record, bool generateNewId)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (!this.recordValidator.ValidateParameters(record))
            {
                throw new ArgumentException("Invalide parameters");
            }

            record.Id = generateNewId ? this.id++ : record.Id;
            this.Write(record);
            return record.Id;
        }

        /// <summary>
        /// Implementation IDisposable.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Edits the record by its id.
        /// </summary>
        /// <param name="record">Edited record.</param>
        public void Edit(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.GoToStart();
            while (true)
            {
                var currentRecord = this.GetNext();
                if (record is null)
                {
                    throw new ArgumentException($"record with id {currentRecord.Id} not found");
                }
                else if (currentRecord.Id == record.Id)
                {
                    this.Write(record, this.iterationIndex - 1);
                    break;
                }
            }
        }

        /// <summary>
        /// Find record by its data of birthday.
        /// </summary>
        /// <param name="dataOfBirthday">Вata of birthday to search.</param>
        /// <returns>Record if found otherwise null.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDate(DateTime dataOfBirthday)
        {
            bool Comparator(FileCabinetRecord record) => record.DateOfBirth == dataOfBirthday;
            return this.FindBy(Comparator);
        }

        /// <summary>
        /// Find record by its first name.
        /// </summary>
        /// <param name="firstName">First name to search.</param>
        /// <returns>Record if found otherwise null.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (firstName is null)
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            bool Comparator(FileCabinetRecord record) => record.FirstName.ToLower(CultureInfo.CurrentCulture) == firstName.ToLower(CultureInfo.CurrentCulture);
            return this.FindBy(Comparator);
        }

        /// <summary>
        /// Find record by its last name.
        /// </summary>
        /// <param name="lastName">Last name to search.</param>
        /// <returns>Record if found otherwise null.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (lastName is null)
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            bool Comparator(FileCabinetRecord record) => record.LastName.ToLower(CultureInfo.CurrentCulture) == lastName.ToLower(CultureInfo.CurrentCulture);
            return this.FindBy(Comparator);
        }

        /// <summary>
        /// Returns all records.
        /// </summary>
        /// <returns>array with records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            var records = new List<FileCabinetRecord>();
            this.GoToStart();
            while (true)
            {
                var record = this.GetNext();
                if (record is null)
                {
                    this.GoToStart();
                    break;
                }
                else
                {
                    records.Add(record);
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <summary>
        /// Returns the number of records in the list.
        /// </summary>
        /// <returns>Number of entries in the list.</returns>
        public int GetStat()
        {
            int i = 0;
            while (true)
            {
                var record = this.GetNext();
                if (record is null)
                {
                    break;
                }
                else
                {
                    i++;
                }
            }

            this.GoToStart();
            return i;
        }

        /// <summary>
        /// Takes a snapshot of the current state of the list of records.
        /// </summary>
        /// <returns>Snapshot of the current list of records.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            var records = this.GetRecords();
            var recordsArray = new FileCabinetRecord[records.Count];
            records.CopyTo(recordsArray, 0);
            return new FileCabinetServiceSnapshot(recordsArray);
        }

        /// <summary>
        /// Restore records from snapshot.
        /// </summary>
        /// <param name="snapshot">Given snapshot.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            foreach (var newRecord in snapshot.Records)
            {
                this.GoToStart();
                int i = 0;
                while (true)
                {
                    var record = this.GetNext();
                    if (record is null || record.Id == newRecord.Id)
                    {
                        this.Write(newRecord, i);
                        break;
                    }

                    i++;
                }

                this.GoToStart();
            }
        }

        /// <inheritdoc/>
        public void Remove(int id)
        {
            int i = 0;
            this.GoToStart();
            while (true)
            {
                var record = this.GetNextAny();
                if (record is null)
                {
                    throw new ArgumentException($"Record {id} does not exists.");
                }
                else if (record.Record.Id.Equals(id) && (record.ServiceInormation & 4) == 0)
                {
                    record.ServiceInormation |= 4;
                    this.Write(record, this.iterationIndex - 1);
                    break;
                }

                i++;
            }

            this.GoToStart();
        }

        /// <summary>
        /// Implementation IDisposable.
        /// </summary>
        /// <param name="disposing">Flag for disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.binaryReader.Close();
                this.binaryWriter.Close();
                this.fileStrieam.Close();
            }
        }

        private void Write(FileCabinetRecord record, int index)
        {
            var fileSystemRecord = new FileCabonetFilesystemRecord
            {
                ServiceInormation = 0,
                Record = record,
            };

            this.Write(fileSystemRecord, index);
        }

        private void Write(FileCabinetRecord record)
        {
            var fileSystemRecord = new FileCabonetFilesystemRecord
            {
                ServiceInormation = 0,
                Record = record,
            };

            this.Write(fileSystemRecord, (int)this.binaryWriter.BaseStream.Length / RecordSize);
        }

        private void Write(FileCabonetFilesystemRecord record, int index)
        {
            this.binaryWriter.BaseStream.Position = index * RecordSize;
            this.binaryWriter.Write(record.ServiceInormation);
            this.binaryWriter.Write(record.Record.Id);
            this.WriteANSIIStringToFile(record.Record.FirstName);
            this.WriteANSIIStringToFile(record.Record.LastName);
            this.binaryWriter.Write(record.Record.DateOfBirth.Year);
            this.binaryWriter.Write(record.Record.DateOfBirth.Month);
            this.binaryWriter.Write(record.Record.DateOfBirth.Day);
            this.binaryWriter.Write(record.Record.IdentificationNumber);
            this.binaryWriter.Write(record.Record.PointsForFourTests);
            this.binaryWriter.Write(record.Record.IdentificationLetter);
            this.binaryWriter.Flush();
        }

        private void WriteANSIIStringToFile(string str)
        {
            byte[] nameBytes = Encoding.ASCII.GetBytes(str);
            var stringBuffer = new byte[MaxNameLength];
            int stringLength = (nameBytes.Length > MaxNameLength) ? MaxNameLength : nameBytes.Length;

            Array.Copy(nameBytes, 0, stringBuffer, 0, stringLength);
            this.binaryWriter.Write(stringBuffer);
        }

        private FileCabonetFilesystemRecord GetRecord(int index)
        {
            int position = index * RecordSize;
            if (position + RecordSize > this.binaryReader.BaseStream.Length)
            {
                return null;
            }

            var fileSystemRecord = new FileCabonetFilesystemRecord();
            this.binaryReader.BaseStream.Position = position;

            fileSystemRecord.ServiceInormation = this.binaryReader.ReadInt16();
            fileSystemRecord.Record = new FileCabinetRecord()
            {
                Id = this.binaryReader.ReadInt32(),
                FirstName = Encoding.ASCII.GetString(this.binaryReader.ReadBytes(MaxNameLength), 0, MaxNameLength).Trim('\0'),
                LastName = Encoding.ASCII.GetString(this.binaryReader.ReadBytes(MaxNameLength), 0, MaxNameLength).Trim('\0'),
                DateOfBirth = DateTime.Parse($"{this.binaryReader.ReadInt32()}/{this.binaryReader.ReadInt32()}/{this.binaryReader.ReadInt32()}", CultureInfo.InvariantCulture),
                IdentificationNumber = this.binaryReader.ReadDecimal(),
                PointsForFourTests = this.binaryReader.ReadInt16(),
                IdentificationLetter = this.binaryReader.ReadChar(),
            };

            return fileSystemRecord;
        }

        private FileCabinetRecord GetNext()
        {
            while (true)
            {
                var filesystemecord = this.GetRecord(this.iterationIndex++);
                if (filesystemecord is null)
                {
                    return null;
                }

                if ((filesystemecord.ServiceInormation & 4) == 4)
                {
                    return this.GetNext();
                }
                else
                {
                    return filesystemecord.Record;
                }
            }
        }

        private FileCabonetFilesystemRecord GetNextAny()
        {
            return this.GetRecord(this.iterationIndex++);
        }

        private void GoToStart()
        {
            this.iterationIndex = 0;
        }

        private ReadOnlyCollection<FileCabinetRecord> FindBy(Predicate<FileCabinetRecord> comparator)
        {
            var subList = new List<FileCabinetRecord>();
            this.GoToStart();
            while (true)
            {
                var fileSystemRecord = this.GetNext();
                if (fileSystemRecord is null)
                {
                    break;
                }
                else if (comparator(fileSystemRecord))
                {
                    subList.Add(fileSystemRecord);
                }
            }

            this.GoToStart();
            return new ReadOnlyCollection<FileCabinetRecord>(subList);
        }

        private int GetHigherId()
        {
            int higherId = 0;
            while (true)
            {
                var fileSystemRecord = this.GetNext();
                if (fileSystemRecord is null)
                {
                    this.GoToStart();
                    break;
                }
                else if (fileSystemRecord.Id > higherId)
                {
                    higherId = fileSystemRecord.Id;
                }
            }

            return higherId;
        }
    }
}

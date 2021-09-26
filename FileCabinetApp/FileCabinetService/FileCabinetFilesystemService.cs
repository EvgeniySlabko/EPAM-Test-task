using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
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

        private readonly Dictionary<int, int> recordsIdDictionary = new ();

        private readonly Memorizer memorizer = new ();

        private int id;
        private int lastPosition;

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

            this.StartupService();
        }

        /// <inheritdoc/>
        public int Insert(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (!this.recordValidator.ValidateParameters(record))
            {
                throw new ArgumentException("Invalide parameters");
            }

            if (this.recordsIdDictionary.ContainsKey(record.Id))
            {
                var position = this.recordsIdDictionary[record.Id];
                this.recordsIdDictionary.Remove(record.Id);
                this.recordsIdDictionary[record.Id] = position;
                this.Write(record, position);
            }
            else
            {
                this.Write(record);
                this.recordsIdDictionary[record.Id] = this.lastPosition - 1;
            }

            this.memorizer.Reset();
            return record.Id;
        }

        /// <inheritdoc/>
        public int CreateRecord(ValidationRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            var fileCabinetRecord = new FileCabinetRecord(record, this.id++);
            return this.Insert(fileCabinetRecord);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public Tuple<int, int> GetStat()
        {
            int existsCount = 0;
            int deletedCount = 0;
            foreach (var record in this.GetAnyRecords())
            {
                if ((record.ServiceInormation & 4) != 4)
                {
                    existsCount++;
                }
                else
                {
                    deletedCount++;
                }
            }

            return new Tuple<int, int>(existsCount, deletedCount);
        }

        /// <inheritdoc/>
        public int Purge()
        {
            this.recordsIdDictionary.Clear();
            int offset = 0;
            int iterationIndex = 0;
            foreach (var record in this.GetAnyRecords())
            {
                if ((record.ServiceInormation & 4) != 0)
                {
                    offset++;
                }
                else
                {
                    int position = iterationIndex - offset;
                    if (offset != 0)
                    {
                        this.Write(record, position);
                    }

                    this.recordsIdDictionary.Add(record.Record.Id, position);
                }

                iterationIndex++;
            }

            this.memorizer.Reset();
            this.fileStrieam.SetLength(RecordSize * (iterationIndex - offset));
            return offset;
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.GetRecords().ToArray());
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            foreach (var newRecord in snapshot.Records)
            {
                if (newRecord.Id >= this.id)
                {
                    this.id = newRecord.Id + 1;
                }

                if (this.recordsIdDictionary.ContainsKey(newRecord.Id))
                {
                    var position = this.recordsIdDictionary[newRecord.Id];
                    this.recordsIdDictionary.Remove(newRecord.Id);
                    this.recordsIdDictionary[newRecord.Id] = position;
                    this.Write(newRecord, position);
                }
                else
                {
                    this.Write(newRecord);
                    this.recordsIdDictionary[newRecord.Id] = this.lastPosition - 1;
                }
            }
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<int> Delete(Query query)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var deletedList = new List<int>();
            foreach (var record in this.GetRecords())
            {
                if (query.Predicate(record))
                {
                    deletedList.Add(record.Id);
                    this.Remove(record.Id);
                }
            }

            this.memorizer.Reset();
            return new ReadOnlyCollection<int>(deletedList);
        }

        /// <inheritdoc/>
        public int Update(Query query, Action<FileCabinetRecord> action)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            int count = 0;
            foreach (var record in this.GetRecords())
            {
                if (query.Predicate(record))
                {
                    var position = this.recordsIdDictionary[record.Id];
                    this.recordsIdDictionary.Remove(record.Id);
                    action(record);
                    this.Write(record, position);
                    this.recordsIdDictionary[record.Id] = position;
                    count++;
                }
            }

            this.memorizer.Reset();
            return count;
        }

        /// <inheritdoc/>
        public IEnumerable<List<string>> SelectParameters(Query query, Func<FileCabinetRecord, List<string>> parametersGetter)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (parametersGetter is null)
            {
                throw new ArgumentNullException(nameof(parametersGetter));
            }

            var cached = this.memorizer.GetCached(query.Hash);
            if (cached is null)
            {
                var result = new List<FileCabinetRecord>();
                foreach (var record in this.GetRecords())
                {
                    if (query.Predicate(record))
                    {
                        result.Add(record);
                        yield return parametersGetter(record);
                    }
                }

                this.memorizer.Add(query.Hash, result);
                yield break;
            }
            else
            {
                foreach (var record in cached)
                {
                    yield return parametersGetter(record);
                }

                yield break;
            }
        }

        /// <summary>
        /// Dispose.
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

        private IEnumerable<FileCabonetFilesystemRecord> GetAnyRecords()
        {
            int i = 0;
            while (true)
            {
                var serviceRecord = this.GetRecord(i++);
                if (serviceRecord is null)
                {
                    yield break;
                }

                yield return serviceRecord;
            }
        }

        private IEnumerable<FileCabinetRecord> GetRecords()
        {
            int i = 0;
            while (true)
            {
                var serviceRecord = this.GetRecord(i++);
                if (serviceRecord is null)
                {
                    yield break;
                }

                if (!serviceRecord.IsDeleted())
                {
                    yield return serviceRecord.Record;
                }
            }
        }

        private void Remove(int id)
        {
            if (!this.recordsIdDictionary.ContainsKey(id))
            {
                throw new ArgumentException($"Record {id} does not exists.");
            }
            else
            {
                var position = this.recordsIdDictionary[id];
                var record = this.GetRecord(position);
                this.recordsIdDictionary.Remove(id);
                record.ServiceInormation |= 4;
                this.Write(record, position);
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

            this.Write(fileSystemRecord, this.lastPosition++);
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

        private void StartupService()
        {
            int currentPosition = 0;
            foreach (var record in this.GetAnyRecords())
            {
                if (!record.IsDeleted())
                {
                    currentPosition++;
                    continue;
                }

                if (record.Record.Id > this.id)
                {
                    this.id = record.Record.Id;
                }

                this.lastPosition++;
                this.recordsIdDictionary[record.Record.Id] = currentPosition;
                currentPosition++;
            }
        }
    }
}

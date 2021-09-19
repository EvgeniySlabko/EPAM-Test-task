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

        private readonly SortedDictionary<int, int> recordsIdDictionary = new ();
        private readonly SortedDictionary<string, List<int>> firstNameDictionary = new (StringComparer.InvariantCultureIgnoreCase);
        private readonly SortedDictionary<string, List<int>> lastNameDictionary = new (StringComparer.InvariantCultureIgnoreCase);
        private readonly SortedDictionary<DateTime, List<int>> dateofbirthDictionary = new ();

        private int id;
        private int iterationIndex;
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

            if (generateNewId)
            {
                record.Id = this.id++;
            }

            if (this.recordsIdDictionary.ContainsKey(record.Id))
            {
                int position = this.recordsIdDictionary[record.Id];
                this.RemoveRecordFromDictionaries(record.Id);
                this.AddRecordToDictionaries(record, position);
                this.Write(record, position);
            }
            else
            {
                this.Write(record);
                this.AddRecordToDictionaries(record, this.lastPosition - 1);
            }

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
        /// Returns the number of records in the list.
        /// </summary>
        /// <returns>Number of entries in the list.</returns>
        public Tuple<int, int> GetStat()
        {
            int existsCount = 0;
            int deletedCount = 0;
            while (true)
            {
                var record = this.GetNextAny();
                if (record is null)
                {
                    break;
                }

                if ((record.ServiceInormation & 4) != 4)
                {
                    existsCount++;
                }
                else
                {
                    deletedCount++;
                }
            }

            this.GoToStart();
            return new Tuple<int, int>(existsCount, deletedCount);
        }

        /// <inheritdoc/>
        public void Purge()
        {
            this.GoToStart();
            this.ClearDictionaries();
            int offset = 0;
            int avalibleRecordCounter = 0;
            while (true)
            {
                var record = this.GetNextAny();
                if (record is null)
                {
                    this.fileStrieam.SetLength(avalibleRecordCounter * RecordSize);
                    break;
                }
                else if ((record.ServiceInormation & 4) != 0)
                {
                    offset++;
                    continue;
                }
                else
                {
                    int position = this.iterationIndex - offset - 1;
                    if (offset != 0)
                    {
                        this.Write(record, position);
                    }
                    else
                    {
                        this.AddRecordToDictionaries(record, position);
                    }
                }

                avalibleRecordCounter++;
            }

            this.GoToStart();
        }

        /// <summary>
        /// Takes a snapshot of the current state of the list of records.
        /// </summary>
        /// <returns>Snapshot of the current list of records.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            var records = this.GetRecordsList();
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
                if (this.recordsIdDictionary.ContainsKey(newRecord.Id))
                {
                    var position = this.recordsIdDictionary[newRecord.Id];
                    this.RemoveRecordFromDictionaries(newRecord.Id);
                    this.AddRecordToDictionaries(newRecord, position);
                    this.Write(newRecord, position);
                }
                else
                {
                    this.Write(newRecord);
                    this.AddRecordToDictionaries(newRecord, this.lastPosition - 1);
                }
            }
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<int> Delete(Predicate<FileCabinetRecord> predicate)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var deletedList = new List<int>();
            foreach (var record in this.GetRecords())
            {
                if (predicate(record))
                {
                    deletedList.Add(record.Id);
                    this.Remove(record.Id);
                }
            }

            return new ReadOnlyCollection<int>(deletedList);
        }

        /// <inheritdoc/>
        public int Update(Predicate<FileCabinetRecord> predicate, Action<FileCabinetRecord> action)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            int count = 0;
            foreach (var record in this.GetRecords())
            {
                if (predicate(record))
                {
                    var position = this.recordsIdDictionary[record.Id];
                    this.RemoveRecordFromDictionaries(record.Id);
                    action(record);
                    this.Write(record, position);
                    this.AddRecordToDictionaries(record, position);
                    count++;
                }
            }

            return count;
        }

        /// <inheritdoc/>
        public IEnumerable<List<string>> SelectParameters(Predicate<FileCabinetRecord> predicate, Func<FileCabinetRecord, List<string>> parametersGetter)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (parametersGetter is null)
            {
                throw new ArgumentNullException(nameof(parametersGetter));
            }

            foreach (var record in this.GetRecords())
            {
                if (predicate(record))
                {
                    yield return parametersGetter(record);
                }
            }

            yield break;
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

        private static void RemoveRecordFromDictionary<T>(SortedDictionary<T, List<int>> dictionary, T index, int position)
        {
            var list = dictionary[index];
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == position)
                {
                    list.RemoveAt(i);
                    break;
                }
            }
        }

        private static void AddRecordToDictionary<T>(SortedDictionary<T, List<int>> dictionary, T key, int value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key].Add(value);
            }
            else
            {
                var list = new List<int>
                {
                    value,
                };
                dictionary.Add(key, list);
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

                if ((serviceRecord.ServiceInormation & 4) == 0)
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
                this.RemoveRecordFromDictionaries(id);
                record.ServiceInormation |= 4;
                this.Write(record, position);
            }
        }

        private ReadOnlyCollection<FileCabinetRecord> GetRecordsList()
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

        private void StartupService()
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
                else
                {
                    if (fileSystemRecord.Id > higherId)
                    {
                        higherId = fileSystemRecord.Id;
                    }

                    this.lastPosition++;
                    this.AddRecordToDictionaries(fileSystemRecord, this.iterationIndex - 1);
                }
            }

            this.id = higherId + 1;
        }

        private void AddRecordToDictionaries(FileCabinetRecord record, int position)
        {
            this.recordsIdDictionary[record.Id] = position;
            AddRecordToDictionary(this.firstNameDictionary, record.FirstName, position);
            AddRecordToDictionary(this.lastNameDictionary, record.LastName, position);
            AddRecordToDictionary(this.dateofbirthDictionary, record.DateOfBirth, position);
        }

        private void AddRecordToDictionaries(FileCabonetFilesystemRecord record, int position)
        {
            this.recordsIdDictionary.Add(record.Record.Id, position);
            AddRecordToDictionary(this.firstNameDictionary, record.Record.FirstName, position);
            AddRecordToDictionary(this.lastNameDictionary, record.Record.LastName, position);
            AddRecordToDictionary(this.dateofbirthDictionary, record.Record.DateOfBirth, position);
        }

        private void ClearDictionaries()
        {
            this.recordsIdDictionary.Clear();
            this.firstNameDictionary.Clear();
            this.lastNameDictionary.Clear();
            this.dateofbirthDictionary.Clear();
        }

        private void RemoveRecordFromDictionaries(int id)
        {
            var position = this.recordsIdDictionary[id];
            var record = this.GetRecord(position);
            this.recordsIdDictionary.Remove(id);
            RemoveRecordFromDictionary(this.firstNameDictionary, record.Record.FirstName, position);
            RemoveRecordFromDictionary(this.lastNameDictionary, record.Record.LastName, position);
            RemoveRecordFromDictionary(this.dateofbirthDictionary, record.Record.DateOfBirth, position);
        }
    }
}

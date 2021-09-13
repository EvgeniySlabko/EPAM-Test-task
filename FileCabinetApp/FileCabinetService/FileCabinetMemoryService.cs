using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Service for working with records in memory.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly IRecordValidator recordValidator;

        private readonly List<FileCabinetRecord> list = new ();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();

        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new ();

        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateTimeDictionary = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="recordValidator">Given validator.</param>
        public FileCabinetMemoryService(IRecordValidator recordValidator)
        {
            this.recordValidator = recordValidator;
        }

        /// <summary>
        /// Create new record and adds it to list and dictionaries.
        /// </summary>
        /// <param name="newRecord">Record to add.</param>
        /// <param name="generateNewId">determines whether a new id needs to be generated.</param>
        /// <returns>id of the new record.</returns>
        public int CreateRecord(FileCabinetRecord newRecord, bool generateNewId = true)
        {
            if (newRecord is null)
            {
                throw new ArgumentNullException(nameof(newRecord));
            }

            if (!this.recordValidator.ValidateParameters(newRecord))
            {
                throw new ArgumentException("Invalide parameters");
            }

            if (generateNewId)
            {
                newRecord.Id = this.list.Count + 1;
                this.AddNewRecord(newRecord);
            }
            else
            {
                var record = this.list.Find(r => r.Id.Equals(newRecord.Id));
                if (record is null)
                {
                    this.AddNewRecord(newRecord);
                }
                else
                {
                    // Replace previous record.
                    this.RemoveRecord(record);
                    this.AddNewRecord(newRecord);
                }
            }

            return newRecord.Id;
        }

        /// <summary>
        /// Returns an array with records.
        /// </summary>
        /// <returns>array with records.</returns>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            foreach (var record in this.list)
            {
                yield return record;
            }

            yield break;
        }

        /// <summary>
        /// Returns the number of entries in the list.
        /// </summary>
        /// <returns>Number of entries in the list.</returns>
        public Tuple<int, int> GetStat()
        {
            return new Tuple<int, int>(this.list.Count, 0);
        }

        /// <summary>
        /// Edits the record by its id.
        /// </summary>
        /// <param name="newRecord">Edited record.</param>
        public void Edit(FileCabinetRecord newRecord)
        {
            if (newRecord is null)
            {
                throw new ArgumentNullException(nameof(newRecord));
            }

            if (!this.recordValidator.ValidateParameters(newRecord))
            {
                throw new ArgumentException("Invalide parameters");
            }

            foreach (var record in this.list)
            {
                if (record.Id == newRecord.Id)
                {
                    this.RemoveRecord(record);

                    this.CreateRecord(newRecord, false);

                    return;
                }
            }

            throw new ArgumentException("Id was not found");
        }

        /// <summary>
        /// Find record by its first name.
        /// </summary>
        /// <param name="firstName">First name to search.</param>
        /// <returns>Record if found otherwise null.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (firstName is null)
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            if (this.firstNameDictionary.TryGetValue(firstName.ToLower(CultureInfo.CurrentCulture), out List<FileCabinetRecord> subList))
            {
                foreach (var record in subList)
                {
                    yield return record;
                }
            }

            yield break;
        }

        /// <summary>
        /// Find record by its last name.
        /// </summary>
        /// <param name="lastName">Last name to search.</param>
        /// <returns>Record if found otherwise null.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (lastName is null)
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            if (this.lastNameDictionary.TryGetValue(lastName.ToLower(CultureInfo.CurrentCulture), out List<FileCabinetRecord> subList))
            {
                foreach (var record in subList)
                {
                    yield return record;
                }
            }

            yield break;
        }

        /// <summary>
        /// Find record by its data of birthday.
        /// </summary>
        /// <param name="dataOfBirthday">Вata of birthday to search.</param>
        /// <returns>Record if found otherwise null.</returns>
        public IEnumerable<FileCabinetRecord> FindByDate(DateTime dataOfBirthday)
        {
            if (this.dateTimeDictionary.TryGetValue(dataOfBirthday, out List<FileCabinetRecord> subList))
            {
                foreach (var record in subList)
                {
                    yield return record;
                }
            }

            yield break;
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
                var record = this.list.Find(r => newRecord.Id == r.Id);
                if (record is not null)
                {
                    record = newRecord;
                }
                else
                {
                    this.list.Add(newRecord);
                }
            }
        }

        /// <summary>
        /// Takes a snapshot of the current state of the list of records.
        /// </summary>
        /// <returns>Snapshot of the current list of records.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            var copy = (FileCabinetRecord[])this.list.ToArray().Clone();
            return new FileCabinetServiceSnapshot(copy);
        }

        /// <inheritdoc/>
        public void Remove(int id)
        {
            var record = this.list.Find(f => f.Id == id);
            if (record is not null)
            {
                this.RemoveRecord(record);
            }
            else
            {
                throw new ArgumentException($"Record {id} does not exists.");
            }
        }

        /// <inheritdoc/>
        public void Purge()
        {
            throw new NotImplementedException();
        }

        private static void AddRecordToDictionary<T>(Dictionary<T, List<FileCabinetRecord>> dictionary, FileCabinetRecord record, T index)
        {
            if (dictionary.TryGetValue(index, out List<FileCabinetRecord> subList))
            {
                subList.Add(record);
            }
            else
            {
                subList = new List<FileCabinetRecord>
                {
                    record,
                };
                dictionary.Add(index, subList);
            }
        }

        private void AddNewRecord(FileCabinetRecord newRecord)
        {
            FileCabinetRecord currrentRecord = new ()
            {
                Id = newRecord.Id,
                FirstName = newRecord.FirstName,
                LastName = newRecord.LastName,
                DateOfBirth = newRecord.DateOfBirth,
                IdentificationLetter = newRecord.IdentificationLetter,
                IdentificationNumber = newRecord.IdentificationNumber,
                PointsForFourTests = newRecord.PointsForFourTests,
            };

            this.AddRecordToDictionaries(currrentRecord);
        }

        private void RemoveRecord(FileCabinetRecord record)
        {
            this.firstNameDictionary[record.FirstName.ToLower(CultureInfo.CurrentCulture)].Remove(record);
            this.firstNameDictionary.Remove(record.FirstName.ToLower(CultureInfo.CurrentCulture));

            this.lastNameDictionary[record.LastName.ToLower(CultureInfo.CurrentCulture)].Remove(record);
            this.lastNameDictionary.Remove(record.LastName.ToLower(CultureInfo.CurrentCulture));

            this.dateTimeDictionary[record.DateOfBirth].Remove(record);
            this.dateTimeDictionary.Remove(record.DateOfBirth);

            this.list.Remove(record);
        }

        private void AddRecordToDictionaries(FileCabinetRecord record)
        {
            AddRecordToDictionary(this.firstNameDictionary, record, record.FirstName.ToLower(CultureInfo.CurrentCulture));
            AddRecordToDictionary(this.lastNameDictionary, record, record.LastName.ToLower(CultureInfo.CurrentCulture));
            AddRecordToDictionary(this.dateTimeDictionary, record, record.DateOfBirth);
            this.list.Add(record);
        }
    }
}
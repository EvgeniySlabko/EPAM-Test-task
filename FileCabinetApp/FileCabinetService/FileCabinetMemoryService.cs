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

            FileCabinetRecord currrentRecord = new ()
            {
                Id = generateNewId ? this.list.Count + 1 : newRecord.Id,
                FirstName = newRecord.FirstName,
                LastName = newRecord.LastName,
                DateOfBirth = newRecord.DateOfBirth,

                IdentificationLetter = newRecord.IdentificationLetter,
                IdentificationNumber = newRecord.IdentificationNumber,
                PointsForFourTests = newRecord.PointsForFourTests,
            };

            return this.AddRecordToDictionaries(currrentRecord);
        }

        /// <summary>
        /// Returns an array with records.
        /// </summary>
        /// <returns>array with records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return new ReadOnlyCollection<FileCabinetRecord>(this.list);
        }

        /// <summary>
        /// Returns the number of entries in the list.
        /// </summary>
        /// <returns>Number of entries in the list.</returns>
        public int GetStat()
        {
            return this.list.Count;
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
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (firstName is null)
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            if (this.firstNameDictionary.TryGetValue(firstName.ToLower(CultureInfo.CurrentCulture), out List<FileCabinetRecord> subList))
            {
                return new ReadOnlyCollection<FileCabinetRecord>(subList);
            }

            return null;
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

            if (this.lastNameDictionary.TryGetValue(lastName.ToLower(CultureInfo.CurrentCulture), out List<FileCabinetRecord> subList))
            {
                return new ReadOnlyCollection<FileCabinetRecord>(subList);
            }

            return null;
        }

        /// <summary>
        /// Find record by its data of birthday.
        /// </summary>
        /// <param name="dataOfBirthday">Вata of birthday to search.</param>
        /// <returns>Record if found otherwise null.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDate(DateTime dataOfBirthday)
        {
            if (this.dateTimeDictionary.TryGetValue(dataOfBirthday, out List<FileCabinetRecord> subList))
            {
                return new ReadOnlyCollection<FileCabinetRecord>(subList);
            }

            return null;
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
                for (int i = 0; i < this.list.Count; i++)
                {
                    if (newRecord.Id == this.list[i].Id)
                    {
                        this.list[i] = newRecord;
                        continue;
                    }
                }

                this.list.Add(newRecord);
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
                this.list.Remove(record);
                this.firstNameDictionary.Remove(record.FirstName);
                this.lastNameDictionary.Remove(record.LastName);
                this.dateTimeDictionary.Remove(record.DateOfBirth);
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

        /// <summary>
        /// Remove record from list and dictionaries.
        /// </summary>
        /// <param name="record">Record to remove.</param>
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

        /// <summary>
        /// Takes a snapshot of the current state of the list of records.
        /// </summary>
        /// <returns>Snapshot of the current list of records.</returns>
        private int AddRecordToDictionaries(FileCabinetRecord currrentRecord)
        {
            // Add record in main list
            this.list.Add(currrentRecord);

            // add record in firstNameDictionary
            if (this.firstNameDictionary.TryGetValue(currrentRecord.FirstName.ToLower(CultureInfo.CurrentCulture), out List<FileCabinetRecord> subList))
            {
                subList.Add(currrentRecord);
            }
            else
            {
                subList = new List<FileCabinetRecord>
            {
                currrentRecord,
            };
                this.firstNameDictionary.Add(currrentRecord.FirstName.ToLower(CultureInfo.CurrentCulture), subList);
            }

            // add record in lastNameDictionary
            if (this.lastNameDictionary.TryGetValue(currrentRecord.LastName.ToLower(CultureInfo.CurrentCulture), out subList))
            {
                subList.Add(currrentRecord);
            }
            else
            {
                subList = new List<FileCabinetRecord>
            {
                currrentRecord,
            };
                this.lastNameDictionary.Add(currrentRecord.LastName.ToLower(CultureInfo.CurrentCulture), subList);
            }

            // add record in dateTimeDictionary
            if (this.dateTimeDictionary.TryGetValue(currrentRecord.DateOfBirth, out subList))
            {
                subList.Add(currrentRecord);
            }
            else
            {
                subList = new List<FileCabinetRecord>
            {
                currrentRecord,
            };
                this.dateTimeDictionary.Add(currrentRecord.DateOfBirth, subList);
            }

            return currrentRecord.Id;
        }
    }
}
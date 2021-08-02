using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Service for working with records.
    /// </summary>
    public class FileCabinetService : IFileCabinetService
    {
        private readonly IRecordValidator validator;

        private readonly List<FileCabinetRecord> list = new ();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();

        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new ();

        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateTimeDictionary = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetService"/> class.
        /// </summary>
        /// <param name="validator">Given validator.</param>
        public FileCabinetService(IRecordValidator validator)
        {
            this.validator = validator;
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

            if (!this.validator.ValidateParameters(newRecord))
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

            if (!this.validator.ValidateParameters(newRecord))
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
        /// Takes a snapshot of the current state of the list of records.
        /// </summary>
        /// <returns>Snapshot of the current list of records.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            var copy = (FileCabinetRecord[])this.list.ToArray().Clone();
            return new FileCabinetServiceSnapshot(copy);
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
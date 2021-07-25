using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Service for working with records.
    /// </summary>
    public abstract class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateTimeDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

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

            if (!this.CreateValidator().ValidateParameters(newRecord))
            {
                throw new ArgumentException("Invalide parameters");
            }

            FileCabinetRecord currrentRecord = new FileCabinetRecord
            {
                Id = generateNewId ? this.list.Count + 1 : newRecord.Id,
                FirstName = newRecord.FirstName,
                LastName = newRecord.LastName,
                DateOfBirth = newRecord.DateOfBirth,

                IdentificationLetter = newRecord.IdentificationLetter,
                IdentificationNumber = newRecord.IdentificationNumber,
                PointsForFourTests = newRecord.PointsForFourTests,
            };

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

        /// <summary>
        /// Returns an array with records.
        /// </summary>
        /// <returns>array with records.</returns>
        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
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

            if (!this.CreateValidator().ValidateParameters(newRecord))
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
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            if (firstName is null)
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            if (this.firstNameDictionary.TryGetValue(firstName.ToLower(CultureInfo.CurrentCulture), out List<FileCabinetRecord> subList))
            {
                return subList.ToArray();
            }

            return null;
        }

        /// <summary>
        /// Find record by its last name.
        /// </summary>
        /// <param name="lastName">Last name to search.</param>
        /// <returns>Record if found otherwise null.</returns>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            if (lastName is null)
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            if (this.lastNameDictionary.TryGetValue(lastName.ToLower(CultureInfo.CurrentCulture), out List<FileCabinetRecord> subList))
            {
                return subList.ToArray();
            }

            return null;
        }

        /// <summary>
        /// Find record by its data of birthday.
        /// </summary>
        /// <param name="dataOfBirthday">Вata of birthday to search.</param>
        /// <returns>Record if found otherwise null.</returns>
        public FileCabinetRecord[] FindByDate(DateTime dataOfBirthday)
        {
            if (this.dateTimeDictionary.TryGetValue(dataOfBirthday, out List<FileCabinetRecord> subList))
            {
                return subList.ToArray();
            }

            return null;
        }

        /// <summary>
        /// Methor for validation given parameters.
        /// </summary>
        /// <param name="record">Given recor.</param>
        /// <returns>true if passed validation otherwise false.</returns>
        protected abstract IRecordValidator CreateValidator();

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
    }
}
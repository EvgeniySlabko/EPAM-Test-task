﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Service for working with records in memory.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService, IDisposable
    {
        private readonly IRecordValidator recordValidator;

        private const int RecordSize = 277;
        private const int MaxNameLength = 120;

        private FileStream fileStrieam;
        private BinaryWriter binaryWriter;
        private BinaryReader binaryReader;

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

            short reservedShortVariable = 0;
            this.binaryWriter.Write(reservedShortVariable);

            // ReWrite generation id.
            int id = generateNewId ? 0 : record.Id;
            this.binaryWriter.Write(record.Id);

            this.WriteANSIIStringToFile(record.FirstName);
            this.WriteANSIIStringToFile(record.LastName);

            this.binaryWriter.Write(record.DateOfBirth.Year);
            this.binaryWriter.Write(record.DateOfBirth.Month);
            this.binaryWriter.Write(record.DateOfBirth.Day);
            this.binaryWriter.Write(record.IdentificationNumber);
            this.binaryWriter.Write(record.PointsForFourTests);
            this.binaryWriter.Write(record.IdentificationLetter);
            this.binaryWriter.Flush();
            return id;
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
        /// <param name="newRecord">Edited record.</param>
        void IFileCabinetService.Edit(FileCabinetRecord newRecord)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find record by its data of birthday.
        /// </summary>
        /// <param name="dataOfBirthday">Вata of birthday to search.</param>
        /// <returns>Record if found otherwise null.</returns>
        ReadOnlyCollection<FileCabinetRecord> IFileCabinetService.FindByDate(DateTime dataOfBirthday)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find record by its first name.
        /// </summary>
        /// <param name="firstName">First name to search.</param>
        /// <returns>Record if found otherwise null.</returns>
        ReadOnlyCollection<FileCabinetRecord> IFileCabinetService.FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find record by its last name.
        /// </summary>
        /// <param name="lastName">Last name to search.</param>
        /// <returns>Record if found otherwise null.</returns>
        ReadOnlyCollection<FileCabinetRecord> IFileCabinetService.FindByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns all records.
        /// </summary>
        /// <returns>array with records.</returns>
        ReadOnlyCollection<FileCabinetRecord> IFileCabinetService.GetRecords()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the number of records in the list.
        /// </summary>
        /// <returns>Number of entries in the list.</returns>
        int IFileCabinetService.GetStat()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Takes a snapshot of the current state of the list of records.
        /// </summary>
        /// <returns>Snapshot of the current list of records.</returns>
        FileCabinetServiceSnapshot IFileCabinetService.MakeSnapshot()
        {
            throw new NotImplementedException();
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

        private void WriteANSIIStringToFile(string str)
        {
            byte[] nameBytes = Encoding.ASCII.GetBytes(str);
            var stringBuffer = new byte[MaxNameLength];
            int stringLength = (nameBytes.Length > MaxNameLength) ? MaxNameLength : nameBytes.Length;

            Array.Copy(nameBytes, 0, stringBuffer, 0, stringLength);
            this.binaryWriter.Write(stringBuffer);
        }
    }
}
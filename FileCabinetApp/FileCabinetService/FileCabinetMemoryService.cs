using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    /// <summary>
    /// Service for working with records in memory.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        // This value will always be 0 for memory service.
        private const int DeletedRecords = 0;

        private readonly IRecordValidator recordValidator;

        private readonly List<FileCabinetRecord> list = new ();

        private readonly Memorizer memorizer = new ();
        private int id;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="recordValidator">Given validator.</param>
        public FileCabinetMemoryService(IRecordValidator recordValidator)
        {
            this.recordValidator = recordValidator;
        }

        /// <inheritdoc/>
        public int Insert(FileCabinetRecord newRecord)
        {
            if (newRecord is null)
            {
                throw new ArgumentNullException(nameof(newRecord));
            }

            if (!this.recordValidator.ValidateParameters(newRecord))
            {
                throw new ArgumentException("Invalide parameters");
            }

            var record = this.list.Find(r => r.Id.Equals(newRecord.Id));
            if (record is null)
            {
                this.AddNewRecord(newRecord);
            }
            else
            {
                // Replace previous record.
                this.list.Remove(record);
                this.AddNewRecord(newRecord);
            }

            return newRecord.Id;
        }

        /// <inheritdoc/>
        public int CreateRecord(ValidationRecord newRecord)
        {
            if (newRecord is null)
            {
                throw new ArgumentNullException(nameof(newRecord));
            }

            var record = new FileCabinetRecord(newRecord, this.list.Count + 1);
            return this.Insert(record);
        }

        /// <inheritdoc/>
        public Tuple<int, int> GetStat()
        {
            return new Tuple<int, int>(this.list.Count, DeletedRecords);
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
                var record = this.list.Find(r => newRecord.Id == r.Id);
                if (record.Id >= this.id)
                {
                    this.id = record.Id + 1;
                }

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

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            var copy = (FileCabinetRecord[])this.list.ToArray().Clone();
            return new FileCabinetServiceSnapshot(copy);
        }

        /// <inheritdoc/>
        public int Purge()
        {
            return DeletedRecords;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<int> Delete(Query query)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var deletedRecordsId = new List<int>();
            var deletedRecords = new List<FileCabinetRecord>();
            foreach (var record in this.list)
            {
                if (query.Predicate(record))
                {
                    deletedRecordsId.Add(record.Id);
                    deletedRecords.Add(record);
                }
            }

            deletedRecords.ForEach(r => this.list.Remove(r));
            return new ReadOnlyCollection<int>(deletedRecordsId);
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
            var recordsToChange = new List<FileCabinetRecord>();
            foreach (var record in this.list)
            {
                if (query.Predicate(record))
                {
                    action(record);
                    count++;
                }
            }

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

            var cachedRecords = this.memorizer.GetCached(query.Hash);
            var recordsForCaching = new List<FileCabinetRecord>();
            if (cachedRecords is null)
            {
                foreach (var record in this.list)
                {
                    if (query.Predicate(record))
                    {
                        recordsForCaching.Add(record);
                        yield return parametersGetter((FileCabinetRecord)record.Clone());
                    }
                }

                this.memorizer.Add(query.Hash, recordsForCaching);
                yield break;
            }
            else
            {
                foreach (var record in cachedRecords)
                {
                    yield return parametersGetter((FileCabinetRecord)record.Clone());
                }

                yield break;
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

            this.list.Add(currrentRecord);
        }
    }
}
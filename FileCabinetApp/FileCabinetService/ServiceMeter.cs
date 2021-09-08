using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Service meter.
    /// </summary>
    public class ServiceMeter : IFileCabinetService
    {
        private readonly IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMeter"/> class.
        /// </summary>
        /// <param name="service">Service.</param>
        public ServiceMeter(IFileCabinetService service)
        {
            this.service = service;
        }

        /// <inheritdoc/>
        public int CreateRecord(FileCabinetRecord newRecord, bool generateNewId = true)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = this.service.CreateRecord(newRecord, generateNewId);
            stopWatch.Stop();
            Console.WriteLine(StringManager.Rm.GetString("DisplayInfoPatternString", CultureInfo.CurrentCulture), "Create", stopWatch.ElapsedTicks);
            return result;
        }

        /// <inheritdoc/>
        public void Edit(FileCabinetRecord newRecord)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            this.service.Edit(newRecord);
            stopWatch.Stop();
            Console.WriteLine(StringManager.Rm.GetString("DisplayInfoPatternString", CultureInfo.CurrentCulture), "edit", stopWatch.ElapsedTicks);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByDate(DateTime dataOfBirthday)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = this.service.FindByDate(dataOfBirthday);
            stopWatch.Stop();
            Console.WriteLine(StringManager.Rm.GetString("DisplayInfoPatternString", CultureInfo.CurrentCulture), "Find", stopWatch.ElapsedTicks);
            return result;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = this.service.FindByFirstName(firstName);
            stopWatch.Stop();
            Console.WriteLine(StringManager.Rm.GetString("DisplayInfoPatternString", CultureInfo.CurrentCulture), "Find", stopWatch.ElapsedTicks);
            return result;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = this.service.FindByLastName(lastName);
            stopWatch.Stop();
            Console.WriteLine(StringManager.Rm.GetString("DisplayInfoPatternString", CultureInfo.CurrentCulture), "Find", stopWatch.ElapsedTicks);
            return result;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = this.service.GetRecords();
            stopWatch.Stop();
            Console.WriteLine(StringManager.Rm.GetString("DisplayInfoPatternString", CultureInfo.CurrentCulture), "list", stopWatch.ElapsedTicks);
            return result;
        }

        /// <inheritdoc/>
        public Tuple<int, int> GetStat()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = this.service.GetStat();
            stopWatch.Stop();
            Console.WriteLine(StringManager.Rm.GetString("DisplayInfoPatternString", CultureInfo.CurrentCulture), "stat", stopWatch.ElapsedTicks);
            return result;
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = this.service.MakeSnapshot();
            stopWatch.Stop();
            Console.WriteLine(StringManager.Rm.GetString("DisplayInfoPatternString", CultureInfo.CurrentCulture), "makeSnapshot", stopWatch.ElapsedTicks);
            return result;
        }

        /// <inheritdoc/>
        public void Purge()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            this.service.Purge();
            stopWatch.Stop();
            Console.WriteLine(StringManager.Rm.GetString("DisplayInfoPatternString", CultureInfo.CurrentCulture), "purge", stopWatch.ElapsedTicks);
        }

        /// <inheritdoc/>
        public void Remove(int id)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            this.service.Remove(id);
            stopWatch.Stop();
            Console.WriteLine(StringManager.Rm.GetString("DisplayInfoPatternString", CultureInfo.CurrentCulture), "remove", stopWatch.ElapsedTicks);
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            this.service.Restore(snapshot);
            stopWatch.Stop();
            Console.WriteLine(StringManager.Rm.GetString("DisplayInfoPatternString", CultureInfo.CurrentCulture), "restore", stopWatch.ElapsedTicks);
        }
    }
}

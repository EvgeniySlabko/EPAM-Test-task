using System;
using System.Collections.Generic;
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
        public int CreateRecord(ValidationRecord newRecord)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = this.service.CreateRecord(newRecord);
            stopWatch.Stop();
            Console.WriteLine(StringManager.Rm.GetString("DisplayInfoPatternString", CultureInfo.CurrentCulture), "Create", stopWatch.ElapsedTicks);
            return result;
        }

        /// <inheritdoc/>
        public int Insert(FileCabinetRecord newRecord)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = this.service.Insert(newRecord);
            stopWatch.Stop();
            Console.WriteLine(StringManager.Rm.GetString("DisplayInfoPatternString", CultureInfo.CurrentCulture), "Insert", stopWatch.ElapsedTicks);
            return result;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<int> Delete(Query query)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = this.service.Delete(query);
            stopWatch.Stop();
            Console.WriteLine(StringManager.Rm.GetString("DisplayInfoPatternString", CultureInfo.CurrentCulture), "Delete", stopWatch.ElapsedTicks);
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
        public int Purge()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = this.service.Purge();
            stopWatch.Stop();
            Console.WriteLine(StringManager.Rm.GetString("DisplayInfoPatternString", CultureInfo.CurrentCulture), "purge", stopWatch.ElapsedTicks);
            return result;
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

        /// <inheritdoc/>
        public IEnumerable<List<string>> SelectParameters(Query query, Func<FileCabinetRecord, List<string>> parameters)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = this.service.SelectParameters(query, parameters);
            stopWatch.Stop();
            Console.WriteLine(StringManager.Rm.GetString("DisplayInfoPatternString", CultureInfo.CurrentCulture), "select", stopWatch.ElapsedTicks);
            return result;
        }

        /// <inheritdoc/>
        public int Update(Query query, Action<FileCabinetRecord> action)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = this.service.Update(query, action);
            stopWatch.Stop();
            Console.WriteLine(StringManager.Rm.GetString("DisplayInfoPatternString", CultureInfo.CurrentCulture), "update", stopWatch.ElapsedTicks);
            return result;
        }
    }
}

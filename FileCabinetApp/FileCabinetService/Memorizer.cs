using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Memorizer.
    /// </summary>
    public class Memorizer
    {
        private readonly List<Tuple<long, List<FileCabinetRecord>>> cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="Memorizer"/> class.
        /// </summary>
        public Memorizer()
        {
            this.cache = new List<Tuple<long, List<FileCabinetRecord>>>();
        }

        /// <summary>
        /// Add cached.
        /// </summary>
        /// <param name="hash">Hash.</param>
        /// <param name="list">Result.</param>
        public void Add(long hash, List<FileCabinetRecord> list)
        {
            this.cache.Add(new Tuple<long, List<FileCabinetRecord>>(hash, list));
        }

        /// <summary>
        /// Find cached result using hash.
        /// </summary>
        /// <param name="hash">Hash.</param>
        /// <returns>Cached data.</returns>
        public List<FileCabinetRecord> GetCached(long hash)
        {
            var result = this.cache.Find(t => t.Item1 == hash);
            return (result is null) ? null : result.Item2;
        }

        /// <summary>
        /// Clear cache.
        /// </summary>
        public void Reset()
        {
            this.cache.Clear();
        }
    }
}

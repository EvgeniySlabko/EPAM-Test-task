using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Query.
    /// </summary>
    public class Query
    {
        /// <summary>
        /// Gets or sets hash.
        /// </summary>
        /// <value>Hash.</value>
        public long Hash { get; set; }

        /// <summary>
        /// Gets or sets predicate.
        /// </summary>
        /// <value>Predicate.</value>
        public Predicate<FileCabinetRecord> Predicate { get; set; }
    }
}

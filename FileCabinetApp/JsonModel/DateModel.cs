using System;
using Newtonsoft.Json;

namespace FileCabinetApp
{
    /// <summary>
    /// Lastname Json model.
    /// </summary>
    public class DateModel
    {
        /// <summary>
        /// Gets or sets minimal date.
        /// </summary>
        /// <value>Minimal value.</value>
        [JsonProperty("from")]
        public DateTime From { get; set; }

        /// <summary>
        /// Gets or sets maximum date.
        /// </summary>
        /// <value>Maximum vale.</value>
        [JsonProperty("to")]
        public DateTime To { get; set; }
    }
}

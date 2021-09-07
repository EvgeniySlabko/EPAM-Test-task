using Newtonsoft.Json;

namespace FileCabinetApp
{
    /// <summary>
    /// Firstname Json model.
    /// </summary>
    public class FirstNameModel
    {
        /// <summary>
        /// Gets or sets minimal length.
        /// </summary>
        /// <value>Record first name.</value>
        [JsonProperty("min")]
        public int Min { get; set; }

        /// <summary>
        /// Gets or sets maximum length.
        /// </summary>
        /// <value>Record first name.</value>
        [JsonProperty("max")]
        public int Max { get; set; }
    }
}

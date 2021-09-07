using Newtonsoft.Json;

namespace FileCabinetApp
{
    /// <summary>
    /// Firstname Json model.
    /// </summary>
    public class IdentificationNumberModel
    {
        /// <summary>
        /// Gets or sets minimal length.
        /// </summary>
        /// <value>Record first name.</value>
        [JsonProperty("min")]
        public decimal Min { get; set; }

        /// <summary>
        /// Gets or sets maximum length.
        /// </summary>
        /// <value>Record first name.</value>
        [JsonProperty("max")]
        public decimal Max { get; set; }
    }
}

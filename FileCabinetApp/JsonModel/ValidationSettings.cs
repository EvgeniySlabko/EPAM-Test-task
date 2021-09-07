using Newtonsoft.Json;

namespace FileCabinetApp
{
    /// <summary>
    /// Validation parameters model.
    /// </summary>
    public class ValidationSettings
    {
        /// <summary>
        /// Gets or sets FirstName model.
        /// </summary>
        /// <value>FirstNameModel.</value>
        [JsonProperty("firstName")]
        public FirstNameModel FirstName { get; set; }

        /// <summary>
        /// Gets or sets LastNameModel model.
        /// </summary>
        /// <value>LastNameModel.</value>
        [JsonProperty("lastName")]
        public LastNameModel LastName { get; set; }

        /// <summary>
        /// Gets or sets DateModel model.
        /// </summary>
        /// <value>DateModel.</value>
        [JsonProperty("dateOfBirth")]
        public DateModel DateModel { get; set; }

        /// <summary>
        /// Gets or sets PointsModel model.
        /// </summary>
        /// <value>PointsModel.</value>
        [JsonProperty("points")]
        public PointsModel PointsModel { get; set; }

        /// <summary>
        /// Gets or sets IdentificationNumberModel model.
        /// </summary>
        /// <value>IdentificationNumberModel.</value>
        [JsonProperty("identificationNumber")]
        public IdentificationNumberModel IdentificationNumberModel { get; set; }
    }
}

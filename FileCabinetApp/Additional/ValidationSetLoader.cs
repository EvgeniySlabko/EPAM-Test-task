using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FileCabinetApp
{
    /// <summary>
    /// Valiadation settings loader.
    /// </summary>
    public static class ValidationSetLoader
    {
        /// <summary>
        /// Load rules from file.
        /// </summary>
        /// <param name="path">Path to the file with rules.</param>
        /// <returns>Rulesets. Key - rule name.</returns>
        public static Dictionary<string, ValidationSettings> LoadRules(string path)
        {
            Dictionary<string, ValidationSettings> validationRules;

            using (var fs = new StreamReader(path))
            {
                var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = "d/M/yyyy" };
                validationRules = JsonConvert.DeserializeObject<Dictionary<string, ValidationSettings>>(fs.ReadToEnd(), dateTimeConverter);
            }

            return validationRules;
        }
    }
}

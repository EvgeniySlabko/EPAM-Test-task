using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Global constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Gets or sets ValidationSettingsFileName.
        /// </summary>
        /// <value>Validation settings file name.</value>
        public const string ValidationSettingsFileName = "validation-rules.json";

        /// <summary>
        /// Gets or sets DefaultValidationSettingsName.
        /// </summary>
        /// <value>Default validation settings name.</value>
        public const string DefaultValidationSettingsName = "default";

        /// <summary>
        /// Gets or sets CustomValidationSettingsName.
        /// </summary>
        /// <value>Custom validation settings name.</value>
        public const string CustomValidationSettingsName = "custom";

        /// <summary>
        /// Gets or sets DefaultServiceType.
        /// </summary>
        /// <value>Default service type.</value>
        public const ServiceType DefaultServiceType = ServiceType.MemoryService;
    }
}

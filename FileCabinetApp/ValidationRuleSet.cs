using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Set of rules for validation.
    /// </summary>
    public class ValidationRuleSet
    {
        /// <summary>
        /// Gets or sets first name validator deleate.
        /// </summary>
        /// <value>First name vaidator delegate.</value>
        public StringValidator FirstNameVAidator { get; set; }

        /// <summary>
        /// Gets or sets last name validator deleate.
        /// </summary>
        /// <value>Last name vaidator delegate.</value>
        public StringValidator LastNameValidator { get; set; }

        /// <summary>
        /// Gets or sets id validator deleate.
        /// </summary>
        /// <value>Id vaidator delegate.</value>
        public IntValidator Id { get; set; }

        /// <summary>
        /// Gets or sets Date validator deleate.
        /// </summary>
        /// <value>Date vaidator delegate.</value>
        public DateValidator DateValidator { get; set; }

        /// <summary>
        /// Gets or sets points for four tests validator deleate.
        /// </summary>
        /// <value>Points for four tests vaidator delegate.</value>
        public ShortValidator PointsForFourTestsValidator { get; set; }

        /// <summary>
        /// Gets or sets identification number validator deleate.
        /// </summary>
        /// <value>Identification number vaidator delegate.</value>
        public DecimalValidator IdentificationNumberValidator { get; set; }

        /// <summary>
        /// Gets or sets identification letter validator deleate.
        /// </summary>
        /// <value>Identification letter vaidator delegate.</value>
        public CharValidator IdentificationLetterValidator { get; set; }
    }
}

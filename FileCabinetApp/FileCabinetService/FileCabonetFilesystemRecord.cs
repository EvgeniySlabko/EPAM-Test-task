using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Filesystem record.
    /// </summary>
    public class FileCabonetFilesystemRecord
    {
        /// <summary>
        /// Gets or sets record.
        /// </summary>
        /// <value>record.</value>
        public FileCabinetRecord Record { get; set; }

        /// <summary>
        /// Gets or sets record service information in file system.
        /// </summary>
        /// <value>Record service information.</value>
        public short ServiceInormation { get; set; }
    }
}

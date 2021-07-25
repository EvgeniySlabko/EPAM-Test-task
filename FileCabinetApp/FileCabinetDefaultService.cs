using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// File cabinet service with default parameters validation.
    /// </summary>
    public class FileCabinetDefaultService : FileCabinetService
    {
        protected override IRecordValidator CreateValidator()
        {
            return new DefaultValidator();
        }
    }
}

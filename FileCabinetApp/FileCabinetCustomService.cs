using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// File cabinet service with custom parameters validation.
    /// </summary>
    public class FileCabinetCustomService : FileCabinetService
    {
        protected override IRecordValidator CreateValidator()
        {
            return new CustomValidator();
        }
    }
}

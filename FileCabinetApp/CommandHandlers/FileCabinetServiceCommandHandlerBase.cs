using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Base class for all service handlers.
    /// </summary>
    public class FileCabinetServiceCommandHandlerBase : CommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceCommandHandlerBase"/> class.
        /// </summary>
        /// <param name="command">Serice command.</param>
        /// <param name="service">Service.</param>
        public FileCabinetServiceCommandHandlerBase(string command, IFileCabinetService service)
           : base(command)
        {
            this.Service = service;
        }

        /// <summary>
        /// Gets service.
        /// </summary>
        /// <value>Service.</value>
        protected IFileCabinetService Service { get; private set; }
    }
}
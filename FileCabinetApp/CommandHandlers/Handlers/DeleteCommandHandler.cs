using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Delete command handler.
    /// </summary>
    public class DeleteCommandHandler : FileCabinetServiceCommandHandlerBase
    {
        private const string Command = "delete";

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service.</param>
        public DeleteCommandHandler(IFileCabinetService service)
            : base(Command, service)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (this.CheckCommand(commandRequest))
            {
                var result = new Parser().WhereParser(commandRequest.Parameters, out Query query);
                if (result.Item1)
                {
                    this.Delete(query);
                }
                else
                {
                    Console.WriteLine(result.Item2);
                }
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Delete(Query query)
        {
            var ids = this.Service.Delete(query);
            if (ids.Count != 0)
            {
                var idsStr = ids.Select(i => "#" + i.ToString(CultureInfo.CurrentCulture)).ToArray();
                var result = string.Join(", ", idsStr);
                Console.WriteLine($"Records {result} are deleted.");
            }
            else
            {
                Console.WriteLine(StringManager.Rm.GetString("NoRecordsMatchingTheseParameters", CultureInfo.CurrentCulture));
            }
        }
    }
}
